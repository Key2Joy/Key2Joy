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

    private WindowUtilities.WindowInfo selectedWindow;

    public WindowActionControl()
    {
        this.InitializeComponent();

        // Get all top-level windows and populate the selector
        var windows = WindowUtilities.GetTopLevelWindows();

        foreach (var window in windows)
        {
            this.cmbWindowSelector.Items.Add(window);
        }
    }

    public void Select(AbstractAction action)
    {
        if (action is not WindowAction windowAction)
        {
            return;
        }

        foreach (var item in this.cmbWindowSelector.Items)
        {
            if (item is not WindowUtilities.WindowInfo window)
            {
                continue;
            }

            if (window.Title == windowAction.WindowTitle
                && window.Executable == windowAction.Executable)
            {
                // Assign before setting Text so TxtWindowTitle_TextChanged does not clear it
                this.selectedWindow = window;
                break;
            }
        }

        this.txtWindowTitle.Text = WindowUtilities.FormatExecutableAndTitle(
            windowAction.Executable,
            windowAction.WindowTitle
        );
    }

    public void Setup(AbstractAction action)
    {
        if (action is not WindowAction windowAction)
        {
            return;
        }

        if (this.selectedWindow != null)
        {
            windowAction.WindowTitle = this.selectedWindow.Title;
            windowAction.Executable = this.selectedWindow.Executable;
            return;
        }

        var text = this.txtWindowTitle.Text;
        var separatorIndex = text.IndexOf(WindowUtilities.TITLE_SEPARATOR, StringComparison.Ordinal);

        if (separatorIndex > 0)
        {
            windowAction.Executable = text.Substring(0, separatorIndex);
            windowAction.WindowTitle = text.Substring(separatorIndex + WindowUtilities.TITLE_SEPARATOR.Length);
        }
        else
        {
            windowAction.Executable = null;
            windowAction.WindowTitle = text;
        }
    }

    public bool CanMappingSave(AbstractAction action) => !string.IsNullOrWhiteSpace(this.txtWindowTitle.Text);

    private void TxtWindowTitle_TextChanged(object sender, EventArgs e)
    {
        this.selectedWindow = null;
        OptionsChanged?.Invoke(this, EventArgs.Empty);
    }

    private void cmbWindowSelector_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (this.cmbWindowSelector.SelectedItem is not WindowUtilities.WindowInfo item)
        {
            return;
        }

        this.selectedWindow = item;
        this.txtWindowTitle.Text = WindowUtilities.FormatExecutableAndTitle(item.Executable, item.Title);
    }
}
