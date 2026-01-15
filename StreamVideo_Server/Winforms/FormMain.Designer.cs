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
            components = new System.ComponentModel.Container();
            lblTitle = new Label();
            btnStart = new Button();
            btnStop = new Button();
            lblStatus = new Label();
            lstLog = new ListBox();
            timerStream = new System.Windows.Forms.Timer(components);
            SuspendLayout();
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblTitle.Location = new Point(23, 20);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(284, 32);
            lblTitle.TabIndex = 4;
            lblTitle.Text = "SERVER STREAM VIDEO";
            // 
            // btnStart
            // 
            btnStart.Location = new Point(29, 73);
            btnStart.Margin = new Padding(3, 4, 3, 4);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(137, 40);
            btnStart.TabIndex = 3;
            btnStart.Text = "Start Server";
            btnStart.UseVisualStyleBackColor = true;
            btnStart.Click += btnStart_Click;
            // 
            // btnStop
            // 
            btnStop.Enabled = false;
            btnStop.Location = new Point(183, 73);
            btnStop.Margin = new Padding(3, 4, 3, 4);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(137, 40);
            btnStop.TabIndex = 2;
            btnStop.Text = "Stop Server";
            btnStop.UseVisualStyleBackColor = true;
            btnStop.Click += btnStop_Click;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.ForeColor = Color.Red;
            lblStatus.Location = new Point(29, 133);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(150, 20);
            lblStatus.TabIndex = 1;
            lblStatus.Text = "Trạng thái: ĐÃ DỪNG";
            // 
            // lstLog
            // 
            lstLog.FormattingEnabled = true;
            lstLog.Location = new Point(29, 173);
            lstLog.Margin = new Padding(3, 4, 3, 4);
            lstLog.Name = "lstLog";
            lstLog.Size = new Size(594, 244);
            lstLog.TabIndex = 0;
            // 
            // timerStream
            // 
            timerStream.Tick += timerStream_Tick_1;
            // 
            // FormMain
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(663, 453);
            Controls.Add(lstLog);
            Controls.Add(lblStatus);
            Controls.Add(btnStop);
            Controls.Add(btnStart);
            Controls.Add(lblTitle);
            Margin = new Padding(3, 4, 3, 4);
            Name = "FormMain";
            Text = "Server Stream";
            ResumeLayout(false);
            PerformLayout();
        }

        private System.Windows.Forms.Timer timerStream;
    }
}
