namespace Key2Joy.Gui;

partial class MappingControl
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
            this.grpAction = new System.Windows.Forms.GroupBox();
            this.actionControl = new Key2Joy.Gui.Mapping.ActionControl();
            this.btnSaveMapping = new System.Windows.Forms.Button();
            this.grpTrigger = new System.Windows.Forms.GroupBox();
            this.triggerControl = new Key2Joy.Gui.Mapping.TriggerControl();
            this.chkCreateReverseMapping = new System.Windows.Forms.CheckBox();
            this.grpAction.SuspendLayout();
            this.grpTrigger.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpAction
            // 
            this.grpAction.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpAction.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.grpAction.Controls.Add(this.actionControl);
            this.grpAction.Location = new System.Drawing.Point(538, 4);
            this.grpAction.Margin = new System.Windows.Forms.Padding(4);
            this.grpAction.Name = "grpAction";
            this.grpAction.Padding = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.grpAction.Size = new System.Drawing.Size(500, 196);
            this.grpAction.TabIndex = 88;
            this.grpAction.TabStop = false;
            this.grpAction.Text = "Action";
            // 
            // actionControl
            // 
            this.actionControl.AutoSize = true;
            this.actionControl.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.actionControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.actionControl.IsTopLevel = false;
            this.actionControl.Location = new System.Drawing.Point(7, 21);
            this.actionControl.Margin = new System.Windows.Forms.Padding(5);
            this.actionControl.MinimumSize = new System.Drawing.Size(400, 39);
            this.actionControl.Name = "actionControl";
            this.actionControl.Padding = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.actionControl.Size = new System.Drawing.Size(486, 169);
            this.actionControl.TabIndex = 0;
            // 
            // btnSaveMapping
            // 
            this.btnSaveMapping.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveMapping.Location = new System.Drawing.Point(910, 208);
            this.btnSaveMapping.Margin = new System.Windows.Forms.Padding(4);
            this.btnSaveMapping.Name = "btnSaveMapping";
            this.btnSaveMapping.Size = new System.Drawing.Size(128, 33);
            this.btnSaveMapping.TabIndex = 91;
            this.btnSaveMapping.Text = "Apply";
            this.btnSaveMapping.UseVisualStyleBackColor = true;
            this.btnSaveMapping.Click += new System.EventHandler(this.btnSaveMapping_Click);
            // 
            // grpTrigger
            // 
            this.grpTrigger.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.grpTrigger.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.grpTrigger.Controls.Add(this.triggerControl);
            this.grpTrigger.Location = new System.Drawing.Point(5, 4);
            this.grpTrigger.Margin = new System.Windows.Forms.Padding(4);
            this.grpTrigger.Name = "grpTrigger";
            this.grpTrigger.Padding = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.grpTrigger.Size = new System.Drawing.Size(525, 196);
            this.grpTrigger.TabIndex = 86;
            this.grpTrigger.TabStop = false;
            this.grpTrigger.Text = "Trigger";
            // 
            // triggerControl
            // 
            this.triggerControl.AutoSize = true;
            this.triggerControl.BackColor = System.Drawing.SystemColors.Control;
            this.triggerControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.triggerControl.IsTopLevel = false;
            this.triggerControl.Location = new System.Drawing.Point(7, 21);
            this.triggerControl.Margin = new System.Windows.Forms.Padding(5);
            this.triggerControl.Name = "triggerControl";
            this.triggerControl.Padding = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.triggerControl.Size = new System.Drawing.Size(511, 169);
            this.triggerControl.TabIndex = 0;
            // 
            // chkCreateReverseMapping
            // 
            this.chkCreateReverseMapping.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.chkCreateReverseMapping.AutoSize = true;
            this.chkCreateReverseMapping.Location = new System.Drawing.Point(708, 208);
            this.chkCreateReverseMapping.Margin = new System.Windows.Forms.Padding(4);
            this.chkCreateReverseMapping.Name = "chkCreateReverseMapping";
            this.chkCreateReverseMapping.Padding = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.chkCreateReverseMapping.Size = new System.Drawing.Size(194, 32);
            this.chkCreateReverseMapping.TabIndex = 0;
            this.chkCreateReverseMapping.Text = "Create Reverse Mapping";
            this.chkCreateReverseMapping.UseVisualStyleBackColor = true;
            this.chkCreateReverseMapping.Click += new System.EventHandler(this.chkCreateReverseMapping_Click);
            // 
            // MappingControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grpTrigger);
            this.Controls.Add(this.grpAction);
            this.Controls.Add(this.chkCreateReverseMapping);
            this.Controls.Add(this.btnSaveMapping);
            this.Name = "MappingControl";
            this.Size = new System.Drawing.Size(1045, 250);
            this.grpAction.ResumeLayout(false);
            this.grpAction.PerformLayout();
            this.grpTrigger.ResumeLayout(false);
            this.grpTrigger.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion
    private System.Windows.Forms.GroupBox grpAction;
    private Mapping.ActionControl actionControl;
    private System.Windows.Forms.Button btnSaveMapping;
    private System.Windows.Forms.GroupBox grpTrigger;
    private Mapping.TriggerControl triggerControl;
    private System.Windows.Forms.CheckBox chkCreateReverseMapping;
}
