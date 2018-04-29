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

        static private SqlWrapper sqlWrapper;


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
                clientList.Remove(p);
            }
        }

        static void DungeonAction(String dungMsg, Player player)
        {
            int RoomNum = 1; 
            String dungeonResponse  = RequestHandle.PlayerAction(dungMsg, player);

            if (RoomNum != player.currentRoom.RoomIndex)
            {
                SendLocations();
            }

            SendDungeonResponse(player, dungeonResponse);
        }

        static void AddNewPlayer(Player p)
        {
            clientList.Add(p);
        }


        static void ReceiveClientProcess(Object o)
        {
            bool bQuit = false;

            Socket chatClient = (Socket)o;

            bool LoggedIn = false;

            String ClientName = " ";

            Player player = null;

            while (!LoggedIn && bQuit == false)
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
                            player = new Player(LM.name, chatClient, RequestHandle.GetPlayerRandomRoom());
                            Console.WriteLine("login recieved" + LM.name);
                            lock (sqlWrapper)
                            {
                                if (sqlWrapper.AddPlayer(player, LM.password))
                                {
                                    Console.Write(" created new player");
                                    LoggedIn = true;
                                }
                                else
                                {
                                    Console.Write("Failed to create player");
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error loggin in: " + e);
                    bQuit = true;
                    return;
                }
            }

            RequestQueue.Enqueue(() => AddNewPlayer(player));

            RequestQueue.Enqueue(() => DungeonAction("look", player));

            RequestQueue.Enqueue(() => SendDungeonInfo(player));

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
                        if (p.currentRoom != null)
                        {
                            rStr += p.PlayerName + " " + p.currentRoom.RoomIndex + "&";
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

            //serverSocket.Bind(new IPEndPoint(IPAddress.Parse("46.101.88.130"), 8500));
            serverSocket.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8500));
			//serverSocket.Bind(new IPEndPoint(IPAddress.Parse("192.168.1.153"), 8500));
            serverSocket.Listen(32);

            bool bQuit = false;

            Console.WriteLine("Should Create new Map?");
            var response = Console.ReadLine();

            sqlWrapper = new SqlWrapper();

            if (response.ToLower() == "yes")
            {
                Dungeon = new Dungeon();
                Dungeon.Init(40, 10);
                sqlWrapper.WriteDungeon(Dungeon);
            }
            else
            {
                Dungeon = sqlWrapper.GetDungeon();
                if (Dungeon.GetRoomList().Count < 1)
                {
                    Dungeon = new Dungeon();
                    Dungeon.Init(40, 10);
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
                myThread.Start(serverClient);
 
                clientID++;

                Thread.Sleep(500);
            }
        }
    }
}
