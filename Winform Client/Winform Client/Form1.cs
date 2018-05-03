using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Collections.Concurrent;


using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

using MessageTypes;
using Utilities;

namespace Winform_Client
{
    public partial class Form1 : Form
    {
        Socket clientSocket;
        private Thread myThread;

        bool bQuit = false;
        public bool bConnected = false;
        bool TestTheStress = false;

        private static String[] IP = { "127.0.0.1", "46.101.88.130", "192.168.1.101" };
        private static int ipIndex = 1;

        List<String> currentClientList = new List<String>();

        List<int> numberOfClients = new List<int>();

        ConcurrentQueue<Action> DrawQueue = new ConcurrentQueue<Action>();

        private delegate void AddTextDelegate(String s, bool newMessage);

        LoginScreen loginScreen;

        int MapMoveSpeed = 10;

        public String ClientName { set; get; } = " ";

        DungeonDraw DGD;

        public Form1()
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
            Form1 form = (Form1)o;
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
            Form1 form = (Form1)o;

            while (form.bConnected == true)
            {
                try
                {
                    byte[] buffer = new byte[4096];
                    int result;

                    result = form.clientSocket.Receive(buffer);

                    if (result > 0)
                    {
                        MemoryStream stream = new MemoryStream(buffer);
                        BinaryReader read = new BinaryReader(stream);

                        Msg m = Msg.DecodeStream(read);

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
                                case LoginMessage.ID:
                                    {
                                        LoginMessage LM = (LoginMessage)m;
                                        if (LM.password == "1")
                                        {
                                            loginScreen.LoginResponse(LM.name, true);
                                        }
                                        else
                                        {
                                            loginScreen.LoginResponse(LM.name, false);
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
                                default:
                                    break;
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    form.bConnected = false;
                    Console.WriteLine(U.NewLineS("Lost server!"));
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
                TextboxDungeon.AppendText(U.NewLineS(s));
            }
        }

        private void StressTest()
        {
            Random rnd = new Random();
            
            while (TestTheStress)
            {
                int rndNum = rnd.Next(0, 6);
                switch (rndNum)
                {
                    case 0:
                        SendDungeonMessage("pickup cheese");
                        break;
                    case 1:
                        SendDungeonMessage("drop cheese");
                        break;
                    case 2:
                        String m = "go north";

                        SendDungeonMessage(m);
                        break;
                    case 3:
                        String b = "go south";

                        SendDungeonMessage(b);
                        break;
                    case 4:
                        String c = "go east";
                        SendDungeonMessage(c);
                        break;
                    case 5:
                        String d = "go west";
                        SendDungeonMessage(d);
                        break;
                }
                Thread.Sleep(200);
            }
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            if (TestTheStress)
            {
                Thread stressThread = new Thread(StressTest);
                stressThread.IsBackground = true;
                stressThread.Start();
            }
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
            MemoryStream outStream = dungMsg.WriteData();
            try
            {
                clientSocket.Send(outStream.GetBuffer());
            }
            catch { }
        }

        private void RequestMapLayout(String Message)
        {
            MapLayout request = new MapLayout();
            request.mapInfo = Message;
            MemoryStream outStream = request.WriteData();
            try
            {
                clientSocket.Send(outStream.GetBuffer());
            }
            catch { }
        }

        public void SendLoginMessage(String name, String password)
        {
            LoginMessage nameMsg = new LoginMessage();
            ClientName = name;
            nameMsg.SetName(name);
            nameMsg.SetPassword(password);
            MemoryStream outStream = nameMsg.WriteData();
            try
            {
                clientSocket.Send(outStream.GetBuffer());
            }
            catch { }
        }

        public void SendCreateUserMessage(String name, String password)
        {
            CreateUser nameMsg = new CreateUser();
            ClientName = name;
            nameMsg.SetName(name);
            nameMsg.SetPassword(password);
            MemoryStream outStream = nameMsg.WriteData();
            try
            {
                clientSocket.Send(outStream.GetBuffer());
            }
            catch { }
        }

        private void ParseMap(String s)
        {
            DGD.MapParser(s);
            DGD.IsInUse = true;
            if (DGD.HasUsers) DGD.UpdateClientPositions();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (!textBox_Input.ContainsFocus)
            {
                switch (e.KeyCode)
                {
                    case Keys.Up:
                        DGD.MoveY(MapMoveSpeed);
                        break;
                    case Keys.Left:
                        DGD.MoveX(MapMoveSpeed);
                        break;
                    case Keys.Down:
                        DGD.MoveY(-MapMoveSpeed);
                        break;
                    case Keys.Right:
                        DGD.MoveX(-MapMoveSpeed);
                        break;
                    case Keys.S:
                        DGD.ChangeScale(-1);
                        break;
                    case Keys.W:
                        DGD.ChangeScale(1);
                        break;

                }
            }
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
    }
}
