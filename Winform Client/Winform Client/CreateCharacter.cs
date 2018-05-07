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
    public partial class CreateCharacter : Form
    {
        //used to call the create char function
        LoginScreen LS;

        /**
         *constructor
         * @param ls Login screen ref
         */
        public CreateCharacter(LoginScreen ls)
        {
            InitializeComponent();
            LS = ls;
        }

        /**
         *To be called everytime text changes, if no bad characters , passwords match then enable button  
         */
        public void CheckForValidLogin()
        {
            if (!U.HasBadChars(NameTextBox.Text) && !U.HasBadChars(PasswordTextBox.Text))
            {
                if (PasswordTextBox.Text == RetypePassword.Text)
                {
                    Create.Enabled = true;
                    return;
                }
            }
            Create.Enabled = false;
        }

        /**
         * Winform generated 
         */
        private void TextChanged(object sender, EventArgs e)
        {
            CheckForValidLogin();
        }


        /**
         * Winform generated 
         */
        private void Create_Click(object sender, EventArgs e)
        {
            LS.TryCreateCharacter(NameTextBox.Text, PasswordTextBox.Text);
            DialogResult = DialogResult.OK;
        }
    }
}
