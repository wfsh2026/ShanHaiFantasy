using System;
using System.Collections.Generic;

/// <summary>
/// 配置注册表。
/// 统一维护“配置类型 -> 加载信息”的映射，避免业务层散写资源 Key 和资产路径。
/// </summary>
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
        // 测试模式当前只需要一份默认配置。
        // 编辑器下会优先按资产路径直接加载，运行时再使用 Addressables Key。
        Register(new ConfigEntry(
            typeof(TestModeConfig),
            "Config/TestModeConfig",
            "Assets/Content/ToBundle/Configs/TestModeConfig.asset",
            true));
    }
}
