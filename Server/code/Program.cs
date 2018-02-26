using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.Net.Sockets;
using System.Threading;
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
        static private Dictionary<String,Socket> clientDictionary = new Dictionary<String,Socket>();

        static private int clientID = 1;

        static private DungeonS Dungeon;

        static private RequestHandler RequestHandle;

        static private PlayerHandler PlayerHandle;

        static void SendClientName(Socket s, String clientName)
        {
            ClientNameMsg nameMsg = new ClientNameMsg();
            nameMsg.name = clientName;

            MemoryStream outStream = nameMsg.WriteData();

            s.Send(outStream.GetBuffer() );
        }

        static void ChangeClientName(Socket s, String newName)
        {

        }

        static void SendClientList()
        {
            ClientListMsg clientListMsg = new ClientListMsg();

            if (clientDictionary.Count() > 0)
            {
                lock (clientDictionary)
                {
                    foreach (KeyValuePair<String, Socket> s in clientDictionary)
                    {
                        clientListMsg.clientList.Add(s.Key);
                    }

                    MemoryStream outStream = clientListMsg.WriteData();

                    foreach (KeyValuePair<String, Socket> s in clientDictionary)
                    {
                        try
                        {
                            s.Value.Send(outStream.GetBuffer());
                        }
                        catch
                        {
                            Console.Write("problem sending");
                            RemoveClientBySocket(s.Value);
                            break;
                        }
                    }
                }
            }
        }

        static void SendChatMessage(String msg)
        {
            PublicChatMsg chatMsg = new PublicChatMsg();

            chatMsg.msg = msg;

            MemoryStream outStream = chatMsg.WriteData();

            lock (clientDictionary)
            {            
                foreach (KeyValuePair<String,Socket> s in clientDictionary)
                {
                    try
                    {
                        s.Value.Send(outStream.GetBuffer());
                    }
                    catch (System.Exception)
                    {
                    	
                    }                    
                }
            }
        }

        static void SendPrivateMessage(Socket s, String from, String msg)
        {
            PrivateChatMsg chatMsg = new PrivateChatMsg();
            chatMsg.msg = msg;
            chatMsg.destination = from;
            MemoryStream outStream = chatMsg.WriteData();

            try
            {
                s.Send(outStream.GetBuffer());
            }
            catch (System.Exception)
            {

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

            }
        }

        static Socket GetSocketFromName(String name)
        {
            lock (clientDictionary)
            {
                return clientDictionary[name];
            }
        }

        static void AttackFunction(AttackMessage attmsg, Socket chatClient)
        {

            RequestHandle.SetPlayerStance(attmsg.action, GetNameFromSocket(chatClient));

            Socket tempS = null;
            try { tempS = GetSocketFromName(attmsg.opponent); }
            catch { SendPrivateMessage(chatClient, "Server: ", "Client does not exsist, select one from the drop menu"); return; }

            if (tempS != null)
            {
                if (attmsg.opponent == GetNameFromSocket(chatClient))
                {
                    SendPrivateMessage(chatClient, "Server:", " Self harm is bad");
                }
                else
                {
                    SendPrivateMessage(chatClient, "", U.NewLineS("Server:" + RequestHandle.StartFight(GetNameFromSocket(chatClient), attmsg.opponent)) + attmsg.opponent + " health:  " + RequestHandle.GetPlayer(attmsg.opponent).GetHealth());
                    SendPrivateMessage(tempS, "Server: ", U.NewLineS(GetNameFromSocket(chatClient) + " atatcked you, Your health is now" + RequestHandle.GetPlayer(attmsg.opponent).GetHealth()));
                }
            }
            else
            {
                SendPrivateMessage(chatClient, "Server:", attmsg.opponent + " Is no longer a part of this world.");
            }
            SendPrivateMessage(chatClient, " ", U.NewLineS("Server: Stance Changed to " + RequestHandle.GetPlayer(GetNameFromSocket(chatClient)).GetStance()));
        }

        static String GetNameFromSocket(Socket s)
        {
            lock (clientDictionary)
            {
                foreach (KeyValuePair<String, Socket> o in clientDictionary)
                {
                    if (o.Value == s)
                    {
                        return o.Key;
                    }
                }
            }

            return null;
        }

        static void RemoveClientBySocket(Socket s)
        {
            string name = GetNameFromSocket(s);

            if (name != null)
            {
                lock (clientDictionary)
                {
                    clientDictionary.Remove(name);
                }
            }
        }

        static void SendHealthToClient( Socket s)
        {
            String playerName = GetNameFromSocket(s);
            Player tempPlayer;
            lock (RequestHandle)
            {
               tempPlayer = RequestHandle.GetPlayer(playerName);
            }
            HealthMessage Msg = new HealthMessage();
            Msg.health = tempPlayer.GetHealth();
            MemoryStream outStream = Msg.WriteData();
            try
            {
                s.Send(outStream.GetBuffer());
            }
            catch (System.Exception)
            {

            }
        }

        static void receiveClientProcess(Object o)
        {
            bool bQuit = false;

            Socket chatClient = (Socket)o;

            Console.WriteLine("client receive thread for " + GetNameFromSocket(chatClient));

            lock (RequestHandle)
            {
                RequestHandle.AddPlayer(GetNameFromSocket(chatClient));
                /// do command
                SendDungeonResponse(chatClient, RequestHandle.PlayerAction("look", GetNameFromSocket(chatClient)));
            }

            SendClientList();
            
            while (bQuit == false)
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

                        if (m != null && !RequestHandle.GetPlayer(GetNameFromSocket(chatClient)).isDead)
                        {

                            switch (m.mID)
                            {
                                case ClientNameMsg.ID:
                                    {
                                        ClientNameMsg clientName = (ClientNameMsg)m;

                                        String oldName = GetNameFromSocket(chatClient);

                                        lock (clientDictionary)
                                        {

                                            if (!clientDictionary.ContainsKey(clientName.name))
                                            {

                                                RequestHandle.playerNameChange(oldName, clientName.name);
                                                SendPrivateMessage(chatClient, " ", ("SERVER: Name changed"));


                                                clientDictionary.Remove(oldName);
                                                clientDictionary.Add(clientName.name, chatClient);
                                                SendClientList();
                                            }
                                            else
                                            {
                                                SendPrivateMessage(chatClient, " ", ("SERVER: oi you sneaky no-do-well, you cant have the two people with the same name"));
                                            }
                                        }


                                    }
                                    break;

                                case PublicChatMsg.ID:

                                    {
                                        PublicChatMsg publicMsg = (PublicChatMsg)m;

                                        String formattedMsg = "<" + GetNameFromSocket(chatClient) + "> " + publicMsg.msg;

                                        Console.WriteLine("public chat - " + formattedMsg);

                                        SendChatMessage(formattedMsg);
                                    }
                                    break;

                                case PrivateChatMsg.ID:
                                    {
                                        PrivateChatMsg privateMsg = (PrivateChatMsg)m;

                                        String formattedMsg = "PRIVATE <" + GetNameFromSocket(chatClient) + "> " + privateMsg.msg;

                                        Console.WriteLine("private chat - " + formattedMsg + "to " + privateMsg.destination);

                                        SendPrivateMessage(GetSocketFromName(privateMsg.destination), GetNameFromSocket(chatClient), formattedMsg);

                                        formattedMsg = "<" + GetNameFromSocket(chatClient) + "> --> <" + privateMsg.destination + "> " + privateMsg.msg;
                                        SendPrivateMessage(chatClient, "", formattedMsg);
                                    }
                                    break;

                                case DungeonCommand.ID:
                                    {
                                        DungeonCommand dungMsg = (DungeonCommand)m;

                                        SendDungeonResponse(chatClient, RequestHandle.PlayerAction(dungMsg.command, GetNameFromSocket(chatClient)));
                                    }
                                    break;

                                case AttackMessage.ID:
                                    {
                                        AttackMessage attmsg = (AttackMessage)m;
                                        AttackFunction(attmsg, chatClient);
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                        else
                        {
                            SendPrivateMessage(chatClient, "", "Server: You are dead, there is nothing you can do");
                        }
                    }                   
                }
                catch (Exception)
                {
                    bQuit = true;

                    String output = "Lost client: " + GetNameFromSocket(chatClient);
                    Console.WriteLine(output);
                    SendChatMessage(output);

                    RemoveClientBySocket(chatClient);

                    SendClientList();
                }
            }
        }

        static void Main(string[] args)
        {
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            serverSocket.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8500));
            serverSocket.Listen(32);

            bool bQuit = false;

            Console.WriteLine("Server");

            Dungeon = new DungeonS();
            Dungeon.Init();

            PlayerHandle = new PlayerHandler();

            RequestHandle = new RequestHandler(ref Dungeon, ref PlayerHandle);

            while (!bQuit)
            {
                Socket serverClient = serverSocket.Accept();

                Thread myThread = new Thread(receiveClientProcess);
                myThread.Start(serverClient);

                lock (clientDictionary)
                {
                    String clientName = "client" + clientID;
                    clientDictionary.Add(clientName, serverClient);

                    SendClientName(serverClient, clientName);
                    Thread.Sleep(500);
                    SendClientList();

                    clientID++;
                }
                
            }
        }
    }
}
