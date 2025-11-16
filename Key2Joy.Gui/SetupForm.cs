using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
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
            ScpDriverInstaller.Install();
            Program.ShowMainForm();
        }
        catch (Win32Exception)
        {
            return;
        }
    }

    private void btnCreateSystemRestorePoint_Click(object sender, EventArgs e)
    {
        Process.Start("SystemPropertiesProtection.exe");
    }
}
