using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 配置系统唯一入口。
/// 负责按类型加载、缓存和返回 ScriptableObject 配置实例。
/// </summary>
public sealed class ConfigManager {
    private static readonly ConfigManager INSTANCE = new ConfigManager();

    private readonly Dictionary<Type, ScriptableObject> configCache;
    private readonly Dictionary<Type, List<Action<ScriptableObject>>> pendingCallbacks;

    private ConfigRegistry configRegistry;
    private ConfigLoaderAdapter configLoaderAdapter;
    private bool isInitialized;

    public static ConfigManager Instance {
        get {
            return INSTANCE;
        }
    }

    private ConfigManager() {
        configCache = new Dictionary<Type, ScriptableObject>(8);
        pendingCallbacks = new Dictionary<Type, List<Action<ScriptableObject>>>(8);
    }

    public void Initialize() {
        if (isInitialized) {
            return;
        }

        configRegistry = new ConfigRegistry();
        configLoaderAdapter = new ConfigLoaderAdapter();
        isInitialized = true;
    }

    public void Clear() {
        configCache.Clear();
        pendingCallbacks.Clear();
        configRegistry = null;
        configLoaderAdapter = null;
        isInitialized = false;
    }

    public void PreloadDefaults() {
        Initialize();
        List<ConfigEntry> preloadEntries = configRegistry.GetPreloadEntries();
        for (int i = 0; i < preloadEntries.Count; ++i) {
            ConfigEntry entry = preloadEntries[i];
            if (entry == null || entry.ConfigType == null) {
                continue;
            }

            LoadInternal(entry.ConfigType, entry);
        }
    }

    public void Load<T>(Action<T> callback) where T : ScriptableObject {
        Initialize();

        Type configType = typeof(T);
        T cachedConfig;
        if (TryGet(out cachedConfig)) {
            if (callback != null) {
                callback.Invoke(cachedConfig);
            }
            return;
        }

        ConfigEntry entry = configRegistry.GetEntry<T>();
        if (entry == null) {
            if (callback != null) {
                callback.Invoke(null);
            }
            return;
        }

        List<Action<ScriptableObject>> callbacks;
        if (!pendingCallbacks.TryGetValue(configType, out callbacks)) {
            callbacks = new List<Action<ScriptableObject>>(2);
            pendingCallbacks.Add(configType, callbacks);
        }

        if (callback != null) {
            // 同类型配置加载中的情况下，后续请求直接挂到回调队列，避免重复发起加载。
            callbacks.Add((asset) => {
                callback.Invoke(asset as T);
            });
        }

        if (callbacks.Count == 1) {
            LoadInternal(configType, entry);
        }
    }

    public T Get<T>() where T : ScriptableObject {
        T config;
        if (TryGet(out config)) {
            return config;
        }

        return null;
    }

    public bool TryGet<T>(out T config) where T : ScriptableObject {
        ScriptableObject cachedConfig;
        if (configCache.TryGetValue(typeof(T), out cachedConfig)) {
            config = cachedConfig as T;
            return config != null;
        }

        config = null;
        return false;
    }

    private void LoadInternal(Type configType, ConfigEntry entry) {
        if (configLoaderAdapter == null) {
            return;
        }

        configLoaderAdapter.Load(entry, (configAsset) => {
            OnConfigLoaded(configType, configAsset);
        });
    }

    private void OnConfigLoaded(Type configType, ScriptableObject configAsset) {
        List<Action<ScriptableObject>> callbacks = null;

        if (configAsset != null) {
            configCache[configType] = configAsset;
        }

        if (pendingCallbacks.TryGetValue(configType, out callbacks)) {
            pendingCallbacks.Remove(configType);
        }

        if (callbacks == null) {
            return;
        }

        for (int i = 0; i < callbacks.Count; ++i) {
            Action<ScriptableObject> callback = callbacks[i];
            if (callback != null) {
                callback.Invoke(configAsset);
            }
        }
    }
}
