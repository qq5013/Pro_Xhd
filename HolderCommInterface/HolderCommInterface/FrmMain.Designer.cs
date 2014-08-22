namespace HolderCommInterface
{
    partial class frmMain
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
            this.gbLog = new System.Windows.Forms.GroupBox();
            this.showinfo = new System.Windows.Forms.RichTextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.userList = new System.Windows.Forms.ListBox();
            this.btnListen = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txtSktPort = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.txtValue = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtDataAddr = new System.Windows.Forms.TextBox();
            this.btnWriteItem = new System.Windows.Forms.Button();
            this.btnReadItem = new System.Windows.Forms.Button();
            this.btnStartService = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txtPlcPort = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtPlcIP = new System.Windows.Forms.TextBox();
            this.gbLog.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbLog
            // 
            this.gbLog.Controls.Add(this.showinfo);
            this.gbLog.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.gbLog.Location = new System.Drawing.Point(0, 234);
            this.gbLog.Name = "gbLog";
            this.gbLog.Size = new System.Drawing.Size(600, 149);
            this.gbLog.TabIndex = 0;
            this.gbLog.TabStop = false;
            this.gbLog.Text = "log";
            // 
            // showinfo
            // 
            this.showinfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.showinfo.Location = new System.Drawing.Point(3, 17);
            this.showinfo.Name = "showinfo";
            this.showinfo.Size = new System.Drawing.Size(594, 129);
            this.showinfo.TabIndex = 0;
            this.showinfo.Text = "";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.userList);
            this.panel1.Controls.Add(this.btnListen);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.txtSktPort);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(227, 234);
            this.panel1.TabIndex = 1;
            // 
            // userList
            // 
            this.userList.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.userList.FormattingEnabled = true;
            this.userList.ItemHeight = 12;
            this.userList.Location = new System.Drawing.Point(0, 98);
            this.userList.Name = "userList";
            this.userList.Size = new System.Drawing.Size(227, 136);
            this.userList.TabIndex = 5;
            // 
            // btnListen
            // 
            this.btnListen.Location = new System.Drawing.Point(16, 62);
            this.btnListen.Name = "btnListen";
            this.btnListen.Size = new System.Drawing.Size(194, 23);
            this.btnListen.TabIndex = 4;
            this.btnListen.Text = "Start  listening";
            this.btnListen.UseVisualStyleBackColor = true;
            this.btnListen.Click += new System.EventHandler(this.btnListen_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "Port";
            // 
            // txtSktPort
            // 
            this.txtSktPort.Location = new System.Drawing.Point(62, 15);
            this.txtSktPort.Name = "txtSktPort";
            this.txtSktPort.Size = new System.Drawing.Size(148, 21);
            this.txtSktPort.TabIndex = 2;
            this.txtSktPort.Text = "7001";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.txtValue);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.txtDataAddr);
            this.panel2.Controls.Add(this.btnWriteItem);
            this.panel2.Controls.Add(this.btnReadItem);
            this.panel2.Controls.Add(this.btnStartService);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.txtPlcPort);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.txtPlcIP);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel2.Location = new System.Drawing.Point(227, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(227, 234);
            this.panel2.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 164);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 12);
            this.label1.TabIndex = 10;
            this.label1.Text = "Value";
            // 
            // txtValue
            // 
            this.txtValue.Location = new System.Drawing.Point(64, 161);
            this.txtValue.Name = "txtValue";
            this.txtValue.Size = new System.Drawing.Size(148, 21);
            this.txtValue.TabIndex = 9;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(11, 138);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(47, 12);
            this.label5.TabIndex = 8;
            this.label5.Text = "Dm Addr";
            // 
            // txtDataAddr
            // 
            this.txtDataAddr.Location = new System.Drawing.Point(64, 132);
            this.txtDataAddr.Name = "txtDataAddr";
            this.txtDataAddr.Size = new System.Drawing.Size(148, 21);
            this.txtDataAddr.TabIndex = 7;
            // 
            // btnWriteItem
            // 
            this.btnWriteItem.Location = new System.Drawing.Point(137, 205);
            this.btnWriteItem.Name = "btnWriteItem";
            this.btnWriteItem.Size = new System.Drawing.Size(75, 23);
            this.btnWriteItem.TabIndex = 6;
            this.btnWriteItem.Text = "Write";
            this.btnWriteItem.UseVisualStyleBackColor = true;
            this.btnWriteItem.Click += new System.EventHandler(this.btnWriteItem_Click);
            // 
            // btnReadItem
            // 
            this.btnReadItem.Location = new System.Drawing.Point(15, 205);
            this.btnReadItem.Name = "btnReadItem";
            this.btnReadItem.Size = new System.Drawing.Size(75, 23);
            this.btnReadItem.TabIndex = 5;
            this.btnReadItem.Text = "Read";
            this.btnReadItem.UseVisualStyleBackColor = true;
            this.btnReadItem.Click += new System.EventHandler(this.btnReadItem_Click);
            // 
            // btnStartService
            // 
            this.btnStartService.Location = new System.Drawing.Point(15, 63);
            this.btnStartService.Name = "btnStartService";
            this.btnStartService.Size = new System.Drawing.Size(197, 23);
            this.btnStartService.TabIndex = 4;
            this.btnStartService.Text = "Start Service";
            this.btnStartService.UseVisualStyleBackColor = true;
            this.btnStartService.Click += new System.EventHandler(this.btnStartService_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 39);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 12);
            this.label3.TabIndex = 3;
            this.label3.Text = "Port";
            // 
            // txtPlcPort
            // 
            this.txtPlcPort.Location = new System.Drawing.Point(64, 36);
            this.txtPlcPort.Name = "txtPlcPort";
            this.txtPlcPort.Size = new System.Drawing.Size(148, 21);
            this.txtPlcPort.TabIndex = 2;
            this.txtPlcPort.Text = "9600";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(11, 13);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 12);
            this.label4.TabIndex = 1;
            this.label4.Text = "IP Addr";
            // 
            // txtPlcIP
            // 
            this.txtPlcIP.Location = new System.Drawing.Point(64, 7);
            this.txtPlcIP.Name = "txtPlcIP";
            this.txtPlcIP.Size = new System.Drawing.Size(148, 21);
            this.txtPlcIP.TabIndex = 0;
            this.txtPlcIP.Text = "127.0.0.1";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(600, 383);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.gbLog);
            this.Name = "frmMain";
            this.Text = "HolderCommInterface";
            this.gbLog.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbLog;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ListBox userList;
        private System.Windows.Forms.Button btnListen;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtSktPort;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnWriteItem;
        private System.Windows.Forms.Button btnReadItem;
        private System.Windows.Forms.Button btnStartService;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtPlcPort;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtPlcIP;
        private System.Windows.Forms.RichTextBox showinfo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtValue;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtDataAddr;
    }
}

