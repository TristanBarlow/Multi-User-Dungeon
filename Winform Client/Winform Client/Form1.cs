using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Collections.Concurrent;


using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using Server;

using MessageTypes;
using Utilities;

namespace Winform_Client
{
    public partial class MudClient : Form
    {
        //socket the server conncection will be bound to
        Socket clientSocket;

        //the thread responsable for recieving server response
        private Thread recieveThread;

        bool bQuit = false;
        public bool bConnected = false;

        //Ips that I needed 0 for local, 1 for server, 2 for VB linux(changes though)
        private static String[] IP = { "127.0.0.1", "46.101.88.130", "192.168.1.101" };
        private static int ipIndex = 0;

        //The queue responsable for all draw commands
        ConcurrentQueue<Action> DrawQueue = new ConcurrentQueue<Action>();

        //add text delegate believe this was gareths code.
        private delegate void AddTextDelegate(String s, bool newMessage);

        //Loginscreen form.
        LoginScreen loginScreen;

        //client name and password. Used for the inbetween time of sending the salt/ requesting the salt
        //And waiting for the response
        public String ClientName { set; get; } = " ";
        public String ClientPassword { set; get; } = " ";
        public String Salt { set; get; } = "";

        public bool ShouldDecrypt = false;

        DungeonDraw DGD;

        public MudClient()
        {
            InitializeComponent();

            DGD = new DungeonDraw(this.DungeonGraphic);

            Application.ApplicationExit += delegate { OnExit(); };
        }

        /**
         *Process the draw queue. 
         */
        private void ProcessDrawQueue()
        {
            while (true)
            {
                if (!DrawQueue.IsEmpty)
                {
                    Action action;
                    DrawQueue.TryDequeue(out action);
                    action.Invoke();
                }
            }
        }

        /**
         * Thread responsbale for openeing and mainiting the connection to the server
         * @param o form object.
         */
        private void ClientProcess(Object o)

        {
            MudClient form = (MudClient)o;
            Thread receiveThread;

            //Wait for the server to form to load fully.
            Thread.Sleep(1500);

            while ((form.bConnected == false) && (form.bQuit == false))
            {
                try
                {
                    if (bConnected == false)
                    {
                        form.clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        form.clientSocket.Connect(new IPEndPoint(IPAddress.Parse(IP[ipIndex]), 8500));
                        form.bConnected = true;

                        //send message to login screen that we're connected
                        form.loginScreen.Connected("Connected");

                        //start recieve thread
                        receiveThread = new Thread(ClientReceive);
                        receiveThread.IsBackground = true;
                        receiveThread.Start(o);

                    }
                    while ((form.bQuit == false) && (form.bConnected == true))
                    {
                        if (form.IsDisposed == true)
                        {
                            form.bQuit = true;
                            form.clientSocket.Close();
                        }

                    }
                }
                catch (System.Exception)
                {
                }
                Thread.Sleep(500);
            }
            //If Sever is randomly lost, restart.
            Application.Restart();
            Environment.Exit(0);
        }

        /**
         *Thread responsible for receiving all incoming data from the server
         * @param o form object reference
         */
        private void ClientReceive(Object o)
        {
            MudClient form = (MudClient)o;

            while (form.bConnected == true)
            {
                try
                {
                    byte[] buffer = new byte[4096];
                    int result;

                    //listen for buffer
                    result = form.clientSocket.Receive(buffer);

                    if (result > 0)
                    {
                        //decode stream
                        Msg m = Msg.DecodeStream(buffer,Salt, ShouldDecrypt);

                        //Get message type and do approriate actiobn
                        if (m != null)
                        {
                            Console.Write("Got a message: ");
                            switch (m.mID)
                            {
                                case DungeonResponse.ID:
                                    {
                                        DungeonResponse dr = (DungeonResponse)m;

                                        form.AddDungeonText(dr.response, true);
                                    }
                                    break;
                                case MapLayout.ID:
                                    {
                                        MapLayout ml = (MapLayout)m;
                                        DrawQueue.Enqueue(() => ParseMap(ml.mapInfo));

                                        break;
                                    }
                                case LoginResponse.ID:
                                    {
                                        
                                        LoginResponse lm = (LoginResponse)m;
                                        if (lm.loggedIn == "1")
                                        {
                                            //Login was successful start decrypting and start main game
                                            loginScreen.LoginResponse(lm.message, true);
                                            ShouldDecrypt = true;
                                        }
                                        else
                                        {
                                            //login failed, show failed message
                                            loginScreen.LoginResponse(lm.message, false);
                                            ShouldDecrypt = false;
                                        }
                                    }
                                    break;
                                case PlayerLocations.ID:
                                    {
                                        PlayerLocations pl = (PlayerLocations)m;
                                        DrawQueue.Enqueue(() => UpdatePlayerLocations(pl.LocationString));
                                    }
                                    break;
                                case UpdateChat.ID:
                                    {
                                        UpdateChat uc = (UpdateChat)m;
                                        form.AddDungeonText(uc.message, false);
                                    }
                                    break;
                                case SaltSend.ID:
                                    {
                                        SaltSend ss = (SaltSend)m;
                                        Salt = ss.salt;
                                        SendLoginMessage();
                                        break;
                                    }
                                case SaltRequest.ID:
                                    {
                                        SaltRequest sr = (SaltRequest)m;
                                        SendCreateUserMessage();
                                        break;
                                    }
                                default:
                                    break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    //If a discconect happens restart the service
                    form.bConnected = false;
                    Console.WriteLine(U.NL("Lost server!") + ex);
                    Application.Restart();
                    Environment.Exit(0);
                    
                }

            }

        }

        /**
         * Proces an update player locations message
         * @param s the string which represents the player locations
         */
        private void UpdatePlayerLocations(String s)
        {
            if (DGD.IsInUse == false)
            {
                //if no map try and get one, DO NOT DRAW Players
                RequestMapLayout("r");
            }
            else
            {
                DGD.AddClientsDraw(s, ClientName);
            }
        }

        /**
         *Adds text to the dungoen texbox, If its a new message it will clear the old then add
         * @param s the message to be added to textbox
         * @param newMessage where or not it is new.
         */
        private void AddDungeonText(String s, bool newMessage)
        {
            if (TextboxDungeon.InvokeRequired)
            {
                Invoke(new AddTextDelegate(AddDungeonText), new object[] { s, newMessage });
            }
            else
            {
                if(newMessage)TextboxDungeon.Clear();
                TextboxDungeon.AppendText(U.NL(s));
            }
        }

        /**
         * Sends the message to the server
         *Winform Generated 
         */
        private void SendClicked(object sender, EventArgs e)
        {
            if ( (textBox_Input.Text.Length > 0) && (clientSocket != null))
            {                
                try
                {
                     SendDungeonMessage(textBox_Input.Text);
                    
                }
                catch (System.Exception)
                {
                }

                textBox_Input.Text = "";
            }
        }

        /**
         *Exit application function 
         */
        private void OnExit()
        {
            if (recieveThread != null)
            {
                recieveThread.Abort();
            }
            bQuit = true;
            bConnected = false;
        }

        /**
         * Send the dungeon message through the socket to the server
         * @param Message the message to send
         */
        private void SendDungeonMessage(String Message)
        {
            DungeonCommand dungMsg = new DungeonCommand();
            dungMsg.command = Message;
            MemoryStream outStream = dungMsg.WriteData(Salt);
            try
            {
                clientSocket.Send(outStream.ToArray());
            }
            catch { }
        }

        /**
         * Send the map layout request message through the socket to the server
         * @param Message the message to send
         */
        private void RequestMapLayout(String Message)
        {
            MapLayout request = new MapLayout();
            request.mapInfo = Message;
            MemoryStream outStream = request.WriteData(Salt);
            try
            {
                clientSocket.Send(outStream.GetBuffer());
            }
            catch { }
        }

        /**
         * Send the salt request message through the socket to the server
         */
        public void RequestSalt()
        {
            SaltRequest sm = new SaltRequest();
            sm.message = ClientName;
            MemoryStream outStream = sm.WriteData("");
            try
            {
                clientSocket.Send(outStream.GetBuffer());
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error sending message: " + ex);
            }
        }

        /**
         * Send the salt through the socket to the server
         */
        public void SendSalt()
        {
            Salt = Encryption.GetSalt();

            SaltSend ss = new SaltSend();
            ss.salt = Salt;
            MemoryStream outStream = ss.WriteData("");
            try
            {
                clientSocket.Send(outStream.ToArray());
            }
            catch { }
        }

        /**
         *Sets the client name and password ready for it to be sent to the server
         *@param name the name of the client
         * @param password the password of the client. 
         */
        public void SetUserData(String name, String password)
        {
            ClientName = name;
            ClientPassword = password;
        }

        /**
         *Send the login message through the socket to the server 
         */
        public void SendLoginMessage()
        {
            LoginMessage nameMsg = new LoginMessage();
            nameMsg.SetName(ClientName);
            nameMsg.SetPassword(ClientPassword, Salt);
            MemoryStream outStream = nameMsg.WriteData(Salt);
            try
            {
                clientSocket.Send(outStream.ToArray());
                ClientPassword = "";
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending message: " + ex);
            }
        }

        /**
         *Send the create user message through the socket to the server 
         */
        public void SendCreateUserMessage()
        {
            CreateUser nameMsg = new CreateUser();
            nameMsg.SetName(ClientName);
            nameMsg.SetPassword(ClientPassword, Salt );
            MemoryStream outStream = nameMsg.WriteData(Salt);
            try
            {
                clientSocket.Send(outStream.ToArray());
            }
            catch { }
        }

        /**
         *Adds the map layout into the dungeon draw class
         * @param s the string holdign the above information
         */
        private void ParseMap(String s)
        {
            DGD.MapParser(s);
            DGD.IsInUse = true;
        }

        /**
         *Winform Generated
         */
        private void Form1_Load(object sender, EventArgs e)
        {
            //start recieve thread
            recieveThread = new Thread(ClientProcess);
            recieveThread.IsBackground = true;
            recieveThread.Start(this);

            //show login screen
            DialogResult result;
            loginScreen = new LoginScreen(this);
            result = loginScreen.ShowDialog();

            //start draw processing queue
            Task drawTask = new Task(ProcessDrawQueue);
            drawTask.Start();
            
            //check for good login result
            if (result != DialogResult.OK)
            {
                bQuit = true;
                OnExit();
                Application.Exit();
            }

        }
        
        /**
         *Draw dungeon call 
         */
        private void DrawDungeon()
        {
            DGD.Draw();
        }

        /**
         *Winform Generated. Called when the playe resizes the windows 
         */
        private void DungeonPaint(object sender, PaintEventArgs e)
        {
            DrawQueue.Enqueue(() => DrawDungeon());
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            OnExit();
            Application.Exit();
        }

        /**
         * Zoomins in
         *Winform Generated
         */
        private void Zoomin_Click(object sender, EventArgs e)
        {
            DGD.ChangeScale(1);
        }

        /**
         * Zoomins out
         *Winform Generated
         */
        private void Zoomout_Click(object sender, EventArgs e)
        {
            DGD.ChangeScale(-1);
        }
    }
}
