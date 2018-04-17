using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

using MessageTypes;
using Request;
using Dungeon;
using PlayerN;
using Utilities;

namespace Server
{
    class Program
    {
        static private Dictionary<Player,Socket> clientDictionary = new Dictionary<Player,Socket>();

        static private int clientID = 1;

        static private DungeonS Dungeon;

        static private RequestHandler RequestHandle;

        static void SendDungeonInfo(Socket s)
        {
            MapLayout ML = new MapLayout();
            lock (Dungeon)
            {
                ML.mapInfo = Dungeon.DungeonStr;
            }
            MemoryStream outStream = ML.WriteData();

            try { s.Send(outStream.GetBuffer()); }
            catch
            {
                Console.Write("problem sending");
                RemoveClientBySocket(s);
            }
        }

        static void SendDungeonResponse(Socket s, String response)
        {
            DungeonResponse msg = new DungeonResponse();
            msg.response = response;
            MemoryStream outStream = msg.WriteData();

            try
            {
                s.Send(outStream.GetBuffer());
            }
            catch (System.Exception)
            {
                RemoveClientBySocket(s);
            }
        }

        static Socket GetSocketFromName(Player p)
        {
            lock (clientDictionary)
            {
                return clientDictionary[p];
            }
        }

        static Player GetPlayerFromSocket(Socket s)
        {
            lock (clientDictionary)
            {
                foreach (KeyValuePair<Player, Socket> o in clientDictionary)
                {
                    if (o.Value == s)
                    {
                        return o.Key;
                    }
                }
            }

            return null;
        }

        static bool ExisitngName(String s)
        {
            foreach (KeyValuePair<Player, Socket> ps in clientDictionary)
            {
                if (ps.Key.PlayerName == s)
                {
                    return true;
                }
            }
            return false;
        }

        static void RemoveClientBySocket(Socket s)
        {
            Player p = GetPlayerFromSocket(s);

            if (p != null)
            {
                lock (clientDictionary)
                {
                    clientDictionary.Remove(p);
                }
            }
        }

        static void ProcessBuffer(byte[] buffer, Object o)
        {
            Socket chatClient = (Socket)o;

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
                            lock (clientDictionary)
                            {
                                SendDungeonResponse(chatClient, RequestHandle.PlayerAction(dungMsg.command, GetPlayerFromSocket(chatClient)));
                            }
                        }
                        break;
                    case MapLayout.ID:
                        SendDungeonInfo(chatClient);
                        break;

                    default:
                        break;
                }
            }
        }

        static void ReceiveClientProcess(Object o)
        {
            bool bQuit = false;

            bool LoggedIn = false;

            Socket chatClient = (Socket)o;
            /// do command
            
            while (bQuit == false)
            {
                try
                {
                    byte[] buffer = new byte[4096];
                    int result;

                    result = chatClient.Receive(buffer);

                    if (result > 0 && LoggedIn)
                    {
                        Task task = new Task(() => ProcessBuffer(buffer, o));
                        task.Start();
                    }
                    else
                    {
                        if (result > 0)
                        {
                            MemoryStream stream = new MemoryStream(buffer);
                            BinaryReader read = new BinaryReader(stream);

                            Msg m = Msg.DecodeStream(read);
                            if (m.mID == LoginMessage.ID)
                            {
                                Player oldPlayer = GetPlayerFromSocket(chatClient);
                                LoginMessage login = (LoginMessage)m;
                                lock (clientDictionary)
                                {

                                    if (!ExisitngName(login.name))
                                    {
                                        clientDictionary.Remove(oldPlayer);
                                        Player p = new Player(login.name, RequestHandle.GetPlayerRandomRoom());

                                        clientDictionary.Add(p, chatClient);

                                        Console.WriteLine("client receive thread for " + GetPlayerFromSocket(chatClient).PlayerName);

                                        SendDungeonInfo(chatClient);
                                        Thread.Sleep(100);

                                        SendDungeonResponse(chatClient, p.currentRoom.GetDescription());
                                        LoggedIn = true;
                                    }
                                }
                                
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    bQuit = true;

                    String output = "Lost client: " + GetPlayerFromSocket(chatClient);
                    Console.WriteLine(output);

                    RemoveClientBySocket(chatClient);
                }
            }
        }

        public static void SendLocations()
        {
            String OldString = "&&";
            while (true)
            {
                String rStr = "&";
                lock (clientDictionary)
                {
                    foreach (KeyValuePair<Player, Socket> s in clientDictionary)
                    {
                        lock (Dungeon)
                        {
                            if (s.Key.currentRoom != null)
                            {
                                rStr += s.Key.PlayerName + " " + s.Key.currentRoom.RoomIndex + "&";
                            }
                        }
                    }
                }

                rStr += "&";
                if (rStr != OldString)
                {

                    PlayerLocations m = new PlayerLocations();
                    m.LocationString = rStr;
                    MemoryStream outStream = m.WriteData();
                    OldString = rStr;
                    lock (clientDictionary)
                    {
                        foreach (KeyValuePair<Player, Socket> s in clientDictionary)
                        {
                            try
                            {
                                s.Value.Send(outStream.GetBuffer());
                            }
                            catch (System.Exception)
                            {
                                RemoveClientBySocket(s.Value);
                            }
                        }
                    }
                }
                Thread.Sleep(200);
            }
        }

        static void Main(string[] args)
        {
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            serverSocket.Bind(new IPEndPoint(IPAddress.Parse("46.101.88.130"), 8500));
            //serverSocket.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8500));
            serverSocket.Listen(32);

            bool bQuit = false;

            Console.WriteLine("Server");

            Dungeon = new DungeonS();
            Dungeon.Init(40,10);

            RequestHandle = new RequestHandler(ref Dungeon);


            {
                Thread t = new Thread(SendLocations);
                 t.Start();
            }

            while (!bQuit)
            {
                Socket serverClient = serverSocket.Accept();

                Thread myThread = new Thread(ReceiveClientProcess);
                myThread.Start(serverClient);

                String clientName = "client" + clientID;

                lock (clientDictionary)
                {
                    clientDictionary.Add(new Player(clientName), serverClient);
                }

                clientID++;

                Thread.Sleep(500);
            }
        }
    }
}
