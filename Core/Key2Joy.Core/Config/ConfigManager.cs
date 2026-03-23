using System.IO;
using System.Text.Json;
using Key2Joy.Contracts;
using Key2Joy.Util;

namespace Key2Joy.Config;

/// <summary>
/// Manages user configurations being loaded from and saved to disk.
/// </summary>
public class ConfigManager : IConfigManager
{
    protected const string CONFIG_PATH = "config.json";

    public bool IsInitialized { get; private set; }

    private ConfigState configState;

    public ConfigManager()
        => this.LoadOrCreate();

    /// <returns>The path to where the config file is located.</returns>
    protected virtual string GetAppDataDirectory() => Output.GetAppDataDirectory();

    public ConfigState GetConfigState()
        => this.configState;

    public void Save()
    {
        var options = GetSerializerOptions();
        var configPath = Path.Combine(
            this.GetAppDataDirectory(),
            CONFIG_PATH);

        File.WriteAllText(configPath, JsonSerializer.Serialize(this.configState, options));
    }

    /// <summary>
    /// Loads the configuration or creates a default one on disk.
    /// </summary>
    public void LoadOrCreate()
    {
        var configPath = Path.Combine(
            this.GetAppDataDirectory(),
            CONFIG_PATH);

        this.configState = new ConfigState(this);

        if (File.Exists(configPath))
        {
            var options = GetSerializerOptions();
            // Merge the loaded config state with the default config state
            JsonUtilities.PopulateObject(
                File.ReadAllText(configPath),
                this.configState,
                options
            );
        }

        var assembly = System.Reflection.Assembly.GetEntryAssembly();

        // If the assembly is null then we are running in a unit test
        if (assembly == null)
        {
            this.CompleteInitialization();
            return;
        }

        var executablePath = assembly.Location;
        if (executablePath.EndsWith("Key2Joy.exe")
            && this.configState.LastInstallPath != executablePath)
        {
            this.configState.LastInstallPath = executablePath;
        }

        this.CompleteInitialization();
        return;
    }

    private void CompleteInitialization()
    {
        this.IsInitialized = true;

        // We save so old properties are removed and new ones are added to the config file immediately
        this.Save();
    }

    protected static JsonSerializerOptions GetSerializerOptions()
    {
        JsonSerializerOptions options = new()
        {
            WriteIndented = true,
        };

        return options;
    }
}
