namespace Tracker
{
    partial class Tracker
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
            this.fileListView = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.startTracking = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.outbox = new System.Windows.Forms.RichTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.fileListView)).BeginInit();
            this.SuspendLayout();
            // 
            // fileListView
            // 
            this.fileListView.AllowUserToAddRows = false;
            this.fileListView.AllowUserToDeleteRows = false;
            this.fileListView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.fileListView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.Column3});
            this.fileListView.Location = new System.Drawing.Point(12, 12);
            this.fileListView.Name = "fileListView";
            this.fileListView.ReadOnly = true;
            this.fileListView.Size = new System.Drawing.Size(922, 168);
            this.fileListView.TabIndex = 0;
            this.fileListView.TabStop = false;
            // 
            // Column1
            // 
            this.Column1.FillWeight = 200F;
            this.Column1.HeaderText = "File Name";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            this.Column1.Width = 200;
            // 
            // Column2
            // 
            this.Column2.FillWeight = 179F;
            this.Column2.HeaderText = "File Size";
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            this.Column2.Width = 179;
            // 
            // Column3
            // 
            this.Column3.FillWeight = 500F;
            this.Column3.HeaderText = "IP EndPoints";
            this.Column3.Name = "Column3";
            this.Column3.ReadOnly = true;
            this.Column3.Width = 500;
            // 
            // startTracking
            // 
            this.startTracking.Location = new System.Drawing.Point(12, 186);
            this.startTracking.Name = "startTracking";
            this.startTracking.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.startTracking.Size = new System.Drawing.Size(792, 22);
            this.startTracking.TabIndex = 1;
            this.startTracking.Text = "Start tracking on port:";
            this.startTracking.UseVisualStyleBackColor = true;
            this.startTracking.Click += new System.EventHandler(this.startTracking_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(810, 188);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(124, 20);
            this.textBox1.TabIndex = 2;
            // 
            // outbox
            // 
            this.outbox.Location = new System.Drawing.Point(12, 214);
            this.outbox.Name = "outbox";
            this.outbox.Size = new System.Drawing.Size(921, 286);
            this.outbox.TabIndex = 3;
            this.outbox.Text = "";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(946, 512);
            this.Controls.Add(this.outbox);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.startTracking);
            this.Controls.Add(this.fileListView);
            this.Name = "Form1";
            this.Text = "Tracker";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.fileListView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView fileListView;
        private System.Windows.Forms.Button startTracking;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.RichTextBox outbox;


    }
}

