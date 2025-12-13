namespace Key2Joy.Gui.Mapping
{
    partial class KeyboardTriggerControl
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
            this.txtKeyBind = new System.Windows.Forms.TextBox();
            this.lblInfo = new System.Windows.Forms.Label();
            this.cmbPressState = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // txtKeyBind
            // 
            this.txtKeyBind.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtKeyBind.Location = new System.Drawing.Point(118, 6);
            this.txtKeyBind.Margin = new System.Windows.Forms.Padding(4);
            this.txtKeyBind.Name = "txtKeyBind";
            this.txtKeyBind.ReadOnly = true;
            this.txtKeyBind.Size = new System.Drawing.Size(259, 22);
            this.txtKeyBind.TabIndex = 7;
            this.txtKeyBind.Text = "(click here, then press any key to select it as the trigger)";
            this.txtKeyBind.MouseUp += new System.Windows.Forms.MouseEventHandler(this.TxtKeyBind_MouseUp);
            // 
            // lblInfo
            // 
            this.lblInfo.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblInfo.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblInfo.Location = new System.Drawing.Point(7, 6);
            this.lblInfo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(111, 21);
            this.lblInfo.TabIndex = 10;
            this.lblInfo.Text = "Keyboard Key:";
            this.lblInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cmbPressState
            // 
            this.cmbPressState.Dock = System.Windows.Forms.DockStyle.Right;
            this.cmbPressState.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPressState.FormattingEnabled = true;
            this.cmbPressState.Location = new System.Drawing.Point(377, 6);
            this.cmbPressState.Margin = new System.Windows.Forms.Padding(4);
            this.cmbPressState.Name = "cmbPressState";
            this.cmbPressState.Size = new System.Drawing.Size(117, 24);
            this.cmbPressState.TabIndex = 11;
            this.cmbPressState.SelectedIndexChanged += new System.EventHandler(this.CmbPressedState_SelectedIndexChanged);
            // 
            // KeyboardTriggerControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.txtKeyBind);
            this.Controls.Add(this.cmbPressState);
            this.Controls.Add(this.lblInfo);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "KeyboardTriggerControl";
            this.Padding = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.Size = new System.Drawing.Size(501, 33);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtKeyBind;
        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.ComboBox cmbPressState;
    }
}