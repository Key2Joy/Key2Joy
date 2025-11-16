namespace Key2Joy.Gui;

partial class SetupForm
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
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblDescription = new System.Windows.Forms.Label();
            this.btnInstall = new System.Windows.Forms.Button();
            this.btnCreateSystemRestorePoint = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.Location = new System.Drawing.Point(12, 9);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(290, 29);
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "Key2Joy First-Time Setup";
            // 
            // lblDescription
            // 
            this.lblDescription.AutoSize = true;
            this.lblDescription.Location = new System.Drawing.Point(14, 47);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(511, 32);
            this.lblDescription.TabIndex = 2;
            this.lblDescription.Text = "In order for Key2Joy to function correctly, you need to install the SCP Virtual B" +
    "us driver.\r\nYou might want to create a system restore point prior to installatio" +
    "n.";
            // 
            // btnInstall
            // 
            this.btnInstall.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnInstall.Image = global::Key2Joy.Gui.Properties.Resources.arrow_down;
            this.btnInstall.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnInstall.Location = new System.Drawing.Point(454, 89);
            this.btnInstall.Name = "btnInstall";
            this.btnInstall.Size = new System.Drawing.Size(102, 30);
            this.btnInstall.TabIndex = 0;
            this.btnInstall.Text = "Install";
            this.btnInstall.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnInstall.UseVisualStyleBackColor = true;
            this.btnInstall.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnCreateSystemRestorePoint
            // 
            this.btnCreateSystemRestorePoint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCreateSystemRestorePoint.Location = new System.Drawing.Point(214, 89);
            this.btnCreateSystemRestorePoint.Name = "btnCreateSystemRestorePoint";
            this.btnCreateSystemRestorePoint.Size = new System.Drawing.Size(234, 30);
            this.btnCreateSystemRestorePoint.TabIndex = 4;
            this.btnCreateSystemRestorePoint.Text = "Create System Restore Point";
            this.btnCreateSystemRestorePoint.UseVisualStyleBackColor = true;
            this.btnCreateSystemRestorePoint.Click += new System.EventHandler(this.btnCreateSystemRestorePoint_Click);
            // 
            // SetupForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(568, 131);
            this.Controls.Add(this.btnCreateSystemRestorePoint);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.lblDescription);
            this.Controls.Add(this.btnInstall);
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(586, 178);
            this.Name = "SetupForm";
            this.ShowIcon = false;
            this.Text = "Key2Joy Setup";
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button btnInstall;
    private System.Windows.Forms.Label lblTitle;
    private System.Windows.Forms.Label lblDescription;
    private System.Windows.Forms.Button btnCreateSystemRestorePoint;
}