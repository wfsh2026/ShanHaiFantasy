using System.Collections.Generic;

/// <summary>
/// 场景注册表。
/// 统一维护场景标识与默认切换配置的对应关系。
/// </summary>
public sealed class SceneRegistry {
    private readonly Dictionary<SceneId, SceneConfig> configById = new Dictionary<SceneId, SceneConfig>();
    private readonly Dictionary<string, SceneConfig> configByPath = new Dictionary<string, SceneConfig>();

    public SceneRegistry() {
        RegisterDefaultConfigs();
    }

    public SceneConfig GetConfig(SceneId sceneId) {
        SceneConfig config;
        if (configById.TryGetValue(sceneId, out config)) {
            return config;
        }

        return null;
    }

    public SceneConfig GetConfigByPath(string scenePath) {
        if (string.IsNullOrEmpty(scenePath)) {
            return null;
        }

        SceneConfig config;
        if (configByPath.TryGetValue(scenePath, out config)) {
            return config;
        }

        return null;
    }

    private void Register(SceneConfig config) {
        if (config == null || string.IsNullOrEmpty(config.ScenePath)) {
            return;
        }

        configById[config.SceneId] = config;
        configByPath[config.ScenePath] = config;
    }

    private void RegisterDefaultConfigs() {
        SceneConfig uiTestConfig = new SceneConfig();
        uiTestConfig.SceneId = SceneId.UITest;
        uiTestConfig.ScenePath = "Assets/Content/Scene/Test/UITestScene.unity/UITestScene.unity";
        uiTestConfig.SceneType = SceneType.Normal;
        uiTestConfig.DefaultShowLoadingUI = true;
        uiTestConfig.DefaultBlockInput = true;
        uiTestConfig.DefaultClearNormalUI = true;
        uiTestConfig.DefaultClearPopupUI = true;
        uiTestConfig.DefaultKeepHUD = false;
        uiTestConfig.AllowReload = true;
        uiTestConfig.Description = "Base feature test scene";
        Register(uiTestConfig);

        SceneConfig roomEntryConfig = new SceneConfig();
        roomEntryConfig.SceneId = SceneId.RoomEntry;
        roomEntryConfig.ScenePath = "Assets/Content/Scene/Lobby/RoomEntryScene.unity/RoomEntryScene.unity/RoomEntryScene.unity";
        roomEntryConfig.SceneType = SceneType.Lobby;
        roomEntryConfig.DefaultShowLoadingUI = true;
        roomEntryConfig.DefaultBlockInput = true;
        roomEntryConfig.DefaultClearNormalUI = true;
        roomEntryConfig.DefaultClearPopupUI = true;
        roomEntryConfig.DefaultKeepHUD = false;
        roomEntryConfig.AllowReload = true;
        roomEntryConfig.Description = "A1 room entry scene";
        Register(roomEntryConfig);

        SceneConfig battleTestConfig = new SceneConfig();
        battleTestConfig.SceneId = SceneId.BattleTest;
        battleTestConfig.ScenePath = "Assets/Content/Scene/Test/BattleTestScene.unity/BattleTestScene.unity";
        battleTestConfig.SceneType = SceneType.Battle;
        battleTestConfig.DefaultShowLoadingUI = true;
        battleTestConfig.DefaultBlockInput = true;
        battleTestConfig.DefaultClearNormalUI = true;
        battleTestConfig.DefaultClearPopupUI = true;
        battleTestConfig.DefaultKeepHUD = false;
        battleTestConfig.AllowReload = true;
        battleTestConfig.Description = "A1 battle placeholder scene";
        Register(battleTestConfig);
    }
}
