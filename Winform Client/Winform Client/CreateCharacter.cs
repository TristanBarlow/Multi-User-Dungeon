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
        LoginScreen LS;
        public CreateCharacter(LoginScreen ls)
        {
            InitializeComponent();
            LS = ls;
        }

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

        private void NameTextBox_TextChanged(object sender, EventArgs e)
        {
            CheckForValidLogin();
        }

        private void Create_Click(object sender, EventArgs e)
        {
            LS.TryCreateCharacter(NameTextBox.Text, PasswordTextBox.Text);
            DialogResult = DialogResult.OK;
        }
    }
}
