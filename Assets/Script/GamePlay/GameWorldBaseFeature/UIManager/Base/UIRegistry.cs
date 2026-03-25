using System;
using System.Collections.Generic;

public sealed class UIRegistry {
    private readonly Dictionary<Type, UIWindowConfig> panelTypeDict = new Dictionary<Type, UIWindowConfig>();
    private readonly Dictionary<string, UIWindowConfig> uiIdDict = new Dictionary<string, UIWindowConfig>();

    public void Register<TPanel>(
        string uiId,
        UILayer layer,
        UICacheMode cacheMode,
        UIOpenMode openMode,
        string prefabKey,
        bool isFullScreen,
        bool useBackStack,
        bool blockRaycast)
        where TPanel : UIPanelBase {
        UIWindowConfig config = new UIWindowConfig();
        config.UIId = uiId;
        config.PrefabKey = prefabKey;
        config.Layer = layer;
        config.CacheMode = cacheMode;
        config.OpenMode = openMode;
        config.IsFullScreen = isFullScreen;
        config.UseBackStack = useBackStack;
        config.BlockRaycast = blockRaycast;
        config.PanelType = typeof(TPanel);

        panelTypeDict[typeof(TPanel)] = config;
        uiIdDict[uiId] = config;
    }

    public UIWindowConfig GetConfig<TPanel>() where TPanel : UIPanelBase {
        return GetConfig(typeof(TPanel));
    }

    public UIWindowConfig GetConfig(Type panelType) {
        if (!panelTypeDict.TryGetValue(panelType, out UIWindowConfig config)) {
            return null;
        }

        return config;
    }

    public UIWindowConfig GetConfig(string uiId) {
        if (!uiIdDict.TryGetValue(uiId, out UIWindowConfig config)) {
            return null;
        }

        return config;
    }
}
