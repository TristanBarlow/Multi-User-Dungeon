using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Utilities;

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

        private void LoginClick(object sender, EventArgs e)
        {
            if (IsConnected && !U.HasBadChars(this.NameTextBox.Text) && !U.HasBadChars(this.PasswordTextBox.Text))
            {

                form.SetUserData(this.NameTextBox.Text, this.PasswordTextBox.Text);
                this.Login.Enabled = false;
                this.CreateUser.Enabled = false;
                form.RequestSalt();
            }
            else
            {
                LoginError.Text = "Bad Characters";
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
                this.Login.Enabled = true;
                this.CreateUser.Enabled = true;
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

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        public void TryCreateCharacter(String name, String password)
        {
            if (IsConnected && !U.HasBadChars(name) && !U.HasBadChars(password))
            {

                form.SetUserData(name, password);
                form.SendSalt();

            }
            else
            {
                LoginError.Text = "Error Creating User";

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            CreateCharacter create = new CreateCharacter(this);
            this.Enabled = false;
            DialogResult result = create.ShowDialog();
            this.Enabled = true;
        }
    }
}
