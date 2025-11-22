namespace WakeOnLanApp
{
    partial class MainForm
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true。それ以外の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        private System.Windows.Forms.TextBox txtMacAddress;
        private System.Windows.Forms.Button btnWake;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.Label lblMacAddress;
        private System.Windows.Forms.TextBox txtBroadcastAddress;
        private System.Windows.Forms.Label lblBroadcastAddress;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.Label lblPort;
        private System.Windows.Forms.Panel panelDevices;
        private System.Windows.Forms.Label lblDevices;
        private System.Windows.Forms.ListBox lstDevices;
        private System.Windows.Forms.Label lblDeviceName;
        private System.Windows.Forms.TextBox txtDeviceName;
        private System.Windows.Forms.Label lblDeviceMac;
        private System.Windows.Forms.TextBox txtDeviceMac;
        private System.Windows.Forms.Label lblDeviceBroadcast;
        private System.Windows.Forms.TextBox txtDeviceBroadcast;
        private System.Windows.Forms.Button btnAddDevice;

        private void InitializeComponent()
        {
            this.txtMacAddress = new System.Windows.Forms.TextBox();
            this.btnWake = new System.Windows.Forms.Button();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.lblMacAddress = new System.Windows.Forms.Label();
            this.txtBroadcastAddress = new System.Windows.Forms.TextBox();
            this.lblBroadcastAddress = new System.Windows.Forms.Label();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.lblPort = new System.Windows.Forms.Label();
            this.panelDevices = new System.Windows.Forms.Panel();
            this.btnAddDevice = new System.Windows.Forms.Button();
            this.txtDeviceBroadcast = new System.Windows.Forms.TextBox();
            this.lblDeviceBroadcast = new System.Windows.Forms.Label();
            this.txtDeviceMac = new System.Windows.Forms.TextBox();
            this.lblDeviceMac = new System.Windows.Forms.Label();
            this.txtDeviceName = new System.Windows.Forms.TextBox();
            this.lblDeviceName = new System.Windows.Forms.Label();
            this.lstDevices = new System.Windows.Forms.ListBox();
            this.lblDevices = new System.Windows.Forms.Label();
            this.panelDevices.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtMacAddress
            // 
            this.txtMacAddress.Location = new System.Drawing.Point(340, 20);
            this.txtMacAddress.Name = "txtMacAddress";
            this.txtMacAddress.Size = new System.Drawing.Size(200, 19);
            this.txtMacAddress.TabIndex = 1;
            // 
            // btnWake
            // 
            this.btnWake.Location = new System.Drawing.Point(720, 18);
            this.btnWake.Name = "btnWake";
            this.btnWake.Size = new System.Drawing.Size(100, 23);
            this.btnWake.TabIndex = 4;
            this.btnWake.Text = "送信";
            this.btnWake.UseVisualStyleBackColor = true;
            this.btnWake.Click += new System.EventHandler(this.btnWake_Click);
            // 
            // txtLog
            // 
            this.txtLog.Location = new System.Drawing.Point(240, 130);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(600, 290);
            this.txtLog.TabIndex = 6;
            // 
            // lblMacAddress
            // 
            this.lblMacAddress.AutoSize = true;
            this.lblMacAddress.Location = new System.Drawing.Point(240, 23);
            this.lblMacAddress.Name = "lblMacAddress";
            this.lblMacAddress.Size = new System.Drawing.Size(79, 12);
            this.lblMacAddress.TabIndex = 4;
            this.lblMacAddress.Text = "MACアドレス:";
            // 
            // txtBroadcastAddress
            // 
            this.txtBroadcastAddress.Location = new System.Drawing.Point(240, 90);
            this.txtBroadcastAddress.Name = "txtBroadcastAddress";
            this.txtBroadcastAddress.Size = new System.Drawing.Size(580, 19);
            this.txtBroadcastAddress.TabIndex = 5;
            this.txtBroadcastAddress.Text = "192.168.3.255";
            // 
            // lblBroadcastAddress
            // 
            this.lblBroadcastAddress.AutoSize = true;
            this.lblBroadcastAddress.Location = new System.Drawing.Point(240, 72);
            this.lblBroadcastAddress.Name = "lblBroadcastAddress";
            this.lblBroadcastAddress.Size = new System.Drawing.Size(241, 12);
            this.lblBroadcastAddress.TabIndex = 5;
            this.lblBroadcastAddress.Text = "ブロードキャストアドレス (複数はカンマ区切り可):";
            // 
            // txtPort
            // 
            this.txtPort.Location = new System.Drawing.Point(580, 20);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(80, 19);
            this.txtPort.TabIndex = 2;
            this.txtPort.Text = "9";
            // 
            // lblPort
            // 
            this.lblPort.AutoSize = true;
            this.lblPort.Location = new System.Drawing.Point(550, 23);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new System.Drawing.Size(34, 12);
            this.lblPort.TabIndex = 6;
            this.lblPort.Text = "ポート:";
            // 
            // panelDevices
            // 
            this.panelDevices.Controls.Add(this.btnAddDevice);
            this.panelDevices.Controls.Add(this.txtDeviceBroadcast);
            this.panelDevices.Controls.Add(this.lblDeviceBroadcast);
            this.panelDevices.Controls.Add(this.txtDeviceMac);
            this.panelDevices.Controls.Add(this.lblDeviceMac);
            this.panelDevices.Controls.Add(this.txtDeviceName);
            this.panelDevices.Controls.Add(this.lblDeviceName);
            this.panelDevices.Controls.Add(this.lstDevices);
            this.panelDevices.Controls.Add(this.lblDevices);
            this.panelDevices.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelDevices.Location = new System.Drawing.Point(0, 0);
            this.panelDevices.Name = "panelDevices";
            this.panelDevices.Size = new System.Drawing.Size(220, 450);
            this.panelDevices.TabIndex = 0;
            // 
            // btnAddDevice
            // 
            this.btnAddDevice.Location = new System.Drawing.Point(10, 360);
            this.btnAddDevice.Name = "btnAddDevice";
            this.btnAddDevice.Size = new System.Drawing.Size(200, 25);
            this.btnAddDevice.TabIndex = 8;
            this.btnAddDevice.Text = "端末を登録";
            this.btnAddDevice.UseVisualStyleBackColor = true;
            this.btnAddDevice.Click += new System.EventHandler(this.btnAddDevice_Click);
            // 
            // txtDeviceBroadcast
            // 
            this.txtDeviceBroadcast.Location = new System.Drawing.Point(10, 330);
            this.txtDeviceBroadcast.Name = "txtDeviceBroadcast";
            this.txtDeviceBroadcast.Size = new System.Drawing.Size(200, 19);
            this.txtDeviceBroadcast.TabIndex = 7;
            // 
            // lblDeviceBroadcast
            // 
            this.lblDeviceBroadcast.AutoSize = true;
            this.lblDeviceBroadcast.Location = new System.Drawing.Point(10, 312);
            this.lblDeviceBroadcast.Name = "lblDeviceBroadcast";
            this.lblDeviceBroadcast.Size = new System.Drawing.Size(176, 12);
            this.lblDeviceBroadcast.TabIndex = 4;
            this.lblDeviceBroadcast.Text = "ブロードキャスト (任意・カンマ可)";
            // 
            // txtDeviceMac
            // 
            this.txtDeviceMac.Location = new System.Drawing.Point(10, 280);
            this.txtDeviceMac.Name = "txtDeviceMac";
            this.txtDeviceMac.Size = new System.Drawing.Size(200, 19);
            this.txtDeviceMac.TabIndex = 6;
            // 
            // lblDeviceMac
            // 
            this.lblDeviceMac.AutoSize = true;
            this.lblDeviceMac.Location = new System.Drawing.Point(10, 262);
            this.lblDeviceMac.Name = "lblDeviceMac";
            this.lblDeviceMac.Size = new System.Drawing.Size(79, 12);
            this.lblDeviceMac.TabIndex = 3;
            this.lblDeviceMac.Text = "MACアドレス:";
            // 
            // txtDeviceName
            // 
            this.txtDeviceName.Location = new System.Drawing.Point(10, 230);
            this.txtDeviceName.Name = "txtDeviceName";
            this.txtDeviceName.Size = new System.Drawing.Size(200, 19);
            this.txtDeviceName.TabIndex = 5;
            // 
            // lblDeviceName
            // 
            this.lblDeviceName.AutoSize = true;
            this.lblDeviceName.Location = new System.Drawing.Point(10, 212);
            this.lblDeviceName.Name = "lblDeviceName";
            this.lblDeviceName.Size = new System.Drawing.Size(53, 12);
            this.lblDeviceName.TabIndex = 2;
            this.lblDeviceName.Text = "端末名:";
            // 
            // lstDevices
            // 
            this.lstDevices.FormattingEnabled = true;
            this.lstDevices.ItemHeight = 12;
            this.lstDevices.Location = new System.Drawing.Point(10, 35);
            this.lstDevices.Name = "lstDevices";
            this.lstDevices.Size = new System.Drawing.Size(200, 160);
            this.lstDevices.TabIndex = 4;
            this.lstDevices.SelectedIndexChanged += new System.EventHandler(this.lstDevices_SelectedIndexChanged);
            // 
            // lblDevices
            // 
            this.lblDevices.AutoSize = true;
            this.lblDevices.Location = new System.Drawing.Point(10, 10);
            this.lblDevices.Name = "lblDevices";
            this.lblDevices.Size = new System.Drawing.Size(77, 12);
            this.lblDevices.TabIndex = 0;
            this.lblDevices.Text = "登録済み端末";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(860, 450);
            this.Controls.Add(this.panelDevices);
            this.Controls.Add(this.lblPort);
            this.Controls.Add(this.txtPort);
            this.Controls.Add(this.lblBroadcastAddress);
            this.Controls.Add(this.txtBroadcastAddress);
            this.Controls.Add(this.lblMacAddress);
            this.Controls.Add(this.txtMacAddress);
            this.Controls.Add(this.btnWake);
            this.Controls.Add(this.txtLog);
            this.Name = "Form1";
            this.Text = "Wake-on-LAN Sender";
            this.panelDevices.ResumeLayout(false);
            this.panelDevices.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
    }
}
