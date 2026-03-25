using System;
using UnityEngine;

public sealed class AudioAssetLoaderAdapter {
    public void LoadClip(AudioConfig config, Action<AudioClip> callback) {
        if (config == null) {
            callback?.Invoke(null);
            return;
        }

        if (!string.IsNullOrEmpty(config.AssetKey)) {
            AddressablesMgr.Instance.LoadAssetAsync<AudioClip>(config.AssetKey, (clip) => {
                if (clip != null) {
                    callback?.Invoke(clip);
                    return;
                }

                callback?.Invoke(CreateFallbackClip(config));
            });
            return;
        }

        callback?.Invoke(CreateFallbackClip(config));
    }

    private static AudioClip CreateFallbackClip(AudioConfig config) {
        if (!config.UseGeneratedClip) {
            return null;
        }

        return AudioGeneratedClipFactory.GetOrCreateClip(config);
    }
}
