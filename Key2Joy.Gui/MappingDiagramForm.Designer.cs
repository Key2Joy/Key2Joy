namespace Key2Joy.Gui;

partial class MappingDiagramForm
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
        this.SuspendLayout();

        this.mappingDiagramControl = new MappingDiagramControl();
        this.Controls.Add(this.mappingDiagramControl);

        // 
        // MappingDiagramForm
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.AutoScroll = true;
        this.ClientSize = new System.Drawing.Size(1100, 740);
        this.Name = "MappingDiagramForm";
        this.ShowIcon = false;
        this.Text = "Controller Diagram";
        this.TopMost = true;
        this.ResumeLayout(false);
        //
        // mappingDiagramControl
        //
        this.mappingDiagramControl.Location = new System.Drawing.Point(0, 0);
        this.mappingDiagramControl.Dock = System.Windows.Forms.DockStyle.Fill;
        this.mappingDiagramControl.Name = "mappingDiagramControl";
        this.mappingDiagramControl.TabIndex = 0;
    }

    private MappingDiagramControl mappingDiagramControl;

    #endregion
}
