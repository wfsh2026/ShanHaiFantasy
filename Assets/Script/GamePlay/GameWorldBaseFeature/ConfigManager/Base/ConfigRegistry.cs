using System;
using System.Collections.Generic;

public sealed class ConfigRegistry {
    private readonly Dictionary<Type, ConfigEntry> configEntries;

    public ConfigRegistry() {
        configEntries = new Dictionary<Type, ConfigEntry>(8);
        RegisterDefaults();
    }

    public ConfigEntry GetEntry<T>() where T : UnityEngine.ScriptableObject {
        return GetEntry(typeof(T));
    }

    public ConfigEntry GetEntry(Type configType) {
        if (configType == null) {
            return null;
        }

        ConfigEntry entry;
        if (configEntries.TryGetValue(configType, out entry)) {
            return entry;
        }

        return null;
    }

    public List<ConfigEntry> GetPreloadEntries() {
        List<ConfigEntry> preloadEntries = new List<ConfigEntry>(configEntries.Count);
        foreach (ConfigEntry entry in configEntries.Values) {
            if (entry.IsPreload) {
                preloadEntries.Add(entry);
            }
        }

        return preloadEntries;
    }

    private void Register(ConfigEntry entry) {
        if (entry == null || entry.ConfigType == null) {
            return;
        }

        configEntries[entry.ConfigType] = entry;
    }

    private void RegisterDefaults() {
        Register(new ConfigEntry(typeof(TestModeConfig), "Config/TestModeConfig", "Assets/ToBundle/Configs/TestModeConfig.asset", true));
    }
}
