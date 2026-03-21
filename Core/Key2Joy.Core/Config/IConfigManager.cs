namespace Key2Joy.Config;

public interface IConfigManager
{
    bool IsInitialized { get; }

    void Save();

    void LoadOrCreate();

    ConfigState GetConfigState();
}
