using System;
using System.ComponentModel;
using System.Windows.Forms;
using Key2Joy.Gui.Util;
using SimWinInput;

namespace Key2Joy.Gui;

public partial class SetupForm : Form
{
    public SetupForm()
    {
        InitializeComponent();
    }

    private void btnNext_Click(object sender, EventArgs e)
    {

        try
        {
            WindowsUtilities.CreateRestorePoint("Key2Joy Installation", WindowsUtilities.EventType.BeginSystemChange, WindowsUtilities.RestorePointType.ApplicationInstall);
        }
        catch
        {
            // ignored, non-critical
        }

        try
        {
            ScpDriverInstaller.Install();
        }
        catch (Win32Exception)
        {
            return;
        }

        Program.ShowMainForm();
    }
}
