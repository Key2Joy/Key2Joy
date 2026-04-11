using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Key2Joy.Config;
using Key2Joy.LowLevelInput;
using Key2Joy.LowLevelInput.SimulatedGamePad;
using Key2Joy.LowLevelInput.XInput;
using Key2Joy.Mapping;
using Key2Joy.Util;

namespace Key2Joy.App.Pages;

[ObservableObject]
public partial class MainMappingPageViewModel : IInvokeOnUI
{
    public ObservableCollection<MappedOption> MappedOptions = new();
    public ObservableCollection<IGamePadInfo> Devices = new();

    public MappingProfile SelectedProfile { get; private set; }

    [ObservableProperty]
    public partial bool Armed { get; set; }

    [ObservableProperty]
    public partial string ArmedButtonText { get; set; } = "Connect";

    [ObservableProperty]
    public partial string ArmErrorMessage { get; set; }

    private readonly ConfigState configState;

    public MainMappingPageViewModel()
    {
        this.configState = ServiceContainer.Get<IConfigManager>()
            .GetConfigState();
    }

    public void Initialize()
    {
        var lastLoadedProfile = MappingProfile.RestoreLastLoaded();

        if (lastLoadedProfile != null)
        {
            this.SetSelectedProfile(lastLoadedProfile);
        }

        // Ensure the manager knows which window handle catches all inputs
        Key2JoyManager.Instance.SetHandlerWithInvoke(this);
        Key2JoyManager.Instance.StatusChanged += (s, ev) =>
        {
            //this.SetStatusView(ev.IsEnabled);

            if (ev.Profile != null)
            {
                this.SetSelectedProfile(ev.Profile);
            }
        };
    }

    public void SetSelectedProfile(MappingProfile profile)
    {
        this.SelectedProfile = profile;
        this.configState.LastLoadedProfile = profile.FilePath;

        this.MappedOptions.Clear();

        foreach (var mappedOption in profile.MappedOptions)
        {
            // Only add top-level mapped options, children will be added as part of the parent mapped option
            if (mappedOption.IsChild)
            {
                continue;
            }

            this.MappedOptions.Add(mappedOption);
        }
    }

    /// <summary>
    /// Called automatically when the Armed property changes. We use this to arm or disarm the mappings in the manager, and to refresh the device list.
    /// </summary>
    /// <param name="isArmed"></param>
    partial void OnArmedChanged(bool isArmed)
    {
        ArmedButtonText = isArmed ? "Disconnect" : "Connect";

        if (isArmed)
        {
            try
            {
                Key2JoyManager.Instance.ArmMappings(SelectedProfile);
            }
            catch (MappingArmingFailedException ex)
            {
                Armed = false;
                ArmErrorMessage = ex.Message;
            }
        }
        else if (Key2JoyManager.Instance.GetIsArmed())
        {
            Key2JoyManager.Instance.DisarmMappings();
        }

        RefreshDevices();
    }

    private void RefreshDevices()
    {
        this.Devices.Clear();

        this.RefreshSimulatedDevices();
        this.RefreshPhysicalDevices();
    }

    private void RefreshPhysicalDevices()
    {
        var xInputService = ServiceContainer.Get<IXInputService>();
        xInputService.RecognizePhysicalDevices();
        var deviceIndexes = xInputService.GetActiveDevicesInfo();

        foreach (var device in deviceIndexes)
        {
            this.Devices.Add(device);
        }
    }

    private void RefreshSimulatedDevices()
    {
        var gamePadService = ServiceContainer.Get<ISimulatedGamePadService>();
        var simulatedGamePads = gamePadService.GetActiveDevicesInfo();

        foreach (var gamePad in simulatedGamePads)
        {
            this.Devices.Add(gamePad);
        }
    }

    public object Invoke(Delegate method)
        => method.DynamicInvoke();

    public object Invoke(Delegate method, params object[] arguments)
        => method.DynamicInvoke(arguments);
}
