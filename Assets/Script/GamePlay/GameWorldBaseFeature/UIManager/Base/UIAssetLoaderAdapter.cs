using System;
using UnityEngine;

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
