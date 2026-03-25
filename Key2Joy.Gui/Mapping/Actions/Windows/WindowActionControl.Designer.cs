namespace Key2Joy.Gui.Mapping
{
    partial class WindowActionControl
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
            this.lblWindowTitle = new System.Windows.Forms.Label();
            this.txtWindowTitle = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.cmbWindowSelector = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblWindowTitle
            // 
            this.lblWindowTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblWindowTitle.Location = new System.Drawing.Point(0, 26);
            this.lblWindowTitle.Margin = new System.Windows.Forms.Padding(0);
            this.lblWindowTitle.Name = "lblWindowTitle";
            this.lblWindowTitle.Size = new System.Drawing.Size(84, 26);
            this.lblWindowTitle.TabIndex = 0;
            this.lblWindowTitle.Text = "Window Title:";
            this.lblWindowTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtWindowTitle
            // 
            this.txtWindowTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtWindowTitle.Location = new System.Drawing.Point(87, 29);
            this.txtWindowTitle.Name = "txtWindowTitle";
            this.txtWindowTitle.Size = new System.Drawing.Size(257, 20);
            this.txtWindowTitle.TabIndex = 1;
            this.txtWindowTitle.TextChanged += new System.EventHandler(this.TxtWindowTitle_TextChanged);
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.ColumnCount = 2;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 84F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.Controls.Add(this.lblWindowTitle, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.txtWindowTitle, 1, 1);
            this.tableLayoutPanel.Controls.Add(this.cmbWindowSelector, 0, 0);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 2;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(347, 51);
            this.tableLayoutPanel.TabIndex = 0;
            // 
            // cmbWindowSelector
            // 
            this.tableLayoutPanel.SetColumnSpan(this.cmbWindowSelector, 2);
            this.cmbWindowSelector.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmbWindowSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbWindowSelector.FormattingEnabled = true;
            this.cmbWindowSelector.Location = new System.Drawing.Point(3, 3);
            this.cmbWindowSelector.Name = "cmbWindowSelector";
            this.cmbWindowSelector.Size = new System.Drawing.Size(341, 21);
            this.cmbWindowSelector.TabIndex = 4;
            this.cmbWindowSelector.SelectedIndexChanged += new System.EventHandler(this.cmbWindowSelector_SelectedIndexChanged);
            // 
            // WindowActionControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel);
            this.Name = "WindowActionControl";
            this.Size = new System.Drawing.Size(347, 51);
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblWindowTitle;
        private System.Windows.Forms.TextBox txtWindowTitle;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.ComboBox cmbWindowSelector;
    }
}
