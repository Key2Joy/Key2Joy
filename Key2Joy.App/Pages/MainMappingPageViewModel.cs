using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using Key2Joy.Config;
using Key2Joy.LowLevelInput;
using Key2Joy.LowLevelInput.SimulatedGamePad;
using Key2Joy.LowLevelInput.XInput;
using Key2Joy.Mapping;
using Key2Joy.Mapping.Actions.Input;
using Key2Joy.Util;

namespace Key2Joy.App.Pages;

[ObservableObject]
public partial class MainMappingPageViewModel : IInvokeOnUI
{
    public ObservableCollection<MappedOption> MappedOptions { get; } = new();
    public ObservableCollection<MappedOption> FilteredMappedOptions { get; } = new();
    public ObservableCollection<IGamePadInfo> Devices { get; } = new();

    public MappingProfile SelectedProfile { get; private set; }

    [ObservableProperty]
    public partial bool Armed { get; set; }

    [ObservableProperty]
    public partial string ArmedButtonText { get; set; } = "Connect";

    [ObservableProperty]
    public partial string ArmErrorMessage { get; set; }

    [ObservableProperty]
    public partial string SearchText { get; set; } = "";

    [ObservableProperty]
    public partial string ProfileName { get; set; } = "";

    [ObservableProperty]
    public partial int DevicesCount { get; set; } = 0;

    private bool _isSettingProfile;

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
        _isSettingProfile = true;

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

        this.ProfileName = profile.Name;
        _isSettingProfile = false;

        this.UpdateFilter();
    }

    public void AddMapping(MappedOption mappedOption)
    {
        this.SelectedProfile?.AddMapping(mappedOption);

        if (!mappedOption.IsChild)
        {
            this.MappedOptions.Add(mappedOption);
        }

        this.UpdateFilter();
    }

    public void RemoveMapping(MappedOption mappedOption)
    {
        this.SelectedProfile?.RemoveMapping(mappedOption);

        if (!mappedOption.IsChild)
        {
            this.MappedOptions.Remove(mappedOption);
        }

        this.UpdateFilter();
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

        this.DevicesCount = this.Devices.Count;
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

    partial void OnSearchTextChanged(string value)
        => this.UpdateFilter();

    partial void OnProfileNameChanged(string value)
    {
        if (_isSettingProfile || this.SelectedProfile == null)
        {
            return;
        }

        this.SelectedProfile.Name = value;
        this.SelectedProfile.Save();
    }

    private void UpdateFilter()
    {
        this.FilteredMappedOptions.Clear();

        foreach (var mappedOption in this.MappedOptions)
        {
            if (string.IsNullOrEmpty(this.SearchText)
                || mappedOption.Action?.ToString().IndexOf(this.SearchText, StringComparison.OrdinalIgnoreCase) > -1
                || mappedOption.Trigger?.ToString().IndexOf(this.SearchText, StringComparison.OrdinalIgnoreCase) > -1)
            {
                this.FilteredMappedOptions.Add(mappedOption);
            }
        }
    }

    public MappingProfile CreateNewProfile(string nameSuffix = default)
    {
        MappingProfile profile = new($"{this.ProfileName}{nameSuffix}", this.SelectedProfile?.MappedOptions);
        this.SetSelectedProfile(profile);
        profile.Save();
        return profile;
    }

    public void HandleMappingCreated(MappedOption mappedOption)
    {
        var isExisting = this.MappedOptions.Contains(mappedOption);

        if (!isExisting)
        {
            this.AddMapping(mappedOption);

            foreach (var child in mappedOption.Children)
            {
                this.AddMapping(child);
            }
        }

        this.SelectedProfile?.Save();
    }

    public void HandleMappingDeleted(MappedOption mappedOption)
    {
        this.RemoveMapping(mappedOption);
        this.SelectedProfile?.Save();
    }

    public void AddAllGamePadMappings(bool pressOnly = false, bool releaseOnly = false)
    {
        List<MappedOption> range = new();

        if (pressOnly)
        {
            range.AddRange(GamePadButtonAction.GetAllButtonActions(PressState.Press));
        }
        else if (releaseOnly)
        {
            range.AddRange(GamePadButtonAction.GetAllButtonActions(PressState.Release));
        }
        else
        {
            // Press (parent) + Release (child) pairs, one per button
            foreach (var pressOption in GamePadButtonAction.GetAllButtonActions(PressState.Press))
            {
                range.Add(pressOption);
                range.Add(MappedOption.GenerateReverseMapping(pressOption));
            }
        }

        this.AddMappingRange(range);
    }

    public void AddAllKeyboardMappings(bool pressOnly = false, bool releaseOnly = false)
    {
        List<MappedOption> range = new();

        if (pressOnly)
        {
            range.AddRange(KeyboardAction.GetAllButtonActions(PressState.Press));
        }
        else if (releaseOnly)
        {
            range.AddRange(KeyboardAction.GetAllButtonActions(PressState.Release));
        }
        else
        {
            // Press (parent) + Release (child) pairs, one per key
            foreach (var pressOption in KeyboardAction.GetAllButtonActions(PressState.Press))
            {
                range.Add(pressOption);
                range.Add(MappedOption.GenerateReverseMapping(pressOption));
            }
        }

        this.AddMappingRange(range);
    }

    private void AddMappingRange(IEnumerable<MappedOption> mappedOptions)
    {
        if (this.SelectedProfile == null)
        {
            return;
        }

        this.SelectedProfile.AddMappingRange(mappedOptions);

        // Rebuild navigation (Children/Parent) for all items — AddMappingRange clones options
        // so Children lists are empty; Initialize rebuilds them from ParentGuid.
        var allOptions = this.SelectedProfile.MappedOptions.ToList();
        foreach (var option in allOptions)
        {
            option.Initialize(allOptions);
        }

        this.SelectedProfile.Save();

        // Rebuild MappedOptions from the profile (same as SetSelectedProfile but without resetting profile/name)
        this.MappedOptions.Clear();

        foreach (var mappedOption in this.SelectedProfile.MappedOptions)
        {
            if (!mappedOption.IsChild)
            {
                this.MappedOptions.Add(mappedOption);
            }
        }

        this.UpdateFilter();
    }

    public object Invoke(Delegate method)
        => method.DynamicInvoke();

    public object Invoke(Delegate method, params object[] arguments)
        => method.DynamicInvoke(arguments);
}
