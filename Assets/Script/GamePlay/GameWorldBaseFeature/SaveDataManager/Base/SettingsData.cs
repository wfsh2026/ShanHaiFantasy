using System;

[Serializable]
public sealed class SettingsData {
    public float masterVolume = 1f;
    public float bgmVolume = 1f;
    public float sfxVolume = 1f;
    public float uiVolume = 1f;
    public float voiceVolume = 1f;
    public float ambientVolume = 1f;
    public bool isMute = false;
    public bool isVibration = true;
    public string language = "zh-CN";
    public int qualityLevel = 0;
    public int targetFrameRate = 60;
}
