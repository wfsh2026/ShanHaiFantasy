using System;
using UnityEngine;

/// <summary>
/// UI 资源加载适配层。
/// 当前统一通过项目内的 ContentLoader 加载 Panel Prefab。
/// </summary>
public sealed class UIAssetLoaderAdapter {
    public void LoadPanel(UIWindowConfig config, Transform parent, Action<GameObject> callback) {
        if (config == null) {
            callback?.Invoke(null);
            return;
        }

        if (string.IsNullOrEmpty(config.PrefabKey)) {
            callback?.Invoke(null);
            return;
        }

        ContentLoader.LoadPrefab(config.PrefabKey, parent, callback);
    }
}
