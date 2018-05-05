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

        static private ConcurrentQueue<Action> RequestQueue = new ConcurrentQueue<Action>();

        static private SqlWrapper sqlWrapper;

        static private GameObjectList AllItems;

        private static String[] IP = { "127.0.0.1", "46.101.88.130", "192.168.1.101" };

        private static int ipIndex = 0;


        static void SendDungeonInfo(Player player)
        {
            MapLayout ML = new MapLayout();
            ML.mapInfo = Dungeon.GetDungeonString();
            MemoryStream outStream = ML.WriteData(player.Salt);

            try
            {
                player.socket.Send(outStream.GetBuffer());
            }
            catch
            {
                Console.Write("problem sending");
                RemoveClientByPlayer(player);
            }
        }

        static void SendDungeonResponse(Player player, String response)
        {
            DungeonResponse msg = new DungeonResponse();
            msg.response = response;
            MemoryStream outStream = msg.WriteData(player.Salt);

            try
            {
                player.socket.Send(outStream.GetBuffer());
            }
            catch (System.Exception)
            {
                RemoveClientByPlayer(player);
            }
        }

        static bool ExisitngName(String s)
        {
            foreach (Player p in clientList)
            {
                if (p.PlayerName == s)
                {
                    return true;
                }
            }
            return false;
        }

        static void RemoveClientByPlayer(Player p)
        {
            if (p != null)
            {
                sqlWrapper.RemoveActivePlayer(p.PlayerName);
                clientList.Remove(p);
            }
        }

        static void SendUpdateMessage(String str, Player p)
        {
            UpdateChat UC = new UpdateChat();
            UC.message = str;
            MemoryStream stream = UC.WriteData(p.Salt);
            p.socket.Send(stream.GetBuffer());
        }

        static void JoinedPlayers(Player player)
        {
            foreach (Player p in clientList)
            {
                if (p != player && p.roomIndex == player.roomIndex)
                {
                    SendUpdateMessage(p.PlayerName + " entered", p);
                }
            }
        }

        static void PlayerSpoken(Player player, string message)
        {
            String str = player.PlayerName;
            foreach (Player p in clientList)
            {
                if (p == player) str = "You ";
                else { str = player.PlayerName; };

                if (p.roomIndex == player.roomIndex)
                {
                    SendUpdateMessage(player.PlayerName + " Said:  " + message , p);
                }
            }
        }

        static void LeftPlayers(Player player, int room)
        {
            foreach (Player p in clientList)
            {
                if (p != player && p.roomIndex == room)
                {
                    SendUpdateMessage("Player: " + p.PlayerName + " left", p);
                }
            }
        }

        static void DungeonAction(String dungMsg, Player player)
        { 
            ActionResponse response  = Dungeon.PlayerAction(dungMsg, player, sqlWrapper);
            switch (response.ID)
            {
                case ActionID.MOVE:
                    JoinedPlayers(player);
                    SendLocations();
                    SendDungeonResponse(player, response.message);
                    break;

                case ActionID.UPDATE:
                    SendUpdateMessage(response.message, player);
                    break;

                case ActionID.SAY:
                    PlayerSpoken(player, response.message);
                    break;

                case ActionID.NORMAL:
                    if (response.message == "") return;
                    SendDungeonResponse(player, response.message);
                    break;
            }
        }

        static void AddNewPlayer(Player p)
        {
            Room r;
            if (p.roomIndex == -1)
            {
                r = Dungeon.GetRandomRoom();
            }
            else
            {
                r = Dungeon.GetRoomList()[p.roomIndex];
            }
            p.SetRoom(r);
            sqlWrapper.UpdatePlayerPos(p);
            clientList.Add(p);
        }

        static void SendSalt(Socket s, String salt)
        {
            SaltMessage SM = new SaltMessage();
            SM.message = salt;
            MemoryStream stream = SM.WriteData("This does not matter as it will not be used");
            s.Send(stream.GetBuffer());
        }

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
                p.socket.Send(outStream.GetBuffer());
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        static void ReceiveClientProcess(Object o)
        {
            bool bQuit = false;

            Socket chatClient = (Socket)o;

            bool LoggedIn = false;

            Player player = new Player(chatClient);

            while (!LoggedIn && bQuit == false)
            {
                LoggedIn = LoginSequence(ref player, ref bQuit);
            }

            if (bQuit == true) return;

            player.socket = chatClient;

            RequestQueue.Enqueue(() => AddNewPlayer(player));

            Thread.Sleep(100);

            RequestQueue.Enqueue(() => DungeonAction("look", player));

            Thread.Sleep(100);

            RequestQueue.Enqueue(() => SendDungeonInfo(player));

            Thread.Sleep(100);

            RequestQueue.Enqueue(() => SendLocations());

            /// do command

            while (bQuit == false)
            {
                try
                {
                    byte[] buffer = new byte[4096];
                    int result;

                    result = chatClient.Receive(buffer);

                    if (result > 0 )
                    {

                        Msg m = Msg.DecodeStream(buffer, player.Salt);

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
                       // do something
                    }
                }
                catch (Exception)
                {
                    bQuit = true;

                    String output = "Lost client: " + player.PlayerName;
                    Console.WriteLine(output);

                    RequestQueue.Enqueue(() =>RemoveClientByPlayer(player));
                }
            }
        }

        static bool LoginSequence(ref Player player, ref bool bQuit)
        {
            try
            {
                byte[] buffer = new byte[4096];
                int result;

                result = player.socket.Receive(buffer);

                if (result > 0)
                {
                    Msg m = Msg.DecodeStream(buffer, player.Salt);

                    switch (m.mID)
                    {
                        case SaltMessage.ID:
                            SaltMessage SM = (SaltMessage)m;
                            lock (sqlWrapper)
                            {
                                player.Salt = sqlWrapper.GetSalt(SM.message);
                                if (player.Salt != "")
                                {
                                    SendSalt(player.socket, player.Salt);
                                }
                                else
                                {
                                    SendLoginResponse(player, "Failed Bad login", false);
                                }

                            }
                            break;
                        case LoginMessage.ID:
                            {
                                LoginMessage LM = (LoginMessage)m;
                                Console.WriteLine("Login request from: " + LM.name);
                                lock (sqlWrapper)
                                {
                                    if (sqlWrapper.GetPlayerLogin(ref player, LM.name, LM.password))
                                    {
                                        Console.WriteLine("Player: " + LM.name + "Logged in");
                                        SendLoginResponse(player, "Success", true);
                                        return true;
                                    }
                                    else
                                    {
                                        Console.WriteLine("Player: " + LM.name + "Failed Login");
                                        SendLoginResponse(player, "Failed to login", false);
                                        return false;
                                    }
                                }

                            }
                        case CreateUser.ID:
                            {
                                CreateUser CM = (CreateUser)m;
                                player.SetPlayerName(CM.name);
                                player.Salt = CM.salt;
                                Console.WriteLine("Create User recieved: " + CM.name);
                                lock (sqlWrapper)
                                {
                                    if (sqlWrapper.AddPlayer(player, CM.password, CM.salt))
                                    {
                                        Console.Write(" created new player");
                                        SendLoginResponse(player, "Success", true);
                                        return true;
                                    }
                                    else
                                    {
                                        Console.Write("Failed to create player");
                                        SendLoginResponse(player, "Failed to Create player", false);
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

        static void ProcessRequestQueue()
        {
            while(true)
            {
                if (!RequestQueue.IsEmpty)
                {
                    Action action;
                    RequestQueue.TryDequeue(out action);
                    action.Invoke();
                }
             }
        }

        public static void SendLocations()
        {
                String rStr = "&";
                foreach (Player p in clientList)
                {
                        if (p.GetRoom() != null)
                        {
                            rStr += p.PlayerName + " " + p.GetRoom().RoomIndex + "&";
                        }
                    
                }
                PlayerLocations m = new PlayerLocations();
                m.LocationString = rStr;

                foreach (Player p in clientList)
                {
                    try
                    {
                        MemoryStream outStream = m.WriteData(p.Salt);
                        p.socket.Send(outStream.GetBuffer());
                    }
                    catch (System.Exception)
                    {
                        RemoveClientByPlayer(p);
                    }
                }  
        }

        static void Main(string[] args)
        {

            Console.WriteLine("Should Create new Map?");
            var response = Console.ReadLine();

            AllItems = new GameObjectList();

            sqlWrapper = new SqlWrapper(AllItems);

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

            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			serverSocket.Bind(new IPEndPoint(IPAddress.Parse(IP[ipIndex]), 8500));
            serverSocket.Listen(32);

            bool bQuit = false;

            Task RequestProcess = new Task(ProcessRequestQueue);
            RequestProcess.Start();

            while (!bQuit)
            {
                Console.WriteLine("Waitting for a client" + clientID);
                Socket serverClient = serverSocket.Accept();
            
                Thread myThread = new Thread(ReceiveClientProcess);
                myThread.IsBackground = true;
                myThread.Start(serverClient);
 
                clientID++;

                Thread.Sleep(500);
            }
        }
    }
}
