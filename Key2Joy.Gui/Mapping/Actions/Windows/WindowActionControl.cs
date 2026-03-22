using System;
using System.Windows.Forms;
using Key2Joy.Contracts.Mapping;
using Key2Joy.Contracts.Mapping.Actions;
using Key2Joy.Gui.Util;
using Key2Joy.Mapping.Actions.Windows;

namespace Key2Joy.Gui.Mapping;

[MappingControl(
    ForTypes = new[] { typeof(WindowMinimizeAction), typeof(WindowFocusAction) },
    ImageResourceName = "application_xp_terminal"
)]
public partial class WindowActionControl : UserControl, IActionOptionsControl
{
    public event EventHandler OptionsChanged;

    public WindowActionControl()
    {
        this.InitializeComponent();

        // Get all top-level windows and populate the selector
        var windows = WindowUtilities.GetTopLevelWindows();

        foreach (var window in windows)
        {
            this.cmbWindowSelector.Items.Add(window.Title);
        }
    }

    public void Select(AbstractAction action)
    {
        if (action is WindowMinimizeAction minimizeAction)
        {
            this.txtWindowTitle.Text = minimizeAction.WindowTitle;
        }
        else if (action is WindowFocusAction focusAction)
        {
            this.txtWindowTitle.Text = focusAction.WindowTitle;
        }
    }

    public void Setup(AbstractAction action)
    {
        if (action is WindowMinimizeAction minimizeAction)
        {
            minimizeAction.WindowTitle = this.txtWindowTitle.Text;
        }
        else if (action is WindowFocusAction focusAction)
        {
            focusAction.WindowTitle = this.txtWindowTitle.Text;
        }
    }

    public bool CanMappingSave(AbstractAction action) => !string.IsNullOrWhiteSpace(this.txtWindowTitle.Text);

    private void TxtWindowTitle_TextChanged(object sender, EventArgs e) => OptionsChanged?.Invoke(this, EventArgs.Empty);

    private void TxtClassName_TextChanged(object sender, EventArgs e) => OptionsChanged?.Invoke(this, EventArgs.Empty);

    private void cmbWindowSelector_SelectedIndexChanged(object sender, EventArgs e)
    {
        var selectedTitle = this.cmbWindowSelector.SelectedItem as string;

        if (!string.IsNullOrEmpty(selectedTitle))
        {
            this.txtWindowTitle.Text = selectedTitle;
        }
    }
}
