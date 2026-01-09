namespace Key2Joy.Gui;

partial class DeviceControl
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
            this.picImage = new System.Windows.Forms.PictureBox();
            this.lblDevice = new System.Windows.Forms.Label();
            this.lblIndex = new System.Windows.Forms.Label();
            this.pnlDevice = new System.Windows.Forms.Panel();
            this.pnlSeparator = new System.Windows.Forms.Panel();
            this.pnlHeader = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.picImage)).BeginInit();
            this.pnlDevice.SuspendLayout();
            this.pnlHeader.SuspendLayout();
            this.SuspendLayout();
            // 
            // picImage
            // 
            this.picImage.Dock = System.Windows.Forms.DockStyle.Left;
            this.picImage.Image = global::Key2Joy.Gui.Properties.Resources.disconnect;
            this.picImage.Location = new System.Drawing.Point(0, 0);
            this.picImage.Margin = new System.Windows.Forms.Padding(4);
            this.picImage.Name = "picImage";
            this.picImage.Size = new System.Drawing.Size(39, 26);
            this.picImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.picImage.TabIndex = 0;
            this.picImage.TabStop = false;
            // 
            // lblDevice
            // 
            this.lblDevice.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblDevice.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblDevice.Location = new System.Drawing.Point(0, 29);
            this.lblDevice.Name = "lblDevice";
            this.lblDevice.Size = new System.Drawing.Size(101, 21);
            this.lblDevice.TabIndex = 1;
            this.lblDevice.Text = "Device Name";
            this.lblDevice.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblIndex
            // 
            this.lblIndex.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblIndex.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblIndex.Location = new System.Drawing.Point(39, 0);
            this.lblIndex.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblIndex.Name = "lblIndex";
            this.lblIndex.Size = new System.Drawing.Size(62, 26);
            this.lblIndex.TabIndex = 2;
            this.lblIndex.Text = "?";
            this.lblIndex.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pnlDevice
            // 
            this.pnlDevice.BackColor = System.Drawing.Color.Transparent;
            this.pnlDevice.Controls.Add(this.pnlHeader);
            this.pnlDevice.Controls.Add(this.lblDevice);
            this.pnlDevice.Controls.Add(this.pnlSeparator);
            this.pnlDevice.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlDevice.Location = new System.Drawing.Point(0, 0);
            this.pnlDevice.Name = "pnlDevice";
            this.pnlDevice.Size = new System.Drawing.Size(101, 51);
            this.pnlDevice.TabIndex = 3;
            // 
            // pnlSeparator
            // 
            this.pnlSeparator.BackColor = System.Drawing.Color.DarkGray;
            this.pnlSeparator.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlSeparator.Location = new System.Drawing.Point(0, 50);
            this.pnlSeparator.Name = "pnlSeparator";
            this.pnlSeparator.Size = new System.Drawing.Size(101, 1);
            this.pnlSeparator.TabIndex = 3;
            // 
            // pnlHeader
            // 
            this.pnlHeader.Controls.Add(this.lblIndex);
            this.pnlHeader.Controls.Add(this.picImage);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(101, 26);
            this.pnlHeader.TabIndex = 4;
            // 
            // DeviceControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.pnlDevice);
            this.Name = "DeviceControl";
            this.Size = new System.Drawing.Size(101, 51);
            ((System.ComponentModel.ISupportInitialize)(this.picImage)).EndInit();
            this.pnlDevice.ResumeLayout(false);
            this.pnlHeader.ResumeLayout(false);
            this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.PictureBox picImage;
    private System.Windows.Forms.Label lblDevice;
    private System.Windows.Forms.Label lblIndex;
    private System.Windows.Forms.Panel pnlDevice;
    private System.Windows.Forms.Panel pnlSeparator;
    private System.Windows.Forms.Panel pnlHeader;
}
