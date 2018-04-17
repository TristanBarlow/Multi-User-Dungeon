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
        public LoginScreen()
        {
            InitializeComponent();
        }

        private void LoginScreen_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void LoginClick(object sender, EventArgs e)
        {
            if (this.NameTextBox.Text.Count() > 0)
            {
                String temp = this.NameTextBox.Text;
                Form1 frm = new Form1(temp, " ");
                this.Visible = false;
                frm.ShowDialog();
                this.Close();
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
