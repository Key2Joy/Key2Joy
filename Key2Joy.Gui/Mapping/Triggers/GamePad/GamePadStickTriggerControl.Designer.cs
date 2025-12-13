namespace Key2Joy.Gui.Mapping
{
    partial class GamePadStickTriggerControl
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
            this.lblInfoSide = new System.Windows.Forms.Label();
            this.pnlStickSide = new System.Windows.Forms.Panel();
            this.cmbStickSide = new System.Windows.Forms.ComboBox();
            this.pnlGamePadIndex = new System.Windows.Forms.Panel();
            this.nudGamePadIndex = new System.Windows.Forms.NumericUpDown();
            this.lblInfoIndex = new System.Windows.Forms.Label();
            this.pnlDeadzone = new System.Windows.Forms.Panel();
            this.pnlDeadzoneConfig = new System.Windows.Forms.Panel();
            this.nudDeadzoneY = new System.Windows.Forms.NumericUpDown();
            this.lblInfoDeadzoneY = new System.Windows.Forms.Label();
            this.nudDeadzoneX = new System.Windows.Forms.NumericUpDown();
            this.lblInfoDeadzoneX = new System.Windows.Forms.Label();
            this.chkOverrideDeadzone = new System.Windows.Forms.CheckBox();
            this.pnlStickSide.SuspendLayout();
            this.pnlGamePadIndex.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudGamePadIndex)).BeginInit();
            this.pnlDeadzone.SuspendLayout();
            this.pnlDeadzoneConfig.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudDeadzoneY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDeadzoneX)).BeginInit();
            this.SuspendLayout();
            // 
            // lblInfoSide
            // 
            this.lblInfoSide.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblInfoSide.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblInfoSide.Location = new System.Drawing.Point(0, 6);
            this.lblInfoSide.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblInfoSide.Name = "lblInfoSide";
            this.lblInfoSide.Size = new System.Drawing.Size(79, 20);
            this.lblInfoSide.TabIndex = 8;
            this.lblInfoSide.Text = "Stick Side:";
            this.lblInfoSide.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pnlStickSide
            // 
            this.pnlStickSide.Controls.Add(this.cmbStickSide);
            this.pnlStickSide.Controls.Add(this.lblInfoSide);
            this.pnlStickSide.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlStickSide.Location = new System.Drawing.Point(7, 31);
            this.pnlStickSide.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pnlStickSide.Name = "pnlStickSide";
            this.pnlStickSide.Padding = new System.Windows.Forms.Padding(0, 6, 0, 6);
            this.pnlStickSide.Size = new System.Drawing.Size(395, 32);
            this.pnlStickSide.TabIndex = 9;
            // 
            // cmbStickSide
            // 
            this.cmbStickSide.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmbStickSide.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbStickSide.FormattingEnabled = true;
            this.cmbStickSide.Location = new System.Drawing.Point(79, 6);
            this.cmbStickSide.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmbStickSide.Name = "cmbStickSide";
            this.cmbStickSide.Size = new System.Drawing.Size(316, 24);
            this.cmbStickSide.TabIndex = 9;
            this.cmbStickSide.SelectedIndexChanged += new System.EventHandler(this.CmbStickSide_SelectedIndexChanged);
            // 
            // pnlGamePadIndex
            // 
            this.pnlGamePadIndex.Controls.Add(this.nudGamePadIndex);
            this.pnlGamePadIndex.Controls.Add(this.lblInfoIndex);
            this.pnlGamePadIndex.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlGamePadIndex.Location = new System.Drawing.Point(7, 6);
            this.pnlGamePadIndex.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pnlGamePadIndex.Name = "pnlGamePadIndex";
            this.pnlGamePadIndex.Size = new System.Drawing.Size(395, 25);
            this.pnlGamePadIndex.TabIndex = 10;
            // 
            // nudGamePadIndex
            // 
            this.nudGamePadIndex.Dock = System.Windows.Forms.DockStyle.Fill;
            this.nudGamePadIndex.Location = new System.Drawing.Point(92, 0);
            this.nudGamePadIndex.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.nudGamePadIndex.Name = "nudGamePadIndex";
            this.nudGamePadIndex.Size = new System.Drawing.Size(303, 22);
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
            // pnlDeadzone
            // 
            this.pnlDeadzone.Controls.Add(this.pnlDeadzoneConfig);
            this.pnlDeadzone.Controls.Add(this.chkOverrideDeadzone);
            this.pnlDeadzone.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlDeadzone.Location = new System.Drawing.Point(7, 63);
            this.pnlDeadzone.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pnlDeadzone.Name = "pnlDeadzone";
            this.pnlDeadzone.Size = new System.Drawing.Size(395, 49);
            this.pnlDeadzone.TabIndex = 11;
            // 
            // pnlDeadzoneConfig
            // 
            this.pnlDeadzoneConfig.Controls.Add(this.nudDeadzoneY);
            this.pnlDeadzoneConfig.Controls.Add(this.lblInfoDeadzoneY);
            this.pnlDeadzoneConfig.Controls.Add(this.nudDeadzoneX);
            this.pnlDeadzoneConfig.Controls.Add(this.lblInfoDeadzoneX);
            this.pnlDeadzoneConfig.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlDeadzoneConfig.Location = new System.Drawing.Point(0, 20);
            this.pnlDeadzoneConfig.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pnlDeadzoneConfig.Name = "pnlDeadzoneConfig";
            this.pnlDeadzoneConfig.Size = new System.Drawing.Size(395, 25);
            this.pnlDeadzoneConfig.TabIndex = 12;
            // 
            // nudDeadzoneY
            // 
            this.nudDeadzoneY.DecimalPlaces = 4;
            this.nudDeadzoneY.Dock = System.Windows.Forms.DockStyle.Fill;
            this.nudDeadzoneY.Location = new System.Drawing.Point(237, 0);
            this.nudDeadzoneY.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.nudDeadzoneY.Name = "nudDeadzoneY";
            this.nudDeadzoneY.Size = new System.Drawing.Size(158, 22);
            this.nudDeadzoneY.TabIndex = 11;
            this.nudDeadzoneY.ValueChanged += new System.EventHandler(this.NudDeadzoneY_ValueChanged);
            // 
            // lblInfoDeadzoneY
            // 
            this.lblInfoDeadzoneY.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblInfoDeadzoneY.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblInfoDeadzoneY.Location = new System.Drawing.Point(186, 0);
            this.lblInfoDeadzoneY.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblInfoDeadzoneY.Name = "lblInfoDeadzoneY";
            this.lblInfoDeadzoneY.Size = new System.Drawing.Size(51, 25);
            this.lblInfoDeadzoneY.TabIndex = 10;
            this.lblInfoDeadzoneY.Text = "Y:";
            this.lblInfoDeadzoneY.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // nudDeadzoneX
            // 
            this.nudDeadzoneX.DecimalPlaces = 4;
            this.nudDeadzoneX.Dock = System.Windows.Forms.DockStyle.Left;
            this.nudDeadzoneX.Location = new System.Drawing.Point(35, 0);
            this.nudDeadzoneX.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.nudDeadzoneX.Name = "nudDeadzoneX";
            this.nudDeadzoneX.Size = new System.Drawing.Size(151, 22);
            this.nudDeadzoneX.TabIndex = 1;
            this.nudDeadzoneX.ValueChanged += new System.EventHandler(this.NudDeadzoneX_ValueChanged);
            // 
            // lblInfoDeadzoneX
            // 
            this.lblInfoDeadzoneX.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblInfoDeadzoneX.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblInfoDeadzoneX.Location = new System.Drawing.Point(0, 0);
            this.lblInfoDeadzoneX.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblInfoDeadzoneX.Name = "lblInfoDeadzoneX";
            this.lblInfoDeadzoneX.Size = new System.Drawing.Size(35, 25);
            this.lblInfoDeadzoneX.TabIndex = 9;
            this.lblInfoDeadzoneX.Text = "X:";
            this.lblInfoDeadzoneX.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // chkOverrideDeadzone
            // 
            this.chkOverrideDeadzone.AutoSize = true;
            this.chkOverrideDeadzone.Dock = System.Windows.Forms.DockStyle.Top;
            this.chkOverrideDeadzone.Location = new System.Drawing.Point(0, 0);
            this.chkOverrideDeadzone.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkOverrideDeadzone.Name = "chkOverrideDeadzone";
            this.chkOverrideDeadzone.Size = new System.Drawing.Size(395, 20);
            this.chkOverrideDeadzone.TabIndex = 0;
            this.chkOverrideDeadzone.Text = "Override default deadzone:";
            this.chkOverrideDeadzone.UseVisualStyleBackColor = true;
            this.chkOverrideDeadzone.CheckedChanged += new System.EventHandler(this.ChkOverrideDeadzone_CheckedChanged);
            // 
            // GamePadStickTriggerControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.pnlDeadzone);
            this.Controls.Add(this.pnlStickSide);
            this.Controls.Add(this.pnlGamePadIndex);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "GamePadStickTriggerControl";
            this.Padding = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.Size = new System.Drawing.Size(409, 114);
            this.pnlStickSide.ResumeLayout(false);
            this.pnlGamePadIndex.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nudGamePadIndex)).EndInit();
            this.pnlDeadzone.ResumeLayout(false);
            this.pnlDeadzone.PerformLayout();
            this.pnlDeadzoneConfig.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nudDeadzoneY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDeadzoneX)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label lblInfoSide;
        private System.Windows.Forms.Panel pnlStickSide;
        private System.Windows.Forms.ComboBox cmbStickSide;
        private System.Windows.Forms.Panel pnlGamePadIndex;
        private System.Windows.Forms.Label lblInfoIndex;
        private System.Windows.Forms.NumericUpDown nudGamePadIndex;
        private System.Windows.Forms.Panel pnlDeadzone;
        private System.Windows.Forms.NumericUpDown nudDeadzoneY;
        private System.Windows.Forms.Label lblInfoDeadzoneY;
        private System.Windows.Forms.NumericUpDown nudDeadzoneX;
        private System.Windows.Forms.Label lblInfoDeadzoneX;
        private System.Windows.Forms.CheckBox chkOverrideDeadzone;
        private System.Windows.Forms.Panel pnlDeadzoneConfig;
    }
}
