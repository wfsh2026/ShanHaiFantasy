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
