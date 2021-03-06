﻿namespace Winform_Client
{
    partial class LoginScreen
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Login = new System.Windows.Forms.Button();
            this.NameTextBox = new System.Windows.Forms.TextBox();
            this.PasswordTextBox = new System.Windows.Forms.TextBox();
            this.ConnectionStatus = new System.Windows.Forms.TextBox();
            this.LoginError = new System.Windows.Forms.TextBox();
            this.CreateUser = new System.Windows.Forms.Button();
            this.Username = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Login
            // 
            this.Login.Location = new System.Drawing.Point(40, 170);
            this.Login.Name = "Login";
            this.Login.Size = new System.Drawing.Size(113, 34);
            this.Login.TabIndex = 0;
            this.Login.Text = "Login";
            this.Login.UseVisualStyleBackColor = true;
            this.Login.Click += new System.EventHandler(this.LoginClick);
            // 
            // NameTextBox
            // 
            this.NameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.NameTextBox.Location = new System.Drawing.Point(13, 49);
            this.NameTextBox.Name = "NameTextBox";
            this.NameTextBox.Size = new System.Drawing.Size(167, 20);
            this.NameTextBox.TabIndex = 1;
            // 
            // PasswordTextBox
            // 
            this.PasswordTextBox.Location = new System.Drawing.Point(14, 144);
            this.PasswordTextBox.Name = "PasswordTextBox";
            this.PasswordTextBox.Size = new System.Drawing.Size(178, 20);
            this.PasswordTextBox.TabIndex = 3;
            this.PasswordTextBox.UseSystemPasswordChar = true;
            // 
            // ConnectionStatus
            // 
            this.ConnectionStatus.BackColor = System.Drawing.SystemColors.Menu;
            this.ConnectionStatus.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ConnectionStatus.Cursor = System.Windows.Forms.Cursors.Default;
            this.ConnectionStatus.ForeColor = System.Drawing.Color.Red;
            this.ConnectionStatus.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ConnectionStatus.Location = new System.Drawing.Point(13, 75);
            this.ConnectionStatus.Name = "ConnectionStatus";
            this.ConnectionStatus.ReadOnly = true;
            this.ConnectionStatus.Size = new System.Drawing.Size(178, 13);
            this.ConnectionStatus.TabIndex = 5;
            this.ConnectionStatus.TabStop = false;
            this.ConnectionStatus.Text = "No Server";
            this.ConnectionStatus.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // LoginError
            // 
            this.LoginError.BackColor = System.Drawing.SystemColors.Menu;
            this.LoginError.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.LoginError.Cursor = System.Windows.Forms.Cursors.Default;
            this.LoginError.ForeColor = System.Drawing.Color.Red;
            this.LoginError.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.LoginError.Location = new System.Drawing.Point(13, 94);
            this.LoginError.Name = "LoginError";
            this.LoginError.ReadOnly = true;
            this.LoginError.Size = new System.Drawing.Size(178, 13);
            this.LoginError.TabIndex = 6;
            this.LoginError.TabStop = false;
            this.LoginError.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // CreateUser
            // 
            this.CreateUser.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CreateUser.Location = new System.Drawing.Point(58, 210);
            this.CreateUser.Name = "CreateUser";
            this.CreateUser.Size = new System.Drawing.Size(81, 23);
            this.CreateUser.TabIndex = 7;
            this.CreateUser.Text = "Create User";
            this.CreateUser.UseVisualStyleBackColor = true;
            this.CreateUser.Click += new System.EventHandler(this.OpenCreateUser);
            // 
            // Username
            // 
            this.Username.AutoSize = true;
            this.Username.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Username.Location = new System.Drawing.Point(29, 9);
            this.Username.Name = "Username";
            this.Username.Size = new System.Drawing.Size(139, 31);
            this.Username.TabIndex = 8;
            this.Username.Text = "Username";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(34, 110);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(134, 31);
            this.label1.TabIndex = 9;
            this.label1.Text = "Password";
            // 
            // LoginScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(206, 237);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Username);
            this.Controls.Add(this.CreateUser);
            this.Controls.Add(this.LoginError);
            this.Controls.Add(this.ConnectionStatus);
            this.Controls.Add(this.PasswordTextBox);
            this.Controls.Add(this.NameTextBox);
            this.Controls.Add(this.Login);
            this.Name = "LoginScreen";
            this.Text = "LoginScreen";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Login;
        private System.Windows.Forms.TextBox NameTextBox;
        private System.Windows.Forms.TextBox PasswordTextBox;
        private System.Windows.Forms.TextBox ConnectionStatus;
        private System.Windows.Forms.TextBox LoginError;
        private System.Windows.Forms.Button CreateUser;
        private System.Windows.Forms.Label Username;
        private System.Windows.Forms.Label label1;
    }
}