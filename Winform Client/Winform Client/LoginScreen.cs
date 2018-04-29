using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Winform_Client
{



    public partial class LoginScreen : Form
    {
        Form1 form;

        private delegate void AddTextDelegate(String s);

        public bool IsConnected = false;

        public LoginScreen(Form1 frm)
        {
            InitializeComponent();
            form = frm;
        }

        private void LoginScreen_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        public void Connected()
        {
            IsConnected = true;
            AddText("Connected");
        }

        private void LoginClick(object sender, EventArgs e)
        {
            if (this.NameTextBox.Text.Count() > 0 && IsConnected && !NameTextBox.Text.Contains(" "))
            {
                form.SendNameChangeMessage(this.NameTextBox.Text);

                DialogResult = DialogResult.OK;
            }
        }

        private void AddText(String s)
        {
            if (ConnectionStatus.InvokeRequired)
            {

                 Invoke(new AddTextDelegate(AddText), new object[] { s });

            }
            else
            {

                ConnectionStatus.Text = "Connect";

            }
        }

        private void LoginScreen_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
