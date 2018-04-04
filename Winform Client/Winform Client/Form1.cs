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
        bool spam = false;

        List<String> currentClientList = new List<String>();
        List<int> numberOfClients = new List<int>();
        int iter = 0;
        int MapMoveSpeed = 10;

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

            SetClientList(new ClientListMsg());

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
                        MemoryStream stream = new MemoryStream(buffer);
                        BinaryReader read = new BinaryReader(stream);

                        Msg m = Msg.DecodeStream(read);

                        if (m != null)
                        {
                            Console.Write("Got a message: ");
                            switch (m.mID)
                            {
                                case PublicChatMsg.ID:
                                    {
                                        PublicChatMsg publicMsg = (PublicChatMsg)m;

                                        form.AddText(publicMsg.msg);
                                    }
                                    break;

                                case PrivateChatMsg.ID:
                                    {
                                        PrivateChatMsg privateMsg = (PrivateChatMsg)m;
                                        form.AddText(privateMsg.msg);
                                    }
                                    break;

                                case ClientListMsg.ID:
                                    {
                                        ClientListMsg clientList = (ClientListMsg)m;
                                        
                                        form.SetClientList(clientList);
                                    }
                                    break;

                                case ClientNameMsg.ID:
                                    {
                                        ClientNameMsg clientName = (ClientNameMsg)m;

                                        form.SetClientName(clientName.name);
                                    }
                                    break;

                                case DungeonResponse.ID:
                                    {
                                        DungeonResponse dSponse = (DungeonResponse)m;

                                        form.AddDungeonText(dSponse.response);
                                    }
                                    break;
                                case HealthMessage.ID:
                                    {
                                        HealthMessage Msg = (HealthMessage)m;
                                        form.AddText("Player Health: " + Msg.health);
                                    }
                                    break;
                                case MapLayout.ID:
                                    {
                                        
                                        MapLayout ML = (MapLayout)m;
                                        DGD.IsInUse = true;
                                        Thread newThread = new Thread(() => DGD.MapParser(ML.mapInfo));
                                        newThread.Start();
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
                                            String[] words = PL.LocationString.Split('&');
                                            DGD.ClientNumberList.Clear();
                                            foreach (String w in words)
                                            {
                                                String[] s = w.Split(' ');
                                                if (s.Count() >= 2)
                                                {
                                                    DGD.ClientNumberList.Add(Int32.Parse(s[1]));
                                                }
                                            }
                                            DGD.DrawPlayers(PL.LocationString);
                                        }
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
                    
                }

            }
        }

        public Form1()
        {
            InitializeComponent();

            myThread = new Thread(ClientProcess);
            myThread.Start(this);


            DGD = new DungeonDraw(this.DungeonGraphic);

            Application.ApplicationExit += delegate { OnExit(); };
        }

        private delegate void AddTextDelegate(String s);

        private void AddText(String s)
        {

            if (ChatBox.InvokeRequired)
            {

                try { Invoke(new AddTextDelegate(AddText), new object[] { s }); }
                catch {}

            }
            else
            {

                try { ChatBox.AppendText(U.NewLineS(s)); }
                catch { ChatBox.Dispose(); };

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
            
            while (spam)
            {
                int rndNum = rnd.Next(0, 100);
                if (rndNum < 33)
                {
                    SendDungeonMessage("pickup cheese");
                }
                if (rndNum> 33 && rndNum < 66)
                {
                    SendNameChangeMessage(rndNum.ToString());
                 }
                else
                {
                    SendDungeonMessage("drop cheese");
                }
                    //SendNameChangeMessage(rnd.Next(0, 1000).ToString());
                Thread.Sleep(500);
            }
        }

        private delegate void SetClientNameDelegate(String s);

        private void SetClientName(String s)
        {
            if (this.InvokeRequired)
            {
                Invoke(new SetClientNameDelegate(SetClientName), new object[] {s});
            }
            else
            {
                Text = s;
            }
        }

        private delegate void SetClientListDelegate(ClientListMsg clientList);

        private void SetClientList(ClientListMsg clientList)
        {
            if (this.InvokeRequired)
            {
                Invoke(new SetClientListDelegate(SetClientList), new object[] { clientList });
            }
            else
            {
                listBox_ClientList.DataSource = null;
                currentClientList.Clear();
                currentClientList.Add("Say");
                currentClientList.Add("Dungeon");

                foreach (String s in clientList.clientList)
                {
                    currentClientList.Add(s);
                }
                listBox_ClientList.DataSource = currentClientList;             
            }
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            if (spam)
            {
                Thread stressThread = new Thread(StressTest);
                stressThread.Start();
            }
            if ( (textBox_Input.Text.Length > 0) && (clientSocket != null))
            {                
                try
                {
                    if (listBox_ClientList.SelectedIndex == 1)
                    {
                        SendDungeonMessage(textBox_Input.Text);
                    }

                    else if (listBox_ClientList.SelectedIndex == 0)
                    {
                        PublicChatMsg publicMsg = new PublicChatMsg();

                        publicMsg.msg = textBox_Input.Text;
                        MemoryStream outStream = publicMsg.WriteData();
                        clientSocket.Send(outStream.GetBuffer());                
                    }
                    else if (listBox_ClientList.SelectedIndex > 1)
                    {
                        PrivateChatMsg privateMsg = new PrivateChatMsg();

                        privateMsg.msg = textBox_Input.Text;
                        privateMsg.destination = currentClientList[listBox_ClientList.SelectedIndex];
                        MemoryStream outStream = privateMsg.WriteData();
                        clientSocket.Send(outStream.GetBuffer());                
                    }
                    
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

        private void SendAttackMessage(String Message)
        {
            if (bConnected)
            {
                AttackMessage attMsg = new AttackMessage();
                attMsg.action = Message;
                attMsg.opponent = currentClientList[listBox_ClientList.SelectedIndex];
                MemoryStream outStream = attMsg.WriteData();
                try
                {
                    clientSocket.Send(outStream.GetBuffer());
                }
                catch { }
            }
        }

        private void SendNameChangeMessage(String name)
        {
            ClientNameMsg nameMsg = new ClientNameMsg();

            nameMsg.name = name;
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
                case Keys.S:
                    DGD.ChangeScale(-1);
                    break;
                case Keys.W:
                    DGD.ChangeScale(1);
                    break;
                    
            }
        }

        private void ChangeNameClick(object sender, EventArgs e)
        {
            if ((NameBox.Text.Length > 0) && (clientSocket != null))
            {
                try
                {
                    SendNameChangeMessage(NameBox.Text);
                }
                catch (System.Exception)
                {

                }

                NameBox.Text = "";

            }
        }

        private void AttackSend(object sender, EventArgs e)
        {
            SendAttackMessage("attack");
        }

        private void DefendSend(object sender, EventArgs e)
        {
            SendAttackMessage("defend");
        }

        private void WildAttackSend(object sender, EventArgs e)
        {
            SendAttackMessage("wildattack");
        }

        private void listBox_ClientList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void textBox_ClientName_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox_Input_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void DungeonPaint(object sender, PaintEventArgs e)
        {
            DGD.ClientNumberList = numberOfClients;
            DGD.Draw();
        }

        private void DungeonGraphic_Click(object sender, EventArgs e)
        {

        }

        private void TextboxDungeon_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
