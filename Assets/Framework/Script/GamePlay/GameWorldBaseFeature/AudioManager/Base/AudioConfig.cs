/// <summary>
/// 单个音频条目的静态配置。
/// </summary>
public sealed class AudioConfig {
    public string AudioId;
    public string AssetKey;
    public AudioBusType BusType;
    public bool IsLoop;
    public bool Is3D;
    public float DefaultVolume;
    public float DefaultPitch;
    public float SpatialBlend;
    public bool UseGeneratedClip;
    public float GeneratedFrequency;
    public float GeneratedDuration;
}
