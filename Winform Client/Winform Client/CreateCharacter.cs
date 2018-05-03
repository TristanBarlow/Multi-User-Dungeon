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
            if (NameTextBox.Text.Count() > 0 && !NameTextBox.Text.Contains(" "))
            {
                if (PasswordTextBox.Text.Count() > 0 && PasswordTextBox.Text == RetypePassword.Text)
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
