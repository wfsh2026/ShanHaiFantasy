using System.Collections.Generic;

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
        uiTestConfig.ScenePath = "Assets/Scenes/UITestScene.unity/UITestScene.unity";
        uiTestConfig.SceneType = SceneType.Normal;
        uiTestConfig.DefaultShowLoadingUI = true;
        uiTestConfig.DefaultBlockInput = true;
        uiTestConfig.DefaultClearNormalUI = true;
        uiTestConfig.DefaultClearPopupUI = true;
        uiTestConfig.DefaultKeepHUD = false;
        uiTestConfig.AllowReload = true;
        uiTestConfig.Description = "UI framework test scene";
        Register(uiTestConfig);
    }
}
