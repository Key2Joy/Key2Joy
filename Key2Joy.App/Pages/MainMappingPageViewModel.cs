using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Key2Joy.Config;
using Key2Joy.LowLevelInput;
using Key2Joy.LowLevelInput.SimulatedGamePad;
using Key2Joy.LowLevelInput.XInput;
using Key2Joy.Mapping;
using Key2Joy.Mapping.Actions;
using Key2Joy.Mapping.Actions.Input;
using Key2Joy.Mapping.Triggers;
using Key2Joy.Util;

namespace Key2Joy.App.Pages;

[ObservableObject]
public partial class MainMappingPageViewModel : IInvokeOnUI
{
    public ObservableCollection<MappedOptionViewModel> MappedOptions { get; } = new();
    public ObservableCollection<MappingGroupViewModel> FilteredMappingGroups { get; } = new();
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

    [ObservableProperty]
    public partial int FilteredMappedOptionsCount { get; set; } = 0;

    [ObservableProperty]
    public partial bool HasSelectedMapping { get; set; } = false;

    private bool _isSettingProfile;

    private MappedOptionViewModel _selectedMappedOption;

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

            this.MappedOptions.Add(new MappedOptionViewModel(mappedOption));
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
            this.MappedOptions.Add(new MappedOptionViewModel(mappedOption));
        }

        this.UpdateFilter();
    }

    public void RemoveMapping(MappedOption mappedOption)
    {
        this.SelectedProfile?.RemoveMapping(mappedOption);

        if (!mappedOption.IsChild)
        {
            var vm = this.MappedOptions.FirstOrDefault(x => x.Option == mappedOption);

            if (vm != null)
            {
                this.MappedOptions.Remove(vm);
            }
        }
        else
        {
            // Detach from parent so parent.Children is updated, then rebuild the parent VM
            // so its Children snapshot reflects the removal.
            var parentOption = mappedOption.Parent;
            mappedOption.SetParent(null);

            if (parentOption != null)
            {
                var parentVm = this.MappedOptions.FirstOrDefault(x => x.Option == parentOption);

                if (parentVm != null)
                {
                    var index = this.MappedOptions.IndexOf(parentVm);
                    this.MappedOptions[index] = new MappedOptionViewModel(parentOption);
                }
            }
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

    [RelayCommand]
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
        this.FilteredMappingGroups.Clear();

        var filtered = this.MappedOptions
            .Where(vm => string.IsNullOrEmpty(this.SearchText)
                || vm.Option.Action?.ToString().IndexOf(this.SearchText, StringComparison.OrdinalIgnoreCase) > -1
                || vm.Option.Trigger?.ToString().IndexOf(this.SearchText, StringComparison.OrdinalIgnoreCase) > -1)
            .ToList();

        var groupType = this.configState.SelectedViewMappingGroupType;

        if (groupType == ViewMappingGroupType.None)
        {
            var group = new MappingGroupViewModel { GroupName = string.Empty, IsHeaderVisible = false };

            foreach (var vm in filtered)
            {
                group.Items.Add(vm);
            }

            this.FilteredMappingGroups.Add(group);
        }
        else
        {
            var grouped = new Dictionary<string, MappingGroupViewModel>();

            foreach (var vm in filtered)
            {
                string key;

                if (groupType == ViewMappingGroupType.ByAction)
                {
                    key = vm.Option.Action != null
                        ? ActionsRepository.GetAttributeForAction(vm.Option.Action)?.GroupName ?? "Other"
                        : "Other";
                }
                else
                {
                    key = vm.Option.Trigger != null
                        ? TriggersRepository.GetAttributeForTrigger(vm.Option.Trigger)?.GroupName ?? "Other"
                        : "Other";
                }

                if (!grouped.TryGetValue(key, out var grp))
                {
                    grp = new MappingGroupViewModel { GroupName = key, IsHeaderVisible = true };
                    grouped[key] = grp;
                }

                grp.Items.Add(vm);
            }

            foreach (var grp in grouped.Values)
            {
                this.FilteredMappingGroups.Add(grp);
            }
        }

        this.FilteredMappedOptionsCount = this.FilteredMappingGroups.Sum(g => g.Items.Count);
    }

    public void SelectMapping(MappedOption option)
    {
        if (this._selectedMappedOption != null)
        {
            this._selectedMappedOption.IsSelected = false;
            this._selectedMappedOption = null;
        }

        if (option == null)
        {
            this.HasSelectedMapping = false;
            return;
        }

        foreach (var group in this.FilteredMappingGroups)
        {
            foreach (var vm in group.Items)
            {
                if (vm.Option.Guid == option.Guid)
                {
                    vm.IsSelected = true;
                    this._selectedMappedOption = vm;
                    this.HasSelectedMapping = true;
                    return;
                }

                foreach (var child in vm.Children)
                {
                    if (child.Option.Guid == option.Guid)
                    {
                        child.IsSelected = true;
                        this._selectedMappedOption = child;
                        this.HasSelectedMapping = true;
                        return;
                    }
                }
            }
        }
    }

    public void DeselectSelectedMapping()
        => this.SelectMapping(null);

    public MappedOption GetSelectedMappingOption()
        => this._selectedMappedOption?.Option;

    public MappingProfile CreateNewProfile(string nameSuffix = default)
    {
        MappingProfile profile = new($"{this.ProfileName}{nameSuffix}", this.SelectedProfile?.MappedOptions);
        this.SetSelectedProfile(profile);
        profile.Save();
        return profile;
    }

    public void HandleMappingCreated(MappedOption mappedOption)
    {
        var isExisting = this.MappedOptions.Any(vm => vm.Option == mappedOption);

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
                this.MappedOptions.Add(new MappedOptionViewModel(mappedOption));
            }
        }

        this.UpdateFilter();
    }

    public void GenerateReversesForMapping(MappedOption mappedOption)
    {
        if (this.SelectedProfile == null)
        {
            return;
        }

        var newOptions = MappedOption.GenerateReverseMappings(new List<MappedOption> { mappedOption });

        foreach (var option in newOptions)
        {
            this.SelectedProfile.MappedOptions.Add(option);
        }

        this.SelectedProfile.Save();
        this.SetSelectedProfile(this.SelectedProfile);
    }

    public void MakeMappingParentless(MappedOption childOption)
    {
        if (this.SelectedProfile == null)
        {
            return;
        }

        childOption.SetParent(null);
        this.SelectedProfile.Save();
        this.SetSelectedProfile(this.SelectedProfile);
    }

    public void ChooseNewParent(MappedOption child, MappedOption newParent)
    {
        if (this.SelectedProfile == null)
        {
            return;
        }

        child.SetParent(newParent);
        this.SelectedProfile.Save();
        this.SetSelectedProfile(this.SelectedProfile);
    }

    public object Invoke(Delegate method)
        => method.DynamicInvoke();

    public object Invoke(Delegate method, params object[] arguments)
        => method.DynamicInvoke(arguments);
}
