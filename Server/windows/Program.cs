using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Concurrent;

using MessageTypes;
using DungeonNamespace;
using PlayerN;
using Utilities;

namespace Server
{
    class Program
    {
        static private List<Player> clientList = new List<Player>();

        static private int clientID = 1;


        static private Dungeon Dungeon;

        //The queue where all the function calls are added to
        static private ConcurrentQueue<Action> RequestQueue = new ConcurrentQueue<Action>();

        // class used for all database queries 
        static private SqlWrapper sqlWrapper;

        //List of all objects in the game, including rooms, items and weapons
        static private GameObjectList AllItems;

        private static String[] IP = { "127.0.0.1", "46.101.88.130", "192.168.1.101" };

        private static int ipIndex = 0;

        /**
         *Creates an sends the map information message type for the dungeon drawer 
         * @param player the player to get the message
         */
        static void SendDungeonInfo(Player player)
        {
            MapLayout ML = new MapLayout();
            ML.mapInfo = Dungeon.GetDungeonString();
            MemoryStream outStream = ML.WriteData(player.Salt);

            try
            {
                player.Socket.Send(outStream.ToArray());
            }
            catch
            {
                Console.Write("problem sending");
                RemoveClientByPlayer(player);
            }
        }

       /**
         *Creates an sends the Dungone response message type 
         * @param player the player to get the message
         * @param response the String that is the dungeons response
         */
        static void SendDungeonResponse(Player player, String response)
        {
            DungeonResponse msg = new DungeonResponse();
            msg.response = response;
            MemoryStream outStream = msg.WriteData(player.Salt);

            try
            {
                player.Socket.Send(outStream.ToArray());
            }
            catch (System.Exception)
            {
                RemoveClientByPlayer(player);
            }
        }

        /**
         *Removes the client from the client list
         * @param player the player to be removed
         */
        static void RemoveClientByPlayer(Player player)
        {
            if (player != null && clientList.Contains(player))
            {
                sqlWrapper.RemoveActivePlayer(player.PlayerName);
                clientList.Remove(player);
            }
        }

        /**
         *Creates and sends the map update message type
         * @param str the string that will be udpdated to players text
         * @param player the player to send the update message to
         */
        static void SendUpdateMessage(String str, Player player)
        {
            UpdateChat UC = new UpdateChat();
            UC.message = str;
            MemoryStream stream = UC.WriteData(player.Salt);
            player.Socket.Send(stream.ToArray());
        }

        /**
        *Inform players in the room that a player enetered
        * @param player the player that enetered the room
        */
        static void JoinedPlayers(Player player)
        {
            foreach (Player p in clientList)
            {
                if (p != player && p.RoomIndex == player.RoomIndex)
                {
                    SendUpdateMessage(p.PlayerName + " entered", p);
                }
            }
        }

        /**
        *Called when a player uses the Say command, it will inform all players in the room of what was said
        * @param player player that spoke
        * @param message the thing the player said
        */
        static void PlayerSpoken(Player player, string message)
        {
            String str = player.PlayerName;

            foreach (Player p in clientList)
            {
                //Send YOU said X to the player that spok, otherwise the player name
                if (p.PlayerName == player.PlayerName) str = "You ";
                else { str = player.PlayerName; };

                if (p.RoomIndex == player.RoomIndex)
                {
                    SendUpdateMessage(str + " Said:  " + message , p);
                }
            }
        }

        /**
        *Sends a message to all players of you leaving the room
        * @param player that left the room
        * @int room the room the player left
        */
        static void LeftPlayers(Player player, int room)
        {
            foreach (Player p in clientList)
            {
                if (p != player && p.RoomIndex == room)
                {
                    SendUpdateMessage("Player: " + p.PlayerName + " left", p);
                }
            }
        }

        /**
        *probably the most called function, It handles all actions to do with the dungeon.
        * 
        * @param dungMsg the action to send to the dungeon
        * @int player the player who is acting upon the dungeon
        */
        static void DungeonAction(String dungMsg, Player player)
        { 
            //A custom class to help translate what happened as a result of a players action
            ActionResponse response  = Dungeon.PlayerAction(dungMsg, player, sqlWrapper);
            switch (response.ID)
            {
                //if the player moved, do requred moved stuff
                case ActionID.MOVE:
                    JoinedPlayers(player);
                    SendLocations();
                    SendDungeonResponse(player, response.message);
                    break;

                //If they did action that requires an update (pickup, drop item etc.)
                case ActionID.UPDATE:
                    SendUpdateMessage(response.message, player);
                    break;

                // If they spoke let other people know
                case ActionID.SAY:
                    PlayerSpoken(player, response.message);
                    break;

                //The standard one, this will clear the clients text and add new text.
                case ActionID.NORMAL:
                    if (response.message == "") return;
                    SendDungeonResponse(player, response.message);
                    break;
            }
        }

        /**
         * Adds a player to the client list, effectively making them part of the game 
         * @param player the player to be added
         */
        static void AddNewPlayer(Player player)
        {
            Room r;

            //get random room if they're a new player
            if (player.RoomIndex == -1 || player.RoomIndex > Dungeon.GetRoomList().Count )
            {
                r = Dungeon.GetRandomRoom();
            }
            else
            {
                r = Dungeon.GetRoomList()[player.RoomIndex];
            }

            //add to the required stuffs
            player.SetRoom(r.RoomIndex);
            sqlWrapper.UpdatePlayerPos(player);
            clientList.Add(player);
        }

        /**
         *If exsisting player, send them the salt to their password
         * @param s socket to send the salt to
         * @param salt the salt to send
         */
        static void SendSalt(Socket s, String salt)
        {
            SaltRequest SM = new SaltRequest();
            SM.message = salt;
            MemoryStream stream = SM.WriteData("This does not matter as it will not be used");
            s.Send(stream.ToArray());
        }

        /**
         *Cycles through invoking any actions in the queue if there are any 
         */
        static void ProcessRequestQueue()
        {
            while (true)
            {
                if (!RequestQueue.IsEmpty)
                {
                    Action action;
                    RequestQueue.TryDequeue(out action);
                    action.Invoke();
                }
            }
        }

        /**
         *Send the location of all players to all players, so that they can be drawn on the map
         * This should only be called when a player moves or a new player joins
         */
        public static void SendLocations()
        {
            //created a string with the player names and their locations
            String rStr = "&";
            foreach (Player p in clientList)
            {
                if (p.RoomIndex != -1)
                {
                    rStr += p.PlayerName + " " + p.RoomIndex + "&";
                }

            }

            PlayerLocations m = new PlayerLocations();
            m.LocationString = rStr;

            //send that message to all the players
            foreach (Player p in clientList)
            {
                try
                {
                    MemoryStream outStream = m.WriteData(p.Salt);
                    p.Socket.Send(outStream.ToArray());
                }
                catch (System.Exception)
                {
                    RemoveClientByPlayer(p);
                }
            }
        }

        /**
         *This function Sends a message back to the client to affirm the salt has been recieved.
         * Now the salt has been receieved the client can now safely send the encrypted create user message.
         * @param player the player to send the request too
         */
        public static void SendSaltRecieved(Player player)
        {
            SaltRequest sr = new SaltRequest();
            sr.message = "Salt recieved send create user now";
            try
            {
                MemoryStream outStream = sr.WriteData(player.Salt);
                player.Socket.Send(outStream.ToArray());
            }
            catch (System.Exception)
            {
                RemoveClientByPlayer(player);
            }
        }

        /**
         * 
         */
        static bool SendLoginResponse(Player p, String msg, bool success)
        {
            LoginResponse response = new LoginResponse();
            response.message = msg;
            if (success)
            {
                response.loggedIn = "1";
            }
            else
            {
                response.loggedIn = "0";
            }
            MemoryStream outStream = response.WriteData(p.Salt);

            try
            {
                p.Socket.Send(outStream.ToArray());
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        /**
         *Main thread for receiving and incoming client buffers. 
         */
        static void ReceiveClientProcess(Object o)
        {
            //initialise varliable
            bool bQuit = false;

            Socket chatClient = (Socket)o;

            bool LoggedIn = false;

            bool ShouldEncrypt = false;

            //init a new player, might not be used, just easier to have all the varilables
            //in one object
            Player player = new Player(chatClient);

            //Stay stuck in the log in squence until the connection drops or they successfuly login
            while (!LoggedIn && bQuit == false)
            {
                LoggedIn = LoginSequence(ref player, ref bQuit, ref ShouldEncrypt);
            }

            //if they exited the login sequence because they quite, return function
            if (bQuit == true) return;

            //Send the start up stuff  needed small delays to give the client time to catchup
            RequestQueue.Enqueue(() => AddNewPlayer(player));

            Thread.Sleep(200);

            RequestQueue.Enqueue(() => DungeonAction("look", player));

            Thread.Sleep(200);

            RequestQueue.Enqueue(() => SendDungeonInfo(player));

            Thread.Sleep(200);

            RequestQueue.Enqueue(() => SendLocations());

            //main process loop
            while (bQuit == false)
            {
                try
                { 
                    byte[] buffer = new byte[4096];
                    int result;

                    result = chatClient.Receive(buffer);

                    if (result > 0 )
                    {

                        Msg m = Msg.DecodeStream(buffer, player.Salt, ShouldEncrypt);

                        if (m != null)
                        {

                            switch (m.mID)
                            {
                                case DungeonCommand.ID:
                                    {
                                        DungeonCommand dungMsg = (DungeonCommand)m;
                                        RequestQueue.Enqueue(() => DungeonAction(dungMsg.command, player));
                                    }
                                    break;

                                    //sometimes the map info does work quite right this is here if that happens
                                    //the client side will request a new one
                                case MapLayout.ID:
                                    RequestQueue.Enqueue(()=>SendDungeonInfo(player));
                                    break;

                                default:
                                    break;
                            }
                        }
                    }
                    else
                    {
                       // No messages to see here!
                    }
                }
                catch (Exception)
                {
                    //lost client, add a remove from queue call print to console
                    bQuit = true;
                    String output = "Lost client: " + player.PlayerName;
                    Console.WriteLine(output);

                    RequestQueue.Enqueue(() =>RemoveClientByPlayer(player));
                }
            }
        }
        
        /**
         *The main loop for loging the player in, Some code duplication with recieve client process.
         * I felt it looked nicer and was easier if the login was kept apart from the other message types.
         * @param player A refernce to the player state so far, this will become fleshed out when the required details are forthcoming
         * @param bQuit primitives are passed by boolean by defualt.
         */
        static bool LoginSequence(ref Player player, ref bool bQuit, ref bool shouldDecrypt)
        {
            try
            {
                byte[] buffer = new byte[4096];
                int result;

                result = player.Socket.Receive(buffer);

                if (result > 0)
                {
                    Msg m = Msg.DecodeStream(buffer, player.Salt, shouldDecrypt);

                    switch (m.mID)
                    {
                        //called when a player sends the username to the server, if the player is existing send back salt
                        case SaltRequest.ID:
                            SaltRequest SM = (SaltRequest)m;
                            lock (sqlWrapper)
                            {
                                //check to see if the salt value is valid
                                player.Salt = sqlWrapper.GetSalt(SM.message);
                                if (player.Salt != "")
                                {
                                    SendSalt(player.Socket, player.Salt);
                                    shouldDecrypt = true;
                                }
                                else
                                {
                                    SendLoginResponse(player, "Failed No Such login", false);
                                }

                            }
                            break;
                        //called at the start of the create user. The client sends the salt at which point
                        //the server sends back a message saying its receivedd. The the encrypted createsuer can be proccessed.
                        case SaltSend.ID:
                            {
                                SaltSend ss = (SaltSend)m;
                                player.Salt = ss.salt;
                                SendSaltRecieved(player);
                                shouldDecrypt = true;
                            }
                            break;
                            //Called when the log in message arrives.
                        case LoginMessage.ID:
                            {
                                LoginMessage LM = (LoginMessage)m;
                                Console.WriteLine("Login request from: " + LM.name);

                                //queueing it with the rest and trapping the result of bool from another thread seem annoying
                                //Also it would have to have access to something back in this thread, which would also require a lock
                                //no way around locking something
                                lock (sqlWrapper)
                                {
                                    if (sqlWrapper.GetPlayerLogin(ref player, LM.name, LM.password))
                                    {
                                        //successful log in
                                        Console.WriteLine("Player: " + LM.name + "Logged in");
                                        SendLoginResponse(player, "Success", true);
                                        shouldDecrypt = true;

                                        return true;
                                    }
                                    else
                                    {
                                        //failed log in 
                                        Console.WriteLine("Player: " + LM.name + "Failed Login");
                                        SendLoginResponse(player, "Failed to login, detailes didnt match", false);
                                        shouldDecrypt = false;
                                        return false;
                                    }
                                }

                            }

                            //Triggered if a create user message arrives
                        case CreateUser.ID:
                            {
                                //set up the new player that was created
                                CreateUser CM = (CreateUser)m;
                                player.PlayerName = CM.name;
                                Console.WriteLine("Create User recieved: " + CM.name);

                                lock (sqlWrapper)
                                {
                                    //check to see if the new player is valid
                                    if (sqlWrapper.AddPlayer(player, CM.password, player.Salt))
                                    {
                                        Console.Write(" created new player");
                                        SendLoginResponse(player, "Success", true);
                                        shouldDecrypt = true;
                                        return true;
                                    }
                                    else
                                    {
                                        Console.Write("Failed to create player");
                                        SendLoginResponse(player, "Failed to Create player, username might already exist", false);
                                        shouldDecrypt = false;
                                        return false;
                                    }
                                }
                            }
                    }
                }
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error loggin in: " + e);
                bQuit = true;
                return false;
            }
        }

        static void Main(string[] args)
        {

            Console.WriteLine("Should Create new Map?");
            var response = Console.ReadLine();

            AllItems = new GameObjectList();

            sqlWrapper = new SqlWrapper(AllItems);

            //Create New dungeon if you want to, or one does not exsist
            if (response.ToLower() == "yes")
            {
                Dungeon = new Dungeon();
                Dungeon.Init(100, AllItems);
                sqlWrapper.AddDungeon(Dungeon);
                sqlWrapper.AddItems(50, 20);
            }
            else
            {
                Dungeon = sqlWrapper.GetDungeon();
                if (Dungeon.GetRoomList().Count < 1)
                {
                    Dungeon = new Dungeon();
                    Dungeon.Init(100, AllItems);
                    sqlWrapper.AddDungeon(Dungeon);
                    sqlWrapper.AddItems(50, 20);
                }
            }

            //bind listner socket
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			serverSocket.Bind(new IPEndPoint(IPAddress.Parse(IP[ipIndex]), 8500));
            serverSocket.Listen(32);

            bool bQuit = false;

            //start the queue that will process all actions
            Task RequestProcess = new Task(ProcessRequestQueue);
            RequestProcess.Start();

            while (!bQuit)
            {
                Console.WriteLine("Waitting for a client" + clientID);
                Socket serverClient = serverSocket.Accept();
            
                //Start a new thread that will receieve incoming messages from client
                Thread myThread = new Thread(ReceiveClientProcess);
                myThread.IsBackground = true;
                myThread.Start(serverClient);
 
                clientID++;

                Thread.Sleep(200);
            }
        }
    }
}
