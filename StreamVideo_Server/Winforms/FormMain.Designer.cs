namespace StreamVideo_Server.Winforms
{
    partial class FormMain
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ListBox lstLog;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            lblTitle = new System.Windows.Forms.Label();
            btnStart = new System.Windows.Forms.Button();
            btnStop = new System.Windows.Forms.Button();
            lblStatus = new System.Windows.Forms.Label();
            lstLog = new System.Windows.Forms.ListBox();
            SuspendLayout();
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            lblTitle.Location = new System.Drawing.Point(20, 15);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new System.Drawing.Size(260, 25);
            lblTitle.Text = "SERVER STREAM VIDEO";
            // 
            // btnStart
            // 
            btnStart.Location = new System.Drawing.Point(25, 55);
            btnStart.Name = "btnStart";
            btnStart.Size = new System.Drawing.Size(120, 30);
            btnStart.Text = "Start Server";
            btnStart.UseVisualStyleBackColor = true;
            btnStart.Click += btnStart_Click;
            // 
            // btnStop
            // 
            btnStop.Location = new System.Drawing.Point(160, 55);
            btnStop.Name = "btnStop";
            btnStop.Size = new System.Drawing.Size(120, 30);
            btnStop.Text = "Stop Server";
            btnStop.Enabled = false;
            btnStop.UseVisualStyleBackColor = true;
            btnStop.Click += btnStop_Click;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.ForeColor = System.Drawing.Color.Red;
            lblStatus.Location = new System.Drawing.Point(25, 100);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new System.Drawing.Size(120, 15);
            lblStatus.Text = "Trạng thái: ĐÃ DỪNG";
            // 
            // lstLog
            // 
            lstLog.FormattingEnabled = true;
            lstLog.ItemHeight = 15;
            lstLog.Location = new System.Drawing.Point(25, 130);
            lstLog.Name = "lstLog";
            lstLog.Size = new System.Drawing.Size(520, 184);
            // 
            // FormMain
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(580, 340);
            Controls.Add(lstLog);
            Controls.Add(lblStatus);
            Controls.Add(btnStop);
            Controls.Add(btnStart);
            Controls.Add(lblTitle);
            Name = "FormMain";
            Text = "Server Stream";
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
