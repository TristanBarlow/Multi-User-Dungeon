namespace Winform_Client
{
    partial class Form1
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
            this.buttonSend = new System.Windows.Forms.Button();
            this.textBox_Input = new System.Windows.Forms.TextBox();
            this.listBox_ClientList = new System.Windows.Forms.ListBox();
            this.NameBox = new System.Windows.Forms.TextBox();
            this.TextboxDungeon = new System.Windows.Forms.TextBox();
            this.ButtonNorth = new System.Windows.Forms.Button();
            this.ButtonEast = new System.Windows.Forms.Button();
            this.ButtonSouth = new System.Windows.Forms.Button();
            this.ButtonWest = new System.Windows.Forms.Button();
            this.ChangeName = new System.Windows.Forms.Button();
            this.AttackButton = new System.Windows.Forms.Button();
            this.DefendButton = new System.Windows.Forms.Button();
            this.WildAttackButton = new System.Windows.Forms.Button();
            this.DungeonGraphic = new System.Windows.Forms.PictureBox();
            this.ChatBox = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.DungeonGraphic)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonSend
            // 
            this.buttonSend.BackColor = System.Drawing.Color.Black;
            this.buttonSend.ForeColor = System.Drawing.Color.Lime;
            this.buttonSend.Location = new System.Drawing.Point(968, 387);
            this.buttonSend.Name = "buttonSend";
            this.buttonSend.Size = new System.Drawing.Size(90, 21);
            this.buttonSend.TabIndex = 0;
            this.buttonSend.Text = "Send";
            this.buttonSend.UseVisualStyleBackColor = false;
            this.buttonSend.Click += new System.EventHandler(this.buttonSend_Click);
            // 
            // textBox_Input
            // 
            this.textBox_Input.BackColor = System.Drawing.Color.Black;
            this.textBox_Input.ForeColor = System.Drawing.Color.White;
            this.textBox_Input.Location = new System.Drawing.Point(578, 388);
            this.textBox_Input.Name = "textBox_Input";
            this.textBox_Input.Size = new System.Drawing.Size(384, 20);
            this.textBox_Input.TabIndex = 2;
            this.textBox_Input.TextChanged += new System.EventHandler(this.textBox_Input_TextChanged);
            // 
            // listBox_ClientList
            // 
            this.listBox_ClientList.BackColor = System.Drawing.Color.Black;
            this.listBox_ClientList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listBox_ClientList.ForeColor = System.Drawing.Color.White;
            this.listBox_ClientList.FormattingEnabled = true;
            this.listBox_ClientList.Location = new System.Drawing.Point(968, 3);
            this.listBox_ClientList.Name = "listBox_ClientList";
            this.listBox_ClientList.Size = new System.Drawing.Size(89, 312);
            this.listBox_ClientList.TabIndex = 5;
            this.listBox_ClientList.SelectedIndexChanged += new System.EventHandler(this.listBox_ClientList_SelectedIndexChanged);
            // 
            // NameBox
            // 
            this.NameBox.BackColor = System.Drawing.Color.Black;
            this.NameBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.NameBox.Location = new System.Drawing.Point(968, 332);
            this.NameBox.Name = "NameBox";
            this.NameBox.Size = new System.Drawing.Size(89, 13);
            this.NameBox.TabIndex = 6;
            this.NameBox.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // TextboxDungeon
            // 
            this.TextboxDungeon.BackColor = System.Drawing.SystemColors.InfoText;
            this.TextboxDungeon.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TextboxDungeon.ForeColor = System.Drawing.Color.White;
            this.TextboxDungeon.Location = new System.Drawing.Point(578, 3);
            this.TextboxDungeon.Multiline = true;
            this.TextboxDungeon.Name = "TextboxDungeon";
            this.TextboxDungeon.ReadOnly = true;
            this.TextboxDungeon.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.TextboxDungeon.Size = new System.Drawing.Size(384, 290);
            this.TextboxDungeon.TabIndex = 7;
            this.TextboxDungeon.TextChanged += new System.EventHandler(this.TextboxDungeon_TextChanged);
            // 
            // ButtonNorth
            // 
            this.ButtonNorth.BackColor = System.Drawing.Color.Black;
            this.ButtonNorth.ForeColor = System.Drawing.Color.Lime;
            this.ButtonNorth.Location = new System.Drawing.Point(1, 388);
            this.ButtonNorth.Name = "ButtonNorth";
            this.ButtonNorth.Size = new System.Drawing.Size(75, 23);
            this.ButtonNorth.TabIndex = 8;
            this.ButtonNorth.Text = "North";
            this.ButtonNorth.UseVisualStyleBackColor = false;
            this.ButtonNorth.Click += new System.EventHandler(this.ButtonNorth_Click);
            // 
            // ButtonEast
            // 
            this.ButtonEast.BackColor = System.Drawing.Color.Black;
            this.ButtonEast.ForeColor = System.Drawing.Color.Lime;
            this.ButtonEast.Location = new System.Drawing.Point(163, 388);
            this.ButtonEast.Name = "ButtonEast";
            this.ButtonEast.Size = new System.Drawing.Size(75, 23);
            this.ButtonEast.TabIndex = 9;
            this.ButtonEast.Text = "East";
            this.ButtonEast.UseVisualStyleBackColor = false;
            this.ButtonEast.Click += new System.EventHandler(this.ButtonEast_Click);
            // 
            // ButtonSouth
            // 
            this.ButtonSouth.BackColor = System.Drawing.Color.Black;
            this.ButtonSouth.ForeColor = System.Drawing.Color.Lime;
            this.ButtonSouth.Location = new System.Drawing.Point(244, 388);
            this.ButtonSouth.Name = "ButtonSouth";
            this.ButtonSouth.Size = new System.Drawing.Size(75, 23);
            this.ButtonSouth.TabIndex = 10;
            this.ButtonSouth.Text = "South";
            this.ButtonSouth.UseVisualStyleBackColor = false;
            this.ButtonSouth.Click += new System.EventHandler(this.ButtonSouth_Click);
            // 
            // ButtonWest
            // 
            this.ButtonWest.BackColor = System.Drawing.Color.Black;
            this.ButtonWest.ForeColor = System.Drawing.Color.Lime;
            this.ButtonWest.Location = new System.Drawing.Point(82, 388);
            this.ButtonWest.Name = "ButtonWest";
            this.ButtonWest.Size = new System.Drawing.Size(75, 23);
            this.ButtonWest.TabIndex = 11;
            this.ButtonWest.Text = "West";
            this.ButtonWest.UseVisualStyleBackColor = false;
            this.ButtonWest.Click += new System.EventHandler(this.ButtonWest_Click);
            // 
            // ChangeName
            // 
            this.ChangeName.BackColor = System.Drawing.Color.Black;
            this.ChangeName.ForeColor = System.Drawing.Color.Lime;
            this.ChangeName.Location = new System.Drawing.Point(968, 358);
            this.ChangeName.Name = "ChangeName";
            this.ChangeName.Size = new System.Drawing.Size(90, 23);
            this.ChangeName.TabIndex = 12;
            this.ChangeName.Text = "Change Name";
            this.ChangeName.UseVisualStyleBackColor = false;
            this.ChangeName.Click += new System.EventHandler(this.ChangeNameClick);
            // 
            // AttackButton
            // 
            this.AttackButton.BackColor = System.Drawing.Color.Black;
            this.AttackButton.ForeColor = System.Drawing.Color.Lime;
            this.AttackButton.Location = new System.Drawing.Point(325, 388);
            this.AttackButton.Name = "AttackButton";
            this.AttackButton.Size = new System.Drawing.Size(75, 23);
            this.AttackButton.TabIndex = 13;
            this.AttackButton.Text = "Attack";
            this.AttackButton.UseVisualStyleBackColor = false;
            this.AttackButton.Click += new System.EventHandler(this.AttackSend);
            // 
            // DefendButton
            // 
            this.DefendButton.BackColor = System.Drawing.Color.Black;
            this.DefendButton.ForeColor = System.Drawing.Color.Lime;
            this.DefendButton.Location = new System.Drawing.Point(406, 388);
            this.DefendButton.Name = "DefendButton";
            this.DefendButton.Size = new System.Drawing.Size(75, 23);
            this.DefendButton.TabIndex = 14;
            this.DefendButton.Text = "Defend";
            this.DefendButton.UseVisualStyleBackColor = false;
            this.DefendButton.Click += new System.EventHandler(this.DefendSend);
            // 
            // WildAttackButton
            // 
            this.WildAttackButton.BackColor = System.Drawing.Color.Black;
            this.WildAttackButton.ForeColor = System.Drawing.Color.Lime;
            this.WildAttackButton.Location = new System.Drawing.Point(487, 388);
            this.WildAttackButton.Name = "WildAttackButton";
            this.WildAttackButton.Size = new System.Drawing.Size(75, 23);
            this.WildAttackButton.TabIndex = 15;
            this.WildAttackButton.Text = "Wild Attack";
            this.WildAttackButton.UseVisualStyleBackColor = false;
            this.WildAttackButton.Click += new System.EventHandler(this.WildAttackSend);
            // 
            // DungeonGraphic
            // 
            this.DungeonGraphic.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.DungeonGraphic.Location = new System.Drawing.Point(1, 3);
            this.DungeonGraphic.Name = "DungeonGraphic";
            this.DungeonGraphic.Size = new System.Drawing.Size(561, 378);
            this.DungeonGraphic.TabIndex = 16;
            this.DungeonGraphic.TabStop = false;
            this.DungeonGraphic.Click += new System.EventHandler(this.DungeonGraphic_Click);
            this.DungeonGraphic.Paint += new System.Windows.Forms.PaintEventHandler(this.DungeonPaint);
            // 
            // ChatBox
            // 
            this.ChatBox.BackColor = System.Drawing.SystemColors.GrayText;
            this.ChatBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ChatBox.ForeColor = System.Drawing.Color.White;
            this.ChatBox.Location = new System.Drawing.Point(578, 299);
            this.ChatBox.Multiline = true;
            this.ChatBox.Name = "ChatBox";
            this.ChatBox.ReadOnly = true;
            this.ChatBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.ChatBox.Size = new System.Drawing.Size(384, 82);
            this.ChatBox.TabIndex = 17;
            // 
            // Form1
            // 
            this.AcceptButton = this.buttonSend;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1075, 417);
            this.Controls.Add(this.ChatBox);
            this.Controls.Add(this.DungeonGraphic);
            this.Controls.Add(this.WildAttackButton);
            this.Controls.Add(this.DefendButton);
            this.Controls.Add(this.AttackButton);
            this.Controls.Add(this.ChangeName);
            this.Controls.Add(this.ButtonWest);
            this.Controls.Add(this.ButtonSouth);
            this.Controls.Add(this.ButtonEast);
            this.Controls.Add(this.ButtonNorth);
            this.Controls.Add(this.TextboxDungeon);
            this.Controls.Add(this.NameBox);
            this.Controls.Add(this.listBox_ClientList);
            this.Controls.Add(this.textBox_Input);
            this.Controls.Add(this.buttonSend);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.KeyPreview = true;
            this.Name = "Form1";
            this.Text = "Dungeon Madness";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.DungeonGraphic)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonSend;
        private System.Windows.Forms.TextBox textBox_Input;
        private System.Windows.Forms.ListBox listBox_ClientList;
        private System.Windows.Forms.TextBox NameBox;
        private System.Windows.Forms.TextBox TextboxDungeon;
        private System.Windows.Forms.Button ButtonNorth;
        private System.Windows.Forms.Button ButtonEast;
        private System.Windows.Forms.Button ButtonSouth;
        private System.Windows.Forms.Button ButtonWest;
        private System.Windows.Forms.Button ChangeName;
        private System.Windows.Forms.Button AttackButton;
        private System.Windows.Forms.Button DefendButton;
        private System.Windows.Forms.Button WildAttackButton;
        private System.Windows.Forms.PictureBox DungeonGraphic;
        private System.Windows.Forms.TextBox ChatBox;
    }
}

