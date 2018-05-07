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
        //reference to the form used for setting credentials
        MudClient form;

        //connection status
        public bool IsConnected = false;

        /**
         * Defualt constructor
         * @param frm reference to the mudclient 
         */
        public LoginScreen(MudClient frm)
        {
            InitializeComponent();
            form = frm;
        }

        /**
         *Start the sequence of calls to log a player in
         * winform Generated
         */
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

        /**
         * When a response come from the
         * @param s the message that came with the response
         * @param success whether or not we are now logged in
         */
        public void LoginResponse(String s, bool success)
        {
            //close the diaglo winner winnder
            if (success)
            {
                DialogResult = DialogResult.OK;
                return;
            }

            //use the method envoker thing until we get the message to appear
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

        /**
         * To be caalled when a conncetion is made with the server
         * @param s the message to display if connected
         */
        public void Connected(String s)
        {
            
            //invoker thing to make sure the mesage displays
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

        /**
         *Called from the create character form, this will start the sequence in the main 
         * form to create a character.
         * @param name name of the character to be created
         * @param password the raw password of the new character(will be hashed later)
         */
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

        /**
         *Opens the create user window. 
         */
        private void OpenCreateUser(object sender, EventArgs e)
        {
            CreateCharacter create = new CreateCharacter(this);
            this.Enabled = false;
            DialogResult result = create.ShowDialog();
            this.Enabled = true;
        }
    }
}
