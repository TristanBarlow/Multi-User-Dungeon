using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;


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
        bool bConnected = false;
        bool TestTheStress = false;

        List<String> currentClientList = new List<String>();

        List<int> numberOfClients = new List<int>();

        int MapMoveSpeed = 10;

        public String ClientName { set; get; } = " ";

        DungeonDraw DGD;

        private void ClientProcess(Object o)

        {
            Form1 form = (Form1)o;
            Thread receiveThread;
            while ((form.bConnected == false) && (form.bQuit == false))
            {
                try
                {
                    if (bConnected == false)
                    {
                        form.clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        form.clientSocket.Connect(new IPEndPoint(IPAddress.Parse("46.101.88.130"), 8500));
                        //form.clientSocket.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8500));
                        form.bConnected = true;
                        receiveThread = new Thread(ClientReceive);
                        receiveThread.Start(o);
                        Thread.Sleep(100);
                        SendNameChangeMessage("");

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
                    try { form.AddText(U.NewLineS("No server!")); }
                    catch { }
                }
                Thread.Sleep(500);
            }
        }

        private void ClientReceive(Object o)
        {
            Form1 form = (Form1)o;

            form.AddText("Connected to server");



            while (form.bConnected == true)
            {
                try
                {
                    byte[] buffer = new byte[4096];
                    int result;

                    result = form.clientSocket.Receive(buffer);

                    if (result > 0)
                    {
                        Task task = new Task(() => ProcessBuffer(buffer, form));
                        task.Start();
                    }
                }
                catch (Exception)
                {
                    form.bConnected = false;
                    Console.WriteLine(U.NewLineS("Lost server!"));

                }

            }
        }

        private void ProcessBuffer(byte[] buffer, Object o)
        {

            Form1 form = (Form1)o;

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

                            form.AddDungeonText(dSponse.response);
                        }
                        break;
                    case MapLayout.ID:
                        {
                            MapLayout ML = (MapLayout)m;
                            lock (DGD)
                            {
                                DGD.MapParser(ML.mapInfo);
                                DGD.IsInUse = true;
                                if (DGD.HasUsers) DGD.UpdateClientPositions();
                            }
                        }
                        break;
                    case PlayerLocations.ID:
                        {
                            if (DGD.IsInUse == false)
                            {
                                RequestMapLayout("r");
                            }
                            else
                            {
                                PlayerLocations PL = (PlayerLocations)m;
                                lock (DGD)
                                {
                                    DGD.HasUsers = true;
                                    DGD.DrawClients(PL.LocationString, ClientName);
                                }
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        public Form1(String Name, String Password)
        {
            InitializeComponent();

            myThread = new Thread(ClientProcess);
            myThread.Start(this);

            ClientName = Name;

            DGD = new DungeonDraw(this.DungeonGraphic);

            Application.ApplicationExit += delegate { OnExit(); };
        }

        private delegate void AddTextDelegate(String s);

        private void AddText(String s)
        {

            if (ChatBox.InvokeRequired)
            {

                try { Invoke(new AddTextDelegate(AddText), new object[] { s }); }
                catch { Application.Exit(); }

            }
            else
            {

                ChatBox.AppendText(U.NewLineS(s));

            }
        }

        private void AddDungeonText(String s)
        {
            if (TextboxDungeon.InvokeRequired)
            {
                Invoke(new AddTextDelegate(AddDungeonText), new object[] { s });
            }
            else
            {
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

        public void SendNameChangeMessage(String name)
        {
            LoginMessage nameMsg = new LoginMessage();
            nameMsg.name = ClientName;
            nameMsg.password = " ";
            MemoryStream outStream = nameMsg.WriteData();
            try
            {
                clientSocket.Send(outStream.GetBuffer());
            }
            catch { }
        }

        private void ButtonNorth_Click(object sender, EventArgs e)
        {
            String m = "go north";

            SendDungeonMessage(m);
        }

        private void ButtonEast_Click(object sender, EventArgs e)
        {
            String m = "go east";
            SendDungeonMessage(m);
        }

        private void ButtonSouth_Click(object sender, EventArgs e)
        {
            String m = "go south";
            SendDungeonMessage(m);
        }

        private void ButtonWest_Click(object sender, EventArgs e)
        {
            String m = "go west";

            SendDungeonMessage(m);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            String m;
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
                //case Keys.S:
                //    DGD.ChangeScale(-1);
                //    break;
                //case Keys.W:
                //    DGD.ChangeScale(1);
                //    break;
                    
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void DungeonPaint(object sender, PaintEventArgs e)
        {
            DGD.ClientNumberList = numberOfClients;
            DGD.Draw();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            OnExit();
            Environment.Exit(Environment.ExitCode);
            Application.Exit();
        }
    }
}
