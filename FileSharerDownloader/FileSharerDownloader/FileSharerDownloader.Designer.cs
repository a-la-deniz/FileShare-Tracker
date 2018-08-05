namespace FileSharerDownloader
{
    partial class FileSharerDownloader
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
            this.share = new System.Windows.Forms.Button();
            this.dir = new System.Windows.Forms.Button();
            this.down = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox6 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.textBox7 = new System.Windows.Forms.TextBox();
            this.outbox = new System.Windows.Forms.RichTextBox();
            this.send = new System.Windows.Forms.Button();
            this.closer = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // share
            // 
            this.share.Location = new System.Drawing.Point(19, 12);
            this.share.Name = "share";
            this.share.Size = new System.Drawing.Size(96, 50);
            this.share.TabIndex = 0;
            this.share.Text = "Share Folder";
            this.share.UseVisualStyleBackColor = true;
            this.share.Click += new System.EventHandler(this.share_Click);
            // 
            // dir
            // 
            this.dir.Location = new System.Drawing.Point(493, 54);
            this.dir.Name = "dir";
            this.dir.Size = new System.Drawing.Size(158, 25);
            this.dir.TabIndex = 1;
            this.dir.Text = "Set Download Location";
            this.dir.UseVisualStyleBackColor = true;
            this.dir.Click += new System.EventHandler(this.dir_Click);
            // 
            // down
            // 
            this.down.Enabled = false;
            this.down.Location = new System.Drawing.Point(825, 12);
            this.down.Name = "down";
            this.down.Size = new System.Drawing.Size(96, 50);
            this.down.TabIndex = 2;
            this.down.Text = "Get File!";
            this.down.UseVisualStyleBackColor = true;
            this.down.Click += new System.EventHandler(this.down_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(124, 25);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(156, 20);
            this.textBox1.TabIndex = 4;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(302, 25);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(74, 20);
            this.textBox2.TabIndex = 5;
            // 
            // textBox6
            // 
            this.textBox6.Location = new System.Drawing.Point(386, 25);
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new System.Drawing.Size(74, 20);
            this.textBox6.TabIndex = 9;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(490, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "File Name:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(286, 28);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(10, 13);
            this.label5.TabIndex = 14;
            this.label5.Text = ":";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(121, 9);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(60, 13);
            this.label6.TabIndex = 15;
            this.label6.Text = "Tracker IP:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(299, 9);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(69, 13);
            this.label7.TabIndex = 16;
            this.label7.Text = "Tracker Port:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(383, 9);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(101, 13);
            this.label8.TabIndex = 17;
            this.label8.Text = "Local Port to Listen:";
            // 
            // textBox7
            // 
            this.textBox7.Location = new System.Drawing.Point(493, 25);
            this.textBox7.Name = "textBox7";
            this.textBox7.Size = new System.Drawing.Size(306, 20);
            this.textBox7.TabIndex = 18;
            // 
            // outbox
            // 
            this.outbox.Location = new System.Drawing.Point(19, 89);
            this.outbox.Name = "outbox";
            this.outbox.Size = new System.Drawing.Size(902, 465);
            this.outbox.TabIndex = 19;
            this.outbox.Text = "";
            // 
            // send
            // 
            this.send.Enabled = false;
            this.send.Location = new System.Drawing.Point(302, 54);
            this.send.Name = "send";
            this.send.Size = new System.Drawing.Size(158, 25);
            this.send.TabIndex = 20;
            this.send.Text = "Send file list and listen";
            this.send.UseVisualStyleBackColor = true;
            this.send.Click += new System.EventHandler(this.send_Click);
            // 
            // closer
            // 
            this.closer.Location = new System.Drawing.Point(657, 54);
            this.closer.Name = "closer";
            this.closer.Size = new System.Drawing.Size(142, 25);
            this.closer.TabIndex = 21;
            this.closer.Text = "Close Program";
            this.closer.UseVisualStyleBackColor = true;
            this.closer.Click += new System.EventHandler(this.closer_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(944, 566);
            this.Controls.Add(this.closer);
            this.Controls.Add(this.send);
            this.Controls.Add(this.outbox);
            this.Controls.Add(this.textBox7);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox6);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.down);
            this.Controls.Add(this.dir);
            this.Controls.Add(this.share);
            this.Name = "Form1";
            this.Text = "FileSharerDownloader";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button share;
        private System.Windows.Forms.Button dir;
        private System.Windows.Forms.Button down;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox6;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBox7;
        private System.Windows.Forms.RichTextBox outbox;
        private System.Windows.Forms.Button send;
        private System.Windows.Forms.Button closer;
    }
}

