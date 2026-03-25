using System.Collections.Generic;
using UnityEngine;

public sealed class AudioManager {
    private static readonly AudioManager INSTANCE = new AudioManager();
    private readonly Dictionary<string, float> busVolumeDict;
    private readonly Dictionary<string, AudioHandle> activeHandleDict;
    private readonly AudioRegistry registry;
    private readonly AudioAssetLoaderAdapter assetLoader;
    private GameObject rootObject;
    private Transform poolRoot;
    private AudioSource bgmSource;
    private AudioSourcePool sourcePool;
    private int handleCounter;
    private bool isInitialized;
    private string currentBgmAudioId;

    public static AudioManager Instance {
        get {
            return INSTANCE;
        }
    }

    private AudioManager() {
        busVolumeDict = new Dictionary<string, float>(8);
        activeHandleDict = new Dictionary<string, AudioHandle>(16);
        registry = new AudioRegistry();
        assetLoader = new AudioAssetLoaderAdapter();
        handleCounter = 0;
        currentBgmAudioId = null;
        InitializeDefaultBusVolume();
    }

    public void Initialize() {
        if (isInitialized && rootObject != null) {
            return;
        }

        if (rootObject == null) {
            rootObject = new GameObject("AudioManagerRoot");
            Object.DontDestroyOnLoad(rootObject);

            GameObject bgmObject = new GameObject("BgmSource");
            bgmObject.transform.SetParent(rootObject.transform, false);
            bgmSource = bgmObject.AddComponent<AudioSource>();
            bgmSource.playOnAwake = false;
            bgmSource.loop = true;
            bgmSource.spatialBlend = 0f;

            GameObject poolObject = new GameObject("AudioPoolRoot");
            poolObject.transform.SetParent(rootObject.transform, false);
            poolRoot = poolObject.transform;
            sourcePool = new AudioSourcePool(poolRoot);
        }

        isInitialized = true;
    }

    public void Tick(float delta) {
        if (!isInitialized) {
            return;
        }

        List<string> removeKeys = null;
        foreach (KeyValuePair<string, AudioHandle> pair in activeHandleDict) {
            AudioHandle handle = pair.Value;
            if (handle == null || handle.Source == null) {
                if (removeKeys == null) {
                    removeKeys = new List<string>(4);
                }
                removeKeys.Add(pair.Key);
                continue;
            }

            if (handle.Source.isPlaying) {
                continue;
            }

            if (handle.IsPooled && sourcePool != null) {
                sourcePool.ReleaseSource(handle.Source);
            }

            if (removeKeys == null) {
                removeKeys = new List<string>(4);
            }
            removeKeys.Add(pair.Key);
        }

        if (removeKeys == null) {
            return;
        }

        for (int i = 0; i < removeKeys.Count; ++i) {
            activeHandleDict.Remove(removeKeys[i]);
        }
    }

    public void PlayBgm(string audioId) {
        Initialize();

        if (string.IsNullOrEmpty(audioId)) {
            return;
        }

        if (currentBgmAudioId == audioId && bgmSource != null && bgmSource.isPlaying) {
            return;
        }

        AudioConfig config = registry.GetConfig(audioId);
        if (config == null) {
            Debug.LogWarning("AudioManager.PlayBgm skipped because config was not found. AudioId = " + audioId);
            return;
        }

        assetLoader.LoadClip(config, (clip) => {
            if (clip == null || bgmSource == null) {
                return;
            }

            ConfigureSource(bgmSource, config, false);
            bgmSource.clip = clip;
            bgmSource.loop = true;
            bgmSource.Play();
            currentBgmAudioId = audioId;
        });
    }

    public void StopBgm() {
        if (bgmSource == null) {
            return;
        }

        bgmSource.Stop();
        bgmSource.clip = null;
        currentBgmAudioId = null;
    }

    public AudioHandle PlaySfx(string audioId) {
        return PlayInternal(audioId, false, Vector3.zero);
    }

    public AudioHandle PlayUISfx(string audioId) {
        return PlayInternal(audioId, false, Vector3.zero);
    }

    public AudioHandle PlayWorldSfx(string audioId, Vector3 position) {
        return PlayInternal(audioId, true, position);
    }

    public void PlayOnSource(string audioId, AudioSource targetSource) {
        Initialize();

        if (targetSource == null || string.IsNullOrEmpty(audioId)) {
            return;
        }

        AudioConfig config = registry.GetConfig(audioId);
        if (config == null) {
            Debug.LogWarning("AudioManager.PlayOnSource skipped because config was not found. AudioId = " + audioId);
            return;
        }

        assetLoader.LoadClip(config, (clip) => {
            if (clip == null || targetSource == null) {
                return;
            }

            ConfigureSource(targetSource, config, config.Is3D);
            targetSource.clip = clip;
            targetSource.loop = config.IsLoop;
            targetSource.Play();
        });
    }

    public void Stop(AudioHandle handle) {
        if (handle == null) {
            return;
        }

        if (handle.Source != null) {
            handle.Source.Stop();
            if (handle.IsPooled && sourcePool != null) {
                sourcePool.ReleaseSource(handle.Source);
            }
        }

        activeHandleDict.Remove(handle.HandleId);
    }

    public void Stop(AudioSource source) {
        if (source == null) {
            return;
        }

        source.Stop();
        source.clip = null;
    }

    public void SetBusVolume(AudioBusType busType, float volume) {
        string key = busType.ToString();
        busVolumeDict[key] = Mathf.Clamp01(volume);
        RefreshBgmVolume();
    }

    private AudioHandle PlayInternal(string audioId, bool is3D, Vector3 position) {
        Initialize();

        AudioConfig config = registry.GetConfig(audioId);
        if (config == null || sourcePool == null) {
            if (config == null) {
                Debug.LogWarning("AudioManager.PlayInternal skipped because config was not found. AudioId = " + audioId);
            }
            return null;
        }

        AudioSource source = sourcePool.GetSource();
        source.transform.position = position;
        AudioHandle handle = new AudioHandle(CreateHandleId(audioId));
        handle.Source = source;
        handle.IsPooled = true;
        handle.AudioId = audioId;
        activeHandleDict[handle.HandleId] = handle;

        assetLoader.LoadClip(config, (clip) => {
            if (clip == null || source == null) {
                activeHandleDict.Remove(handle.HandleId);
                if (sourcePool != null && source != null) {
                    sourcePool.ReleaseSource(source);
                }
                return;
            }

            ConfigureSource(source, config, is3D || config.Is3D);
            source.clip = clip;
            source.loop = config.IsLoop;
            source.Play();
        });

        return handle;
    }

    private void ConfigureSource(AudioSource source, AudioConfig config, bool is3D) {
        if (source == null || config == null) {
            return;
        }

        source.playOnAwake = false;
        source.pitch = config.DefaultPitch <= 0f ? 1f : config.DefaultPitch;
        source.volume = GetFinalVolume(config);
        source.spatialBlend = is3D ? Mathf.Clamp01(config.SpatialBlend <= 0f ? 1f : config.SpatialBlend) : 0f;
        source.rolloffMode = AudioRolloffMode.Linear;
        source.minDistance = 1f;
        source.maxDistance = 12f;
    }

    private float GetFinalVolume(AudioConfig config) {
        float masterVolume = GetBusVolume(AudioBusType.Master);
        float busVolume = GetBusVolume(config.BusType);
        return Mathf.Clamp01(config.DefaultVolume * masterVolume * busVolume);
    }

    private float GetBusVolume(AudioBusType busType) {
        string key = busType.ToString();
        if (busVolumeDict.TryGetValue(key, out float volume)) {
            return volume;
        }

        return 1f;
    }

    private void RefreshBgmVolume() {
        if (bgmSource == null || string.IsNullOrEmpty(currentBgmAudioId)) {
            return;
        }

        AudioConfig config = registry.GetConfig(currentBgmAudioId);
        if (config == null) {
            return;
        }

        bgmSource.volume = GetFinalVolume(config);
    }

    private void InitializeDefaultBusVolume() {
        busVolumeDict[AudioBusType.Master.ToString()] = 1f;
        busVolumeDict[AudioBusType.Bgm.ToString()] = 1f;
        busVolumeDict[AudioBusType.Sfx.ToString()] = 1f;
        busVolumeDict[AudioBusType.UI.ToString()] = 1f;
        busVolumeDict[AudioBusType.Voice.ToString()] = 1f;
        busVolumeDict[AudioBusType.Ambient.ToString()] = 1f;
    }

    private string CreateHandleId(string audioId) {
        handleCounter += 1;
        return audioId + "_" + handleCounter;
    }
}
