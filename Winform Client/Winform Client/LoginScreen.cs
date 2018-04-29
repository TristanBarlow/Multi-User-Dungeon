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

        private void LoginClick(object sender, EventArgs e)
        {
            if (this.NameTextBox.Text.Count() > 0 && this.PasswordTextBox.Text.Count() > 0 && IsConnected && !NameTextBox.Text.Contains(" "))
            {

                form.SendLoginMessage(this.NameTextBox.Text, this.PasswordTextBox.Text);

            }
            else
            {
                LoginError.Text = "Error logging in";
            }
        }

        public void LoginResponse(String s, bool success)
        {

            if (success)
            {
                DialogResult = DialogResult.OK;
                return;
            }
            MethodInvoker methodInvokerDelegate = delegate ()
            {
                LoginError.Text = s;
            };
            if (this.InvokeRequired)
            {
                this.Invoke(methodInvokerDelegate);
            }
            else
            {
                methodInvokerDelegate();
            }
        }

        public void Connected(String s)
        {
                
            MethodInvoker methodInvokerDelegate = delegate ()
            {
                ConnectionStatus.Text = s;
                IsConnected = true;
            };
            if (this.InvokeRequired)
            {
                this.Invoke(methodInvokerDelegate);
            }
            else
            {
                methodInvokerDelegate();
            }
        }

        private void LoginScreen_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (this.NameTextBox.Text.Count() > 0 && this.PasswordTextBox.Text.Count() > 0 && IsConnected && !NameTextBox.Text.Contains(" "))
            {

                form.SendCreateUserMessage(this.NameTextBox.Text, this.PasswordTextBox.Text);

            }
            else
            {
                LoginError.Text = "Error Creating User in";
            }
        }
    }
}
