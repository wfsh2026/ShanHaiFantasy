using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public sealed class ConfigLoaderAdapter {
    public void Load(ConfigEntry entry, Action<ScriptableObject> callback) {
        if (entry == null) {
            if (callback != null) {
                callback.Invoke(null);
            }
            return;
        }

#if UNITY_EDITOR
        if (!string.IsNullOrEmpty(entry.AssetPath)) {
            ScriptableObject editorAsset = AssetDatabase.LoadAssetAtPath<ScriptableObject>(entry.AssetPath);
            if (editorAsset != null) {
                if (callback != null) {
                    callback.Invoke(editorAsset);
                }
                return;
            }
        }
#endif

        if (string.IsNullOrEmpty(entry.ConfigKey)) {
            if (callback != null) {
                callback.Invoke(null);
            }
            return;
        }

        AddressablesMgr.Instance.LoadAssetAsync<ScriptableObject>(entry.ConfigKey, callback);
    }
}
