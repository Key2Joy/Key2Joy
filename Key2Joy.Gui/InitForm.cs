using System;
using System.Windows.Forms;
using CommonServiceLocator;
using Key2Joy.LowLevelInput.SimulatedGamePad;
using Key2Joy.Mapping;
using SimWinInput;

namespace Key2Joy.Gui;

public partial class InitForm : Form
{
    private readonly bool shouldStartMinimized;

    public InitForm(bool shouldStartMinimized = false)
    {
        this.shouldStartMinimized = shouldStartMinimized;

        this.InitializeComponent();

        this.WindowState = FormWindowState.Minimized;
        this.ShowInTaskbar = false;
    }

    private void InitForm_Load(object sender, EventArgs e)
    {
        MappingProfile.ExtractDefaultIfNotExists();
        var gamePadService = ServiceLocator.Current.GetInstance<ISimulatedGamePadService>();

        try
        {
            gamePadService.Initialize();
        }
        catch (UserDeclinedDriverException)
        {
            Application.Exit();
            return;
        }

        MainForm mainForm = new(this.shouldStartMinimized);
        Program.GoToNextForm(mainForm);
    }
}
