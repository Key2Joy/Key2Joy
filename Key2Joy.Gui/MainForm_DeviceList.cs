using System.Windows.Forms;
using Key2Joy.LowLevelInput.SimulatedGamePad;
using Key2Joy.LowLevelInput.XInput;
using Key2Joy.Util;

namespace Key2Joy.Gui;

partial class MainForm
{
    private void AddDeviceControl(DeviceControl control)
    {
        control.Dock = DockStyle.Top;
        this.pnlDevices.Controls.Add(control);
    }

    private void RefreshPhysicalDevices()
    {
        var xInputService = ServiceContainer.Get<IXInputService>();
        xInputService.RecognizePhysicalDevices();
        var deviceIndexes = xInputService.GetActiveDevicesInfo();

        foreach (var device in deviceIndexes)
        {
            this.AddDeviceControl(new DeviceControl(device));
        }
    }

    private void RefreshSimulatedDevices()
    {
        var gamePadService = ServiceContainer.Get<ISimulatedGamePadService>();
        var simulatedGamePads = gamePadService.GetActiveDevicesInfo();

        foreach (var gamePad in simulatedGamePads)
        {
            this.AddDeviceControl(new DeviceControl(gamePad));
        }
    }

    private void RefreshDevices()
    {
        this.pnlDevices.Controls.Clear();

        this.RefreshSimulatedDevices();
        this.RefreshPhysicalDevices();

        this.lblListPlaceholder.Visible = this.pnlDevices.Controls.Count == 0;
    }
}
