namespace Key2Joy.Gui;

partial class DeviceListControl
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

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
            this.pnlDevices = new System.Windows.Forms.Panel();
            this.lblDevices = new System.Windows.Forms.Label();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.lblListPlaceholder = new System.Windows.Forms.Label();
            this.pnlContainer = new System.Windows.Forms.Panel();
            this.pnlDeviceListContainer = new System.Windows.Forms.Panel();
            this.pnlContainer.SuspendLayout();
            this.pnlDeviceListContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlDevices
            // 
            this.pnlDevices.AutoSize = true;
            this.pnlDevices.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlDevices.Location = new System.Drawing.Point(0, 486);
            this.pnlDevices.Margin = new System.Windows.Forms.Padding(4);
            this.pnlDevices.Name = "pnlDevices";
            this.pnlDevices.Size = new System.Drawing.Size(88, 0);
            this.pnlDevices.TabIndex = 0;
            // 
            // lblDevices
            // 
            this.lblDevices.BackColor = System.Drawing.Color.Transparent;
            this.lblDevices.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblDevices.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDevices.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblDevices.Location = new System.Drawing.Point(5, 0);
            this.lblDevices.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDevices.Name = "lblDevices";
            this.lblDevices.Size = new System.Drawing.Size(88, 58);
            this.lblDevices.TabIndex = 0;
            this.lblDevices.Text = "Connected Devices";
            this.lblDevices.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnRefresh.Location = new System.Drawing.Point(5, 58);
            this.btnRefresh.Margin = new System.Windows.Forms.Padding(4);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(88, 35);
            this.btnRefresh.TabIndex = 2;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.BtnRefresh_Click);
            // 
            // lblListPlaceholder
            // 
            this.lblListPlaceholder.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblListPlaceholder.ForeColor = System.Drawing.SystemColors.WindowText;
            this.lblListPlaceholder.Location = new System.Drawing.Point(0, 5);
            this.lblListPlaceholder.Name = "lblListPlaceholder";
            this.lblListPlaceholder.Size = new System.Drawing.Size(88, 481);
            this.lblListPlaceholder.TabIndex = 3;
            this.lblListPlaceholder.Text = "No devices found.\r\n\r\nTry connecting the Key2Joy device.";
            this.lblListPlaceholder.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // pnlContainer
            // 
            this.pnlContainer.Controls.Add(this.pnlDeviceListContainer);
            this.pnlContainer.Controls.Add(this.btnRefresh);
            this.pnlContainer.Controls.Add(this.lblDevices);
            this.pnlContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContainer.Location = new System.Drawing.Point(0, 0);
            this.pnlContainer.Name = "pnlContainer";
            this.pnlContainer.Padding = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.pnlContainer.Size = new System.Drawing.Size(98, 579);
            this.pnlContainer.TabIndex = 4;
            // 
            // pnlDeviceListContainer
            // 
            this.pnlDeviceListContainer.AutoSize = true;
            this.pnlDeviceListContainer.Controls.Add(this.pnlDevices);
            this.pnlDeviceListContainer.Controls.Add(this.lblListPlaceholder);
            this.pnlDeviceListContainer.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlDeviceListContainer.Location = new System.Drawing.Point(5, 93);
            this.pnlDeviceListContainer.Name = "pnlDeviceListContainer";
            this.pnlDeviceListContainer.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.pnlDeviceListContainer.Size = new System.Drawing.Size(88, 486);
            this.pnlDeviceListContainer.TabIndex = 4;
            // 
            // DeviceListControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.pnlContainer);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "DeviceListControl";
            this.Size = new System.Drawing.Size(98, 579);
            this.pnlContainer.ResumeLayout(false);
            this.pnlContainer.PerformLayout();
            this.pnlDeviceListContainer.ResumeLayout(false);
            this.pnlDeviceListContainer.PerformLayout();
            this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Panel pnlDevices;
    private System.Windows.Forms.Label lblDevices;
    private System.Windows.Forms.Button btnRefresh;
    private System.Windows.Forms.Label lblListPlaceholder;
    private System.Windows.Forms.Panel pnlContainer;
    private System.Windows.Forms.Panel pnlDeviceListContainer;
}
