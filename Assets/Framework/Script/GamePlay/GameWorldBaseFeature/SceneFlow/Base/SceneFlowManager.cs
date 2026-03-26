using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 场景流转统一入口。
/// 负责构建场景请求、校验状态并触发运行时切换。
/// </summary>
public sealed class SceneFlowManager {
    private readonly SceneRegistry sceneRegistry;
    private readonly ClientInputFeatureManager inputFeatureManager;

    public SceneFlowManager(SceneRegistry registry, ClientInputFeatureManager targetInputFeatureManager) {
        sceneRegistry = registry;
        inputFeatureManager = targetInputFeatureManager;
    }

    public bool IsLoading() {
        return SceneFlowRuntimeRunner.Instance.IsLoading;
    }

    public SceneId GetCurrentSceneId() {
        Scene currentScene = SceneManager.GetActiveScene();
        if (sceneRegistry == null) {
            return SceneId.None;
        }

        SceneConfig config = sceneRegistry.GetConfigByPath(currentScene.path);
        if (config == null) {
            return SceneId.None;
        }

        return config.SceneId;
    }

    public bool ReloadCurrentScene(string reason) {
        SceneId currentSceneId = GetCurrentSceneId();
        if (currentSceneId == SceneId.None) {
            Debug.LogWarning("SceneFlowManager.ReloadCurrentScene failed because current scene is not registered.");
            return false;
        }

        SceneConfig config = sceneRegistry.GetConfig(currentSceneId);
        if (config == null) {
            return false;
        }

        SceneRequest request = BuildDefaultRequest(config, reason);
        request.ReloadIfSame = true;
        return LoadScene(request);
    }

    public bool LoadScene(SceneRequest request) {
        if (request == null) {
            return false;
        }

        if (IsLoading()) {
            Debug.LogWarning("SceneFlowManager ignored load request because a scene load is already in progress.");
            return false;
        }

        SceneConfig config = sceneRegistry.GetConfig(request.TargetSceneId);
        if (config == null) {
            Debug.LogError("SceneFlowManager could not find target scene config.");
            return false;
        }

        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.path == config.ScenePath && !request.ReloadIfSame) {
            Debug.LogWarning("SceneFlowManager ignored load request because target scene is already active.");
            return false;
        }

        SceneLoadingContext loadingContext = new SceneLoadingContext();
        loadingContext.CurrentSceneId = GetCurrentSceneId();
        loadingContext.TargetSceneId = request.TargetSceneId;
        loadingContext.PreviousSceneId = loadingContext.CurrentSceneId;
        loadingContext.Progress = 0f;
        loadingContext.IsLoading = true;
        loadingContext.Step = SceneLoadingStep.PrepareLeave;
        loadingContext.StepText = "Prepare Leave";
        loadingContext.Reason = request.Reason;

        return SceneFlowRuntimeRunner.Instance.BeginLoad(config, request, loadingContext, inputFeatureManager);
    }

    public SceneRequest BuildDefaultRequest(SceneConfig config, string reason) {
        SceneRequest request = new SceneRequest();
        request.TargetSceneId = config.SceneId;
        request.ReloadIfSame = false;
        request.ShowLoadingUI = config.DefaultShowLoadingUI;
        request.BlockInput = config.DefaultBlockInput;
        request.ClearNormalUI = config.DefaultClearNormalUI;
        request.ClearPopupUI = config.DefaultClearPopupUI;
        request.KeepHUD = config.DefaultKeepHUD;
        request.Reason = reason;
        return request;
    }
}
