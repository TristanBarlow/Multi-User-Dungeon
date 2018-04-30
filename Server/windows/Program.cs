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
using Request;
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

        static private RequestHandler RequestHandle;

        static private ConcurrentQueue<Action> RequestQueue = new ConcurrentQueue<Action>();

        static private ConcurrentQueue<Action> LoginQueue = new ConcurrentQueue<Action>();

        static private SqlWrapper sqlWrapper;

        private static String[] IP = { "127.0.0.1", "46.101.88.130", "192.168.1.153" };

        private static int ipIndex = 0;


        static void SendDungeonInfo(Player player)
        {
            MapLayout ML = new MapLayout();
            ML.mapInfo = Dungeon.GetDungeonString();
            MemoryStream outStream = ML.WriteData();

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
            MemoryStream outStream = msg.WriteData();

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
                lock (sqlWrapper)
                {
                    sqlWrapper.WritePlayer(p);
                }
                clientList.Remove(p);
            }
        }

        static void DungeonAction(String dungMsg, Player player)
        { 
            String dungeonResponse  = RequestHandle.PlayerAction(dungMsg, player);
            if (player.GetHasMoved())
            {
                SendLocations();
            }
            SendDungeonResponse(player, dungeonResponse);
        }

        static void AddNewPlayer(Player p)
        {
            if (p.roomIndex == -1)
            {
                p.SetRoom(Dungeon.GetRandomRoom());
            }
            else
            {
                p.SetRoom(Dungeon.GetRoomList()[p.roomIndex]);
            }
            clientList.Add(p);
        }

        static bool LoginSequence(Socket chatClient, ref Player player, ref bool bQuit)
        {
            try
            {
                byte[] buffer = new byte[4096];
                int result;

                result = chatClient.Receive(buffer);

                if (result > 0)
                {
                    MemoryStream stream = new MemoryStream(buffer);
                    BinaryReader read = new BinaryReader(stream);

                    Msg m = Msg.DecodeStream(read);

                    if (m.mID == LoginMessage.ID)
                    {
                        LoginMessage LM = (LoginMessage)m;
                        player = new Player(chatClient);
                        Console.WriteLine("Login request from: " + LM.name);
                        lock (sqlWrapper)
                        {
                            if (sqlWrapper.GetPlayerLogin(ref player, LM.name, LM.password))
                            {
                                Console.WriteLine("Player: " + LM.name + "Logged in");
                                SendLoginResponse(chatClient, "Success", true);
                                return true;
                            }
                            else
                            {
                                Console.WriteLine("Player: " + LM.name + "Failed Login");
                                SendLoginResponse(chatClient, "Failed to login", false);
                                return false;
                            }
                        }

                    }
                    else if (m.mID == CreateUser.ID)
                    {
                        CreateUser CM = (CreateUser)m;
                        player = new Player(CM.name, chatClient);
                        Console.WriteLine("Create User recieved: " + CM.name);
                        lock (sqlWrapper)
                        {
                            if (sqlWrapper.AddPlayer(player, CM.password))
                            {
                                Console.Write(" created new player");
                                SendLoginResponse(chatClient, "Success", true);
                                return true;
                            }
                            else
                            {
                                Console.Write("Failed to create player");
                                SendLoginResponse(chatClient, "Failed to Create player", false);
                                return false;
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

        static bool SendLoginResponse(Socket socket, String error, bool success)
        {
            LoginMessage response = new LoginMessage();
            response.name = error;
            if (success)
            {
                response.password = "1";
            }
            else
            {
                response.password = "0";
            }
            MemoryStream outStream = response.WriteData();

            try
            {
                socket.Send(outStream.GetBuffer());
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

            Player player = null;

            while (!LoggedIn && bQuit == false)
            {
                LoggedIn = LoginSequence(chatClient,ref player, ref bQuit);
            }

            if (bQuit == true) return;

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
                        MemoryStream stream = new MemoryStream(buffer);
                        BinaryReader read = new BinaryReader(stream);

                        Msg m = Msg.DecodeStream(read);

                        if (m != null)
                        {

                            switch (m.mID)
                            {
                                case DungeonCommand.ID:
                                    {
                                        DungeonCommand dungMsg = (DungeonCommand)m;
                                        RequestQueue.Enqueue(() => DungeonAction(dungMsg.command, player));
                                        Console.Write("message recieved");
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
                MemoryStream outStream = m.WriteData();
                foreach (Player p in clientList)
                {
                    try
                    {
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
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			serverSocket.Bind(new IPEndPoint(IPAddress.Parse(IP[ipIndex]), 8500));
            serverSocket.Listen(32);

            bool bQuit = false;

            Console.WriteLine("Should Create new Map?");
            var response = Console.ReadLine();

            sqlWrapper = new SqlWrapper();

            if (response.ToLower() == "yes")
            {
                Dungeon = new Dungeon();
                Dungeon.Init(100);
                sqlWrapper.WriteDungeon(Dungeon);
            }
            else
            {
                Dungeon = sqlWrapper.GetDungeon();
                if (Dungeon.GetRoomList().Count < 1)
                {
                    Dungeon = new Dungeon();
                    Dungeon.Init(100);
                    sqlWrapper.WriteDungeon(Dungeon);
                }
            }

            RequestHandle = new RequestHandler(ref Dungeon);

            Task RequestProcess = new Task(ProcessRequestQueue);
            RequestProcess.Start();

            while (!bQuit)
            {
                Console.WriteLine("Waitting for a client");
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
