using System.Windows.Forms;
using Key2Joy.LowLevelInput;

namespace Key2Joy.Gui;

public partial class DeviceControl : UserControl
{
    private DeviceControl()
    {
        this.InitializeComponent();
    }

    public DeviceControl(IGamePadInfo device)
        : this()
    {
        this.lblIndex.Text = $"#{device.Index}";
        this.lblDevice.Text = device.Name;
    }
}
