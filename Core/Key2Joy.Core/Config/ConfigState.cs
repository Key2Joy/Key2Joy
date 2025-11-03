using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Key2Joy.Config;

public class ConfigState
{
    private IConfigManager configManager;

    [JsonConstructor]
    public ConfigState()
    { }

    public ConfigState(IConfigManager configManager)
        => this.configManager = configManager;

    public string LastInstallPath
    {
        get => this.lastInstallPath;
        set => this.SaveIfInitialized(this.lastInstallPath = value);
    }

    private string lastInstallPath;

    [EnumConfigControl(
        Text = "Group Mappings by",
        EnumType = typeof(ViewMappingGroupType)
    )]
    public ViewMappingGroupType SelectedViewMappingGroupType
    {
        get => this.selectedViewMappingGroupType;
        set => this.SaveIfInitialized(this.selectedViewMappingGroupType = value);
    }

    private ViewMappingGroupType selectedViewMappingGroupType = ViewMappingGroupType.ByAction;

    [BooleanConfigControl(
        Text = "Minimize to Tray"
    )]
    public bool ShouldCloseButtonMinimize
    {
        get => this.shouldCloseButtonMinimize;
        set => this.SaveIfInitialized(this.shouldCloseButtonMinimize = value);
    }

    private bool shouldCloseButtonMinimize;

    #region Listener Overrides

    [BooleanConfigControl(
        Text = "Override Mapped Keyboard Input when Connected"
    )]
    public bool ListenerOverrideDefaultKeyboard
    {
        get => this.listenerOverrideDefaultKeyboard;
        set => this.SaveIfInitialized(this.listenerOverrideDefaultKeyboard = value);
    }

    private bool listenerOverrideDefaultKeyboard = true;

    [BooleanConfigControl(
        Text = "Override All Keyboard Input when Connected",
        Hint = "Make sure you map 'Abort', so you can disconnect."
    )]
    public bool ListenerOverrideDefaultKeyboardAll
    {
        get => this.listenerOverrideDefaultKeyboardAll;
        set => this.SaveIfInitialized(this.listenerOverrideDefaultKeyboardAll = value);
    }

    private bool listenerOverrideDefaultKeyboardAll = false;

    [BooleanConfigControl(
        Text = "Override Mapped Mouse Buttons when Connected"
    )]
    public bool ListenerOverrideDefaultMouse
    {
        get => this.listenerOverrideDefaultMouse;
        set => this.SaveIfInitialized(this.listenerOverrideDefaultMouse = value);
    }

    private bool listenerOverrideDefaultMouse = true;

    [BooleanConfigControl(
        Text = "Override All Mouse Buttons when Connected",
        Hint = "Make sure you map 'Abort', so you can disconnect."
    )]
    public bool ListenerOverrideDefaultMouseAll
    {
        get => this.listenerOverrideDefaultMouseAll;
        set => this.SaveIfInitialized(this.listenerOverrideDefaultMouseAll = value);
    }

    private bool listenerOverrideDefaultMouseAll = false;

    [BooleanConfigControl(
        Text = "Override Mapped Mouse Moves when Connected",
        Hint = "This cannot properly override moves in a specific direction. Use the 'Any' mouse move direction instead."
    )]
    public bool ListenerOverrideDefaultMouseMove
    {
        get => this.listenerOverrideDefaultMouseMove;
        set => this.SaveIfInitialized(this.listenerOverrideDefaultMouseMove = value);
    }

    private bool listenerOverrideDefaultMouseMove = false;

    [BooleanConfigControl(
        Text = "Override All Mouse Moves when Connected",
        Hint = "Make sure you map 'Abort', so you can disconnect."
    )]
    public bool ListenerOverrideDefaultMouseMoveAll
    {
        get => this.listenerOverrideDefaultMouseMoveAll;
        set => this.SaveIfInitialized(this.listenerOverrideDefaultMouseMoveAll = value);
    }

    private bool listenerOverrideDefaultMouseMoveAll = false;

    #endregion Listener Overrides

    [TextConfigControl(
        Text = "Most Recent Profile"
    )]
    public string LastLoadedProfile
    {
        get => this.lastLoadedProfile;
        set => this.SaveIfInitialized(this.lastLoadedProfile = value);
    }

    private string lastLoadedProfile;

    public Dictionary<string, string> EnabledPlugins { get; set; } = new Dictionary<string, string>();

    private void SaveIfInitialized(object changedValue = null)
    {
        if (this.configManager == null || !this.configManager.IsInitialized)
        {
            return;
        }

        this.configManager.Save();
    }
}
