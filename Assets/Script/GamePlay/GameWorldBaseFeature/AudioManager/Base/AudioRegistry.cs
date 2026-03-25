using System.Collections.Generic;

public sealed class AudioRegistry {
    private readonly Dictionary<string, AudioConfig> configDict;

    public AudioRegistry() {
        configDict = new Dictionary<string, AudioConfig>(16);
        RegisterDefaults();
    }

    public void Register(AudioConfig config) {
        if (config == null || string.IsNullOrEmpty(config.AudioId)) {
            return;
        }

        configDict[config.AudioId] = config;
    }

    public AudioConfig GetConfig(string audioId) {
        if (string.IsNullOrEmpty(audioId)) {
            return null;
        }

        if (configDict.TryGetValue(audioId, out AudioConfig config)) {
            return config;
        }

        return null;
    }

    private void RegisterDefaults() {
        Register(new AudioConfig {
            AudioId = "test_bgm",
            BusType = AudioBusType.Bgm,
            IsLoop = true,
            Is3D = false,
            DefaultVolume = 0.22f,
            DefaultPitch = 1f,
            SpatialBlend = 0f,
            UseGeneratedClip = true,
            GeneratedFrequency = 220f,
            GeneratedDuration = 0.6f
        });

        Register(new AudioConfig {
            AudioId = "ui_click",
            BusType = AudioBusType.UI,
            IsLoop = false,
            Is3D = false,
            DefaultVolume = 0.65f,
            DefaultPitch = 1.1f,
            SpatialBlend = 0f,
            UseGeneratedClip = true,
            GeneratedFrequency = 980f,
            GeneratedDuration = 0.1f
        });

        Register(new AudioConfig {
            AudioId = "hp_change",
            BusType = AudioBusType.Sfx,
            IsLoop = false,
            Is3D = false,
            DefaultVolume = 0.55f,
            DefaultPitch = 1f,
            SpatialBlend = 0f,
            UseGeneratedClip = true,
            GeneratedFrequency = 460f,
            GeneratedDuration = 0.14f
        });

        Register(new AudioConfig {
            AudioId = "mp_change",
            BusType = AudioBusType.Sfx,
            IsLoop = false,
            Is3D = false,
            DefaultVolume = 0.55f,
            DefaultPitch = 1f,
            SpatialBlend = 0f,
            UseGeneratedClip = true,
            GeneratedFrequency = 720f,
            GeneratedDuration = 0.12f
        });

        Register(new AudioConfig {
            AudioId = "scene_reload",
            BusType = AudioBusType.UI,
            IsLoop = false,
            Is3D = false,
            DefaultVolume = 0.7f,
            DefaultPitch = 1f,
            SpatialBlend = 0f,
            UseGeneratedClip = true,
            GeneratedFrequency = 840f,
            GeneratedDuration = 0.16f
        });

        Register(new AudioConfig {
            AudioId = "demo_emitter_loop",
            BusType = AudioBusType.Ambient,
            IsLoop = true,
            Is3D = true,
            DefaultVolume = 0.45f,
            DefaultPitch = 1f,
            SpatialBlend = 1f,
            UseGeneratedClip = true,
            GeneratedFrequency = 310f,
            GeneratedDuration = 0.42f
        });
    }
}
