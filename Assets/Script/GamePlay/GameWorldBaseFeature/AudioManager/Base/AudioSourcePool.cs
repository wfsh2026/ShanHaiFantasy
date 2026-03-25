using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AudioSource 复用池。
/// 用于管理一次性音效的临时播放源，减少频繁创建销毁。
/// </summary>
public sealed class AudioSourcePool {
    private readonly Transform root;
    private readonly Stack<AudioSource> sourceStack;

    public AudioSourcePool(Transform poolRoot) {
        root = poolRoot;
        sourceStack = new Stack<AudioSource>(8);
    }

    public AudioSource GetSource() {
        while (sourceStack.Count > 0) {
            AudioSource cachedSource = sourceStack.Pop();
            if (cachedSource != null) {
                cachedSource.gameObject.SetActive(true);
                return cachedSource;
            }
        }

        GameObject sourceObject = new GameObject("PooledAudioSource");
        sourceObject.transform.SetParent(root, false);
        AudioSource source = sourceObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        return source;
    }

    public void ReleaseSource(AudioSource source) {
        if (source == null) {
            return;
        }

        source.Stop();
        source.clip = null;
        source.loop = false;
        source.spatialBlend = 0f;
        source.transform.SetParent(root, false);
        source.transform.localPosition = Vector3.zero;
        source.gameObject.SetActive(false);
        sourceStack.Push(source);
    }
}
