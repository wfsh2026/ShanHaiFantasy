/// <summary>
/// 单个配置类型在运行时的注册信息。
/// 包含类型、Addressables Key、编辑器资产路径和是否预加载。
/// </summary>
public sealed class ConfigEntry {
    public ConfigEntry(System.Type configType, string configKey, string assetPath, bool isPreload) {
        ConfigType = configType;
        ConfigKey = configKey;
        AssetPath = assetPath;
        IsPreload = isPreload;
    }

    public System.Type ConfigType {
        get;
        private set;
    }

    public string ConfigKey {
        get;
        private set;
    }

    public string AssetPath {
        get;
        private set;
    }

    public bool IsPreload {
        get;
        private set;
    }
}
