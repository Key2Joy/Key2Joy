namespace Key2Joy.Gui.Mapping.Actions.Graphics;

partial class SetCursorActionControl
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
            this.cmbCursor = new System.Windows.Forms.ComboBox();
            this.lblInfo = new System.Windows.Forms.Label();
            this.pnlFileInput = new System.Windows.Forms.Panel();
            this.txtFilePath = new System.Windows.Forms.TextBox();
            this.btnBrowseFile = new System.Windows.Forms.Button();
            this.pnlFileInput.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmbCursor
            // 
            this.cmbCursor.Dock = System.Windows.Forms.DockStyle.Top;
            this.cmbCursor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCursor.FormattingEnabled = true;
            this.cmbCursor.Location = new System.Drawing.Point(84, 0);
            this.cmbCursor.Name = "cmbCursor";
            this.cmbCursor.Size = new System.Drawing.Size(185, 21);
            this.cmbCursor.TabIndex = 11;
            this.cmbCursor.SelectedIndexChanged += new System.EventHandler(this.CmbCursor_SelectedIndexChanged);
            // 
            // lblInfo
            // 
            this.lblInfo.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblInfo.Location = new System.Drawing.Point(0, 0);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(84, 43);
            this.lblInfo.TabIndex = 12;
            this.lblInfo.Text = "Cursor:";
            this.lblInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pnlFileInput
            // 
            this.pnlFileInput.Controls.Add(this.txtFilePath);
            this.pnlFileInput.Controls.Add(this.btnBrowseFile);
            this.pnlFileInput.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlFileInput.Location = new System.Drawing.Point(84, 21);
            this.pnlFileInput.Name = "pnlFileInput";
            this.pnlFileInput.Size = new System.Drawing.Size(185, 21);
            this.pnlFileInput.TabIndex = 17;
            // 
            // txtFilePath
            // 
            this.txtFilePath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtFilePath.Location = new System.Drawing.Point(0, 0);
            this.txtFilePath.Name = "txtFilePath";
            this.txtFilePath.Size = new System.Drawing.Size(121, 20);
            this.txtFilePath.TabIndex = 0;
            // 
            // btnBrowseFile
            // 
            this.btnBrowseFile.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnBrowseFile.Location = new System.Drawing.Point(121, 0);
            this.btnBrowseFile.Name = "btnBrowseFile";
            this.btnBrowseFile.Size = new System.Drawing.Size(64, 21);
            this.btnBrowseFile.TabIndex = 1;
            this.btnBrowseFile.Text = "Browse...";
            this.btnBrowseFile.UseVisualStyleBackColor = true;
            this.btnBrowseFile.Click += new System.EventHandler(this.btnBrowseFile_Click);
            // 
            // SetCursorActionControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlFileInput);
            this.Controls.Add(this.cmbCursor);
            this.Controls.Add(this.lblInfo);
            this.Name = "SetCursorActionControl";
            this.Size = new System.Drawing.Size(269, 43);
            this.pnlFileInput.ResumeLayout(false);
            this.pnlFileInput.PerformLayout();
            this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.ComboBox cmbCursor;
    private System.Windows.Forms.Label lblInfo;
    private System.Windows.Forms.Panel pnlFileInput;
    private System.Windows.Forms.TextBox txtFilePath;
    private System.Windows.Forms.Button btnBrowseFile;
}
