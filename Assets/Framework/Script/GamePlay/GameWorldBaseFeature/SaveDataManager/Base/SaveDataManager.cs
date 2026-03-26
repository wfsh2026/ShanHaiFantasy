using UnityEngine;

/// <summary>
/// 本地存档唯一入口。
/// 统一管理设置数据、玩家本地数据、脏标记和 JSON 落盘。
/// </summary>
public sealed class SaveDataManager {
    private static readonly SaveDataManager INSTANCE = new SaveDataManager();
    private const float AUTO_SAVE_INTERVAL = 5f;

    private SaveFileAdapter saveFileAdapter;
    private bool isInitialized;
    private bool isDirty;
    private float autoSaveTimer;

    public static SaveDataManager Instance {
        get {
            return INSTANCE;
        }
    }

    public SettingsData Settings {
        get;
        private set;
    }

    public PlayerLocalData PlayerData {
        get;
        private set;
    }

    private SaveDataManager() {
        Settings = new SettingsData();
        PlayerData = new PlayerLocalData();
    }

    public void Initialize() {
        if (isInitialized) {
            return;
        }

        saveFileAdapter = new SaveFileAdapter();
        LoadAll();
        isInitialized = true;
    }

    public void Clear() {
        SaveAll();
        saveFileAdapter = null;
        Settings = new SettingsData();
        PlayerData = new PlayerLocalData();
        isInitialized = false;
        isDirty = false;
        autoSaveTimer = 0f;
    }

    public void Tick(float delta) {
        if (!isInitialized || !isDirty) {
            return;
        }

        // 脏数据统一走定时保存，避免每次修改都立即写磁盘。
        autoSaveTimer += delta;
        if (autoSaveTimer >= AUTO_SAVE_INTERVAL) {
            SaveAll();
        }
    }

    public void LoadAll() {
        if (saveFileAdapter == null) {
            saveFileAdapter = new SaveFileAdapter();
        }

        Settings = saveFileAdapter.Load<SettingsData>(SavePathRegistry.SettingsFilePath);
        PlayerData = saveFileAdapter.Load<PlayerLocalData>(SavePathRegistry.PlayerDataFilePath);
        ApplySettingsToRuntime();
        isDirty = false;
        autoSaveTimer = 0f;
    }

    public void SaveAll() {
        if (saveFileAdapter == null) {
            saveFileAdapter = new SaveFileAdapter();
        }

        saveFileAdapter.Save(SavePathRegistry.SettingsFilePath, Settings);
        saveFileAdapter.Save(SavePathRegistry.PlayerDataFilePath, PlayerData);
        isDirty = false;
        autoSaveTimer = 0f;
    }

    public void MarkDirty() {
        isDirty = true;
        autoSaveTimer = 0f;
    }

    public void SetBusVolume(AudioBusType busType, float volume) {
        float targetVolume = Mathf.Clamp01(volume);
        switch (busType) {
            case AudioBusType.Master:
                Settings.masterVolume = targetVolume;
                break;
            case AudioBusType.Bgm:
                Settings.bgmVolume = targetVolume;
                break;
            case AudioBusType.Sfx:
                Settings.sfxVolume = targetVolume;
                break;
            case AudioBusType.UI:
                Settings.uiVolume = targetVolume;
                break;
            case AudioBusType.Voice:
                Settings.voiceVolume = targetVolume;
                break;
            case AudioBusType.Ambient:
                Settings.ambientVolume = targetVolume;
                break;
        }

        ApplySettingsToRuntime();
        MarkDirty();
    }

    public void SetMute(bool isMute) {
        Settings.isMute = isMute;
        ApplySettingsToRuntime();
        MarkDirty();
    }

    public void SetLastScene(SceneId sceneId, string scenePath) {
        PlayerData.lastSceneId = (int)sceneId;
        PlayerData.lastScenePath = scenePath ?? string.Empty;
        MarkDirty();
    }

    public void SetLastMode(string modeId) {
        PlayerData.lastModeId = modeId ?? string.Empty;
        MarkDirty();
    }

    public void SetPlayerDisplayName(string playerDisplayName) {
        if (string.IsNullOrEmpty(playerDisplayName)) {
            return;
        }

        PlayerData.playerDisplayName = playerDisplayName;
        MarkDirty();
    }

    public void MarkTutorialFinished() {
        PlayerData.isTutorialFinished = true;
        MarkDirty();
    }

    public void MarkLaunch() {
        PlayerData.isFirstLaunch = false;
        PlayerData.lastLoginDate = System.DateTime.Now.ToString("yyyy-MM-dd");
        MarkDirty();
    }

    public void ApplySettingsToRuntime() {
        // 静音只收口到 Master，其他分组音量仍然保留用户设置值。
        float masterVolume = Settings.isMute ? 0f : Settings.masterVolume;
        AudioManager.Instance.SetBusVolume(AudioBusType.Master, masterVolume);
        AudioManager.Instance.SetBusVolume(AudioBusType.Bgm, Settings.bgmVolume);
        AudioManager.Instance.SetBusVolume(AudioBusType.Sfx, Settings.sfxVolume);
        AudioManager.Instance.SetBusVolume(AudioBusType.UI, Settings.uiVolume);
        AudioManager.Instance.SetBusVolume(AudioBusType.Voice, Settings.voiceVolume);
        AudioManager.Instance.SetBusVolume(AudioBusType.Ambient, Settings.ambientVolume);
        Application.targetFrameRate = Settings.targetFrameRate;
    }
}
