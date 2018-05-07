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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.buttonSend = new System.Windows.Forms.Button();
            this.textBox_Input = new System.Windows.Forms.TextBox();
            this.TextboxDungeon = new System.Windows.Forms.TextBox();
            this.DungeonGraphic = new System.Windows.Forms.PictureBox();
            this.Zoomin = new System.Windows.Forms.Button();
            this.Zoomout = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.DungeonGraphic)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonSend
            // 
            this.buttonSend.BackColor = System.Drawing.Color.Black;
            this.buttonSend.ForeColor = System.Drawing.Color.Lime;
            this.buttonSend.Location = new System.Drawing.Point(352, 471);
            this.buttonSend.Name = "buttonSend";
            this.buttonSend.Size = new System.Drawing.Size(90, 40);
            this.buttonSend.TabIndex = 0;
            this.buttonSend.Text = "Send";
            this.buttonSend.UseVisualStyleBackColor = false;
            this.buttonSend.Click += new System.EventHandler(this.buttonSend_Click);
            // 
            // textBox_Input
            // 
            this.textBox_Input.BackColor = System.Drawing.Color.Black;
            this.textBox_Input.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox_Input.ForeColor = System.Drawing.Color.White;
            this.textBox_Input.Location = new System.Drawing.Point(11, 471);
            this.textBox_Input.Multiline = true;
            this.textBox_Input.Name = "textBox_Input";
            this.textBox_Input.Size = new System.Drawing.Size(334, 40);
            this.textBox_Input.TabIndex = 2;
            // 
            // TextboxDungeon
            // 
            this.TextboxDungeon.BackColor = System.Drawing.SystemColors.InfoText;
            this.TextboxDungeon.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TextboxDungeon.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextboxDungeon.ForeColor = System.Drawing.Color.White;
            this.TextboxDungeon.Location = new System.Drawing.Point(11, 8);
            this.TextboxDungeon.Multiline = true;
            this.TextboxDungeon.Name = "TextboxDungeon";
            this.TextboxDungeon.ReadOnly = true;
            this.TextboxDungeon.Size = new System.Drawing.Size(431, 457);
            this.TextboxDungeon.TabIndex = 7;
            // 
            // DungeonGraphic
            // 
            this.DungeonGraphic.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.DungeonGraphic.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.DungeonGraphic.ErrorImage = null;
            this.DungeonGraphic.Location = new System.Drawing.Point(448, 8);
            this.DungeonGraphic.Name = "DungeonGraphic";
            this.DungeonGraphic.Size = new System.Drawing.Size(624, 503);
            this.DungeonGraphic.TabIndex = 16;
            this.DungeonGraphic.TabStop = false;
            this.DungeonGraphic.Paint += new System.Windows.Forms.PaintEventHandler(this.DungeonPaint);
            // 
            // Zoomin
            // 
            this.Zoomin.BackColor = System.Drawing.Color.Black;
            this.Zoomin.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.Zoomin.Image = ((System.Drawing.Image)(resources.GetObject("Zoomin.Image")));
            this.Zoomin.Location = new System.Drawing.Point(1020, 12);
            this.Zoomin.Name = "Zoomin";
            this.Zoomin.Size = new System.Drawing.Size(50, 50);
            this.Zoomin.TabIndex = 17;
            this.Zoomin.UseVisualStyleBackColor = false;
            this.Zoomin.Click += new System.EventHandler(this.Zoomin_Click);
            // 
            // Zoomout
            // 
            this.Zoomout.BackColor = System.Drawing.Color.Black;
            this.Zoomout.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.Zoomout.Image = ((System.Drawing.Image)(resources.GetObject("Zoomout.Image")));
            this.Zoomout.Location = new System.Drawing.Point(1020, 68);
            this.Zoomout.Name = "Zoomout";
            this.Zoomout.Size = new System.Drawing.Size(50, 50);
            this.Zoomout.TabIndex = 18;
            this.Zoomout.UseVisualStyleBackColor = false;
            this.Zoomout.Click += new System.EventHandler(this.Zoomout_Click);
            // 
            // Form1
            // 
            this.AcceptButton = this.buttonSend;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1082, 518);
            this.Controls.Add(this.Zoomout);
            this.Controls.Add(this.Zoomin);
            this.Controls.Add(this.DungeonGraphic);
            this.Controls.Add(this.TextboxDungeon);
            this.Controls.Add(this.textBox_Input);
            this.Controls.Add(this.buttonSend);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.KeyPreview = true;
            this.Name = "Form1";
            this.Text = "Dungeon Madness";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.DungeonGraphic)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonSend;
        private System.Windows.Forms.TextBox textBox_Input;
        private System.Windows.Forms.TextBox TextboxDungeon;
        private System.Windows.Forms.PictureBox DungeonGraphic;
        private System.Windows.Forms.Button Zoomin;
        private System.Windows.Forms.Button Zoomout;
    }
}

