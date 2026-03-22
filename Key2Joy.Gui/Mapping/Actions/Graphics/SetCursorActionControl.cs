using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Key2Joy.Contracts.Mapping;
using Key2Joy.Contracts.Mapping.Actions;
using Key2Joy.Mapping.Actions.Graphics;

namespace Key2Joy.Gui.Mapping.Actions.Graphics;

[MappingControl(
    ForType = typeof(SetCursorAction),
    ImageResourceName = "cursor"
)]
public partial class SetCursorActionControl : UserControl, IActionOptionsControl
{
    public event EventHandler OptionsChanged;

    public SetCursorActionControl()
    {
        this.InitializeComponent();

        this.pnlFileInput.Enabled = false;

        List<string> cursors = new();

        foreach (var entry in SetCursorAction.SystemCursorIds)
        {
            cursors.Add(entry.Key);
        }

        this.cmbCursor.DataSource = cursors;
    }

    public void Select(AbstractAction action)
    {
        var thisAction = (SetCursorAction)action;

        this.txtFilePath.Text = thisAction.CursorFilePath;
        this.cmbCursor.SelectedItem = thisAction.CursorName;
    }

    public void Setup(AbstractAction action)
    {
        var thisAction = (SetCursorAction)action;

        thisAction.CursorFilePath = this.txtFilePath.Text;
        thisAction.CursorName = (string)this.cmbCursor.SelectedItem;
    }
    public bool CanMappingSave(AbstractAction action) => true;

    private void CmbCursor_SelectedIndexChanged(object sender, EventArgs e)
    {
        OptionsChanged?.Invoke(this, EventArgs.Empty);

        switch (this.cmbCursor.SelectedItem)
        {
            case "FILE":
                this.pnlFileInput.Enabled = true;
                break;
            default:
                break;
        }
    }

    private void btnBrowseFile_Click(object sender, EventArgs e)
    {
        OpenFileDialog filePicker = new();

        if (filePicker.ShowDialog() != DialogResult.OK)
        {
            return;
        }

        var file = filePicker.FileName;
        this.txtFilePath.Text = file;
        OptionsChanged?.Invoke(this, EventArgs.Empty);
    }
}
