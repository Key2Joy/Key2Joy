namespace Key2Joy.Gui.Mapping
{
    partial class GamePadButtonTriggerControl
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
            this.txtButtonBind = new System.Windows.Forms.TextBox();
            this.lblInfo = new System.Windows.Forms.Label();
            this.cmbPressState = new System.Windows.Forms.ComboBox();
            this.pnlGamePadIndex = new System.Windows.Forms.Panel();
            this.nudGamePadIndex = new System.Windows.Forms.NumericUpDown();
            this.lblInfoIndex = new System.Windows.Forms.Label();
            this.pnlButton = new System.Windows.Forms.Panel();
            this.lblLastGamePadLabel = new System.Windows.Forms.Label();
            this.pnlGamePadIndex.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudGamePadIndex)).BeginInit();
            this.pnlButton.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtButtonBind
            // 
            this.txtButtonBind.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtButtonBind.Location = new System.Drawing.Point(57, 6);
            this.txtButtonBind.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtButtonBind.Name = "txtButtonBind";
            this.txtButtonBind.ReadOnly = true;
            this.txtButtonBind.Size = new System.Drawing.Size(333, 22);
            this.txtButtonBind.TabIndex = 7;
            this.txtButtonBind.Text = "(click here, then press any button to select it as the trigger)";
            this.txtButtonBind.MouseUp += new System.Windows.Forms.MouseEventHandler(this.TxtKeyBind_MouseUp);
            // 
            // lblInfo
            // 
            this.lblInfo.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblInfo.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblInfo.Location = new System.Drawing.Point(0, 6);
            this.lblInfo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(57, 20);
            this.lblInfo.TabIndex = 10;
            this.lblInfo.Text = "Button:";
            this.lblInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cmbPressState
            // 
            this.cmbPressState.Dock = System.Windows.Forms.DockStyle.Right;
            this.cmbPressState.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPressState.FormattingEnabled = true;
            this.cmbPressState.Location = new System.Drawing.Point(390, 6);
            this.cmbPressState.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmbPressState.Name = "cmbPressState";
            this.cmbPressState.Size = new System.Drawing.Size(97, 24);
            this.cmbPressState.TabIndex = 11;
            this.cmbPressState.SelectedIndexChanged += new System.EventHandler(this.CmbPressedState_SelectedIndexChanged);
            // 
            // pnlGamePadIndex
            // 
            this.pnlGamePadIndex.Controls.Add(this.nudGamePadIndex);
            this.pnlGamePadIndex.Controls.Add(this.lblInfoIndex);
            this.pnlGamePadIndex.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlGamePadIndex.Location = new System.Drawing.Point(7, 6);
            this.pnlGamePadIndex.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pnlGamePadIndex.Name = "pnlGamePadIndex";
            this.pnlGamePadIndex.Size = new System.Drawing.Size(487, 25);
            this.pnlGamePadIndex.TabIndex = 12;
            // 
            // nudGamePadIndex
            // 
            this.nudGamePadIndex.Dock = System.Windows.Forms.DockStyle.Fill;
            this.nudGamePadIndex.Location = new System.Drawing.Point(92, 0);
            this.nudGamePadIndex.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.nudGamePadIndex.Name = "nudGamePadIndex";
            this.nudGamePadIndex.Size = new System.Drawing.Size(395, 22);
            this.nudGamePadIndex.TabIndex = 10;
            this.nudGamePadIndex.ValueChanged += new System.EventHandler(this.NudGamePadIndex_ValueChanged);
            // 
            // lblInfoIndex
            // 
            this.lblInfoIndex.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblInfoIndex.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblInfoIndex.Location = new System.Drawing.Point(0, 0);
            this.lblInfoIndex.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblInfoIndex.Name = "lblInfoIndex";
            this.lblInfoIndex.Size = new System.Drawing.Size(92, 25);
            this.lblInfoIndex.TabIndex = 9;
            this.lblInfoIndex.Text = "GamePad #:";
            this.lblInfoIndex.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pnlButton
            // 
            this.pnlButton.Controls.Add(this.txtButtonBind);
            this.pnlButton.Controls.Add(this.cmbPressState);
            this.pnlButton.Controls.Add(this.lblInfo);
            this.pnlButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlButton.Location = new System.Drawing.Point(7, 31);
            this.pnlButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pnlButton.Name = "pnlButton";
            this.pnlButton.Padding = new System.Windows.Forms.Padding(0, 6, 0, 6);
            this.pnlButton.Size = new System.Drawing.Size(487, 32);
            this.pnlButton.TabIndex = 14;
            // 
            // lblLastGamePadLabel
            // 
            this.lblLastGamePadLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblLastGamePadLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLastGamePadLabel.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblLastGamePadLabel.Location = new System.Drawing.Point(7, 63);
            this.lblLastGamePadLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblLastGamePadLabel.Name = "lblLastGamePadLabel";
            this.lblLastGamePadLabel.Size = new System.Drawing.Size(487, 18);
            this.lblLastGamePadLabel.TabIndex = 15;
            this.lblLastGamePadLabel.Text = "Last GamePad used was #: <none>";
            // 
            // GamePadButtonTriggerControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.lblLastGamePadLabel);
            this.Controls.Add(this.pnlButton);
            this.Controls.Add(this.pnlGamePadIndex);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "GamePadButtonTriggerControl";
            this.Padding = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.Size = new System.Drawing.Size(501, 87);
            this.pnlGamePadIndex.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nudGamePadIndex)).EndInit();
            this.pnlButton.ResumeLayout(false);
            this.pnlButton.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox txtButtonBind;
        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.ComboBox cmbPressState;
        private System.Windows.Forms.Panel pnlGamePadIndex;
        private System.Windows.Forms.NumericUpDown nudGamePadIndex;
        private System.Windows.Forms.Label lblInfoIndex;
        private System.Windows.Forms.Panel pnlButton;
        private System.Windows.Forms.Label lblLastGamePadLabel;
    }
}
