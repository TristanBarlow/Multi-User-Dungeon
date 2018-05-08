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
using System.Text;

namespace Winform_Client
{
    public partial class MudClient : Form
    {
        Socket clientSocket;
        private Thread myThread;

        bool bQuit = false;
        public bool bConnected = false;

        private static String[] IP = { "127.0.0.1", "46.101.88.130", "192.168.1.101" };
        private static int ipIndex = 1;

        List<String> currentClientList = new List<String>();

        List<int> numberOfClients = new List<int>();

        ConcurrentQueue<Action> DrawQueue = new ConcurrentQueue<Action>();

        private delegate void AddTextDelegate(String s, bool newMessage);

        LoginScreen loginScreen;

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

        private void ClientProcess(Object o)

        {
            MudClient form = (MudClient)o;
            Thread receiveThread;
            Thread.Sleep(1000);
            while ((form.bConnected == false) && (form.bQuit == false))
            {
                try
                {
                    if (bConnected == false)
                    {
                        form.clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        form.clientSocket.Connect(new IPEndPoint(IPAddress.Parse(IP[ipIndex]), 8500));
                        form.bConnected = true;
                        form.loginScreen.Connected("Connected");
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
            Application.Restart();
            Environment.Exit(0);
        }

        private void ClientReceive(Object o)
        {
            MudClient form = (MudClient)o;

            while (form.bConnected == true)
            {
                try
                {
                    byte[] buffer = new byte[4096];
                    int result;

                    result = form.clientSocket.Receive(buffer);

                    if (result > 0)
                    {

                        Msg m = Msg.DecodeStream(buffer,Salt, ShouldDecrypt);

                        if (m != null)
                        {
                            Console.Write("Got a message: ");
                            switch (m.mID)
                            {
                                case DungeonResponse.ID:
                                    {
                                        DungeonResponse dSponse = (DungeonResponse)m;

                                        form.AddDungeonText(dSponse.response, true);
                                    }
                                    break;
                                case MapLayout.ID:
                                    {
                                        MapLayout ML = (MapLayout)m;
                                        DrawQueue.Enqueue(() => ParseMap(ML.mapInfo));

                                        break;
                                    }
                                case LoginResponse.ID:
                                    {
                                        
                                        LoginResponse LM = (LoginResponse)m;
                                        if (LM.loggedIn == "1")
                                        {
                                            loginScreen.LoginResponse(LM.message, true);
                                            ShouldDecrypt = true;
                                        }
                                        else
                                        {
                                            loginScreen.LoginResponse(LM.message, false);
                                            ShouldDecrypt = false;
                                        }
                                    }
                                    break;
                                case PlayerLocations.ID:
                                    {
                                        PlayerLocations PL = (PlayerLocations)m;
                                        DrawQueue.Enqueue(() => UpdatePlayerLocations(PL.LocationString));
                                    }
                                    break;
                                case UpdateChat.ID:
                                    {
                                        UpdateChat UC = (UpdateChat)m;
                                        form.AddDungeonText(UC.message, false);
                                    }
                                    break;
                                case SaltSend.ID:
                                    {
                                        SaltSend SS = (SaltSend)m;
                                        Salt = SS.salt;
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
                    form.bConnected = false;
                    Console.WriteLine(U.NL("Lost server!") + ex);
                    Application.Restart();
                    Environment.Exit(0);
                    
                }

            }

        }

        private void UpdatePlayerLocations(String s)
        {
            if (DGD.IsInUse == false)
            {
                RequestMapLayout("r");
            }
            else
            {

                lock (DGD)
                {
                    DGD.HasUsers = true;
                    DGD.DrawClients(s, ClientName);
                }
            }
        }

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

        private void buttonSend_Click(object sender, EventArgs e)
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

        private void OnExit()
        {
            if (myThread != null)
            {
                myThread.Abort();
            }
            bQuit = true;
            bConnected = false;
        }

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

        public void SetUserData(String name, String password)
        {
            ClientName = name;
            ClientPassword = password;
        }

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

        private void ParseMap(String s)
        {
            DGD.MapParser(s);
            DGD.IsInUse = true;
            if (DGD.HasUsers) DGD.UpdateClientPositions();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            myThread = new Thread(ClientProcess);
            myThread.IsBackground = true;
            myThread.Start(this);
            DialogResult result;
            loginScreen = new LoginScreen(this);
            result = loginScreen.ShowDialog();

            Task drawTask = new Task(ProcessDrawQueue);
            drawTask.Start();

            if (result != DialogResult.OK)
            {
                bQuit = true;
                OnExit();
                Application.Exit();
            }

        }

        private void DrawDungeon()
        {
            DGD.ClientNumberList = numberOfClients;
            DGD.Draw();
        }

        private void DungeonPaint(object sender, PaintEventArgs e)
        {
            DrawQueue.Enqueue(() => DrawDungeon());
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            OnExit();
            Application.Exit();
        }

        private void Zoomin_Click(object sender, EventArgs e)
        {
            DGD.ChangeScale(1);
        }

        private void Zoomout_Click(object sender, EventArgs e)
        {
            DGD.ChangeScale(-1);
        }
    }
}
