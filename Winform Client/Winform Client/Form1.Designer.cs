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
            this.textBox_Output = new System.Windows.Forms.TextBox();
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
            ((System.ComponentModel.ISupportInitialize)(this.DungeonGraphic)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonSend
            // 
            this.buttonSend.Location = new System.Drawing.Point(411, 396);
            this.buttonSend.Name = "buttonSend";
            this.buttonSend.Size = new System.Drawing.Size(90, 21);
            this.buttonSend.TabIndex = 0;
            this.buttonSend.Text = "Send";
            this.buttonSend.UseVisualStyleBackColor = true;
            this.buttonSend.Click += new System.EventHandler(this.buttonSend_Click);
            // 
            // textBox_Input
            // 
            this.textBox_Input.Location = new System.Drawing.Point(21, 396);
            this.textBox_Input.Name = "textBox_Input";
            this.textBox_Input.Size = new System.Drawing.Size(384, 20);
            this.textBox_Input.TabIndex = 2;
            this.textBox_Input.TextChanged += new System.EventHandler(this.textBox_Input_TextChanged);
            // 
            // textBox_Output
            // 
            this.textBox_Output.Location = new System.Drawing.Point(22, 312);
            this.textBox_Output.Multiline = true;
            this.textBox_Output.Name = "textBox_Output";
            this.textBox_Output.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox_Output.Size = new System.Drawing.Size(384, 78);
            this.textBox_Output.TabIndex = 3;
            // 
            // listBox_ClientList
            // 
            this.listBox_ClientList.FormattingEnabled = true;
            this.listBox_ClientList.Location = new System.Drawing.Point(412, 69);
            this.listBox_ClientList.Name = "listBox_ClientList";
            this.listBox_ClientList.Size = new System.Drawing.Size(89, 316);
            this.listBox_ClientList.TabIndex = 5;
            this.listBox_ClientList.SelectedIndexChanged += new System.EventHandler(this.listBox_ClientList_SelectedIndexChanged);
            // 
            // NameBox
            // 
            this.NameBox.Location = new System.Drawing.Point(412, 41);
            this.NameBox.Name = "NameBox";
            this.NameBox.Size = new System.Drawing.Size(89, 20);
            this.NameBox.TabIndex = 6;
            this.NameBox.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // TextboxDungeon
            // 
            this.TextboxDungeon.Location = new System.Drawing.Point(22, 12);
            this.TextboxDungeon.Multiline = true;
            this.TextboxDungeon.Name = "TextboxDungeon";
            this.TextboxDungeon.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.TextboxDungeon.Size = new System.Drawing.Size(384, 294);
            this.TextboxDungeon.TabIndex = 7;
            this.TextboxDungeon.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            // 
            // ButtonNorth
            // 
            this.ButtonNorth.Location = new System.Drawing.Point(147, 424);
            this.ButtonNorth.Name = "ButtonNorth";
            this.ButtonNorth.Size = new System.Drawing.Size(75, 23);
            this.ButtonNorth.TabIndex = 8;
            this.ButtonNorth.Text = "North";
            this.ButtonNorth.UseVisualStyleBackColor = true;
            this.ButtonNorth.Click += new System.EventHandler(this.ButtonNorth_Click);
            // 
            // ButtonEast
            // 
            this.ButtonEast.Location = new System.Drawing.Point(224, 452);
            this.ButtonEast.Name = "ButtonEast";
            this.ButtonEast.Size = new System.Drawing.Size(75, 23);
            this.ButtonEast.TabIndex = 9;
            this.ButtonEast.Text = "East";
            this.ButtonEast.UseVisualStyleBackColor = true;
            this.ButtonEast.Click += new System.EventHandler(this.ButtonEast_Click);
            // 
            // ButtonSouth
            // 
            this.ButtonSouth.Location = new System.Drawing.Point(147, 481);
            this.ButtonSouth.Name = "ButtonSouth";
            this.ButtonSouth.Size = new System.Drawing.Size(75, 23);
            this.ButtonSouth.TabIndex = 10;
            this.ButtonSouth.Text = "South";
            this.ButtonSouth.UseVisualStyleBackColor = true;
            this.ButtonSouth.Click += new System.EventHandler(this.ButtonSouth_Click);
            // 
            // ButtonWest
            // 
            this.ButtonWest.Location = new System.Drawing.Point(66, 452);
            this.ButtonWest.Name = "ButtonWest";
            this.ButtonWest.Size = new System.Drawing.Size(75, 23);
            this.ButtonWest.TabIndex = 11;
            this.ButtonWest.Text = "West";
            this.ButtonWest.UseVisualStyleBackColor = true;
            this.ButtonWest.Click += new System.EventHandler(this.ButtonWest_Click);
            // 
            // ChangeName
            // 
            this.ChangeName.Location = new System.Drawing.Point(411, 12);
            this.ChangeName.Name = "ChangeName";
            this.ChangeName.Size = new System.Drawing.Size(90, 23);
            this.ChangeName.TabIndex = 12;
            this.ChangeName.Text = "Change Name";
            this.ChangeName.UseVisualStyleBackColor = true;
            this.ChangeName.Click += new System.EventHandler(this.ChangeNameClick);
            // 
            // AttackButton
            // 
            this.AttackButton.Location = new System.Drawing.Point(305, 423);
            this.AttackButton.Name = "AttackButton";
            this.AttackButton.Size = new System.Drawing.Size(75, 23);
            this.AttackButton.TabIndex = 13;
            this.AttackButton.Text = "Attack";
            this.AttackButton.UseVisualStyleBackColor = true;
            this.AttackButton.Click += new System.EventHandler(this.AttackSend);
            // 
            // DefendButton
            // 
            this.DefendButton.Location = new System.Drawing.Point(305, 452);
            this.DefendButton.Name = "DefendButton";
            this.DefendButton.Size = new System.Drawing.Size(75, 23);
            this.DefendButton.TabIndex = 14;
            this.DefendButton.Text = "Defend";
            this.DefendButton.UseVisualStyleBackColor = true;
            this.DefendButton.Click += new System.EventHandler(this.DefendSend);
            // 
            // WildAttackButton
            // 
            this.WildAttackButton.Location = new System.Drawing.Point(305, 481);
            this.WildAttackButton.Name = "WildAttackButton";
            this.WildAttackButton.Size = new System.Drawing.Size(75, 23);
            this.WildAttackButton.TabIndex = 15;
            this.WildAttackButton.Text = "Wild Attack";
            this.WildAttackButton.UseVisualStyleBackColor = true;
            this.WildAttackButton.Click += new System.EventHandler(this.WildAttackSend);
            // 
            // DungeonGraphic
            // 
            this.DungeonGraphic.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.DungeonGraphic.Location = new System.Drawing.Point(507, 69);
            this.DungeonGraphic.Name = "DungeonGraphic";
            this.DungeonGraphic.Size = new System.Drawing.Size(291, 237);
            this.DungeonGraphic.TabIndex = 16;
            this.DungeonGraphic.TabStop = false;
            // 
            // Form1
            // 
            this.AcceptButton = this.buttonSend;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Info;
            this.ClientSize = new System.Drawing.Size(851, 512);
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
            this.Controls.Add(this.textBox_Output);
            this.Controls.Add(this.textBox_Input);
            this.Controls.Add(this.buttonSend);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.KeyPreview = true;
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.DungeonGraphic)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonSend;
        private System.Windows.Forms.TextBox textBox_Input;
        private System.Windows.Forms.TextBox textBox_Output;
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
    }
}

