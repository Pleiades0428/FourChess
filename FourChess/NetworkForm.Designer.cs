namespace FourChess
{
    partial class NetworkForm
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
            this.tbLog = new System.Windows.Forms.TextBox();
            this.btnStartHost = new System.Windows.Forms.Button();
            this.tbIP = new System.Windows.Forms.TextBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // tbLog
            // 
            this.tbLog.Location = new System.Drawing.Point(191, 12);
            this.tbLog.Multiline = true;
            this.tbLog.Name = "tbLog";
            this.tbLog.Size = new System.Drawing.Size(222, 320);
            this.tbLog.TabIndex = 5;
            // 
            // btnStartHost
            // 
            this.btnStartHost.Location = new System.Drawing.Point(12, 12);
            this.btnStartHost.Name = "btnStartHost";
            this.btnStartHost.Size = new System.Drawing.Size(173, 23);
            this.btnStartHost.TabIndex = 6;
            this.btnStartHost.Text = "我要建主机";
            this.btnStartHost.UseVisualStyleBackColor = true;
            // 
            // tbIP
            // 
            this.tbIP.Location = new System.Drawing.Point(12, 41);
            this.tbIP.Name = "tbIP";
            this.tbIP.Size = new System.Drawing.Size(100, 21);
            this.tbIP.TabIndex = 8;
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(110, 39);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(75, 23);
            this.btnConnect.TabIndex = 7;
            this.btnConnect.Text = "连接";
            this.btnConnect.UseVisualStyleBackColor = true;
            // 
            // NetworkForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(425, 344);
            this.Controls.Add(this.tbLog);
            this.Controls.Add(this.btnStartHost);
            this.Controls.Add(this.tbIP);
            this.Controls.Add(this.btnConnect);
            this.Name = "NetworkForm";
            this.Text = "NetworkForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbLog;
        private System.Windows.Forms.Button btnStartHost;
        private System.Windows.Forms.TextBox tbIP;
        private System.Windows.Forms.Button btnConnect;
    }
}