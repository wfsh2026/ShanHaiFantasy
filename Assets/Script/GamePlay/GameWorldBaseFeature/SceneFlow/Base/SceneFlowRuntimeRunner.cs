using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class SceneFlowRuntimeRunner : MonoBehaviour {
    private static SceneFlowRuntimeRunner instance;

    private bool isLoading;

    public static SceneFlowRuntimeRunner Instance {
        get {
            if (instance == null) {
                GameObject runtimeObject = new GameObject("SceneFlowRuntimeRunner");
                Object.DontDestroyOnLoad(runtimeObject);
                instance = runtimeObject.AddComponent<SceneFlowRuntimeRunner>();
            }

            return instance;
        }
    }

    public bool IsLoading {
        get {
            return isLoading;
        }
    }

    public bool BeginLoad(SceneConfig config, SceneRequest request, SceneLoadingContext loadingContext, ClientInputFeatureManager inputFeatureManager) {
        if (isLoading || config == null || request == null || loadingContext == null) {
            return false;
        }

        StartCoroutine(CoLoadScene(config, request, loadingContext, inputFeatureManager));
        return true;
    }

    private IEnumerator CoLoadScene(SceneConfig config, SceneRequest request, SceneLoadingContext loadingContext, ClientInputFeatureManager inputFeatureManager) {
        isLoading = true;
        loadingContext.IsLoading = true;
        loadingContext.Step = SceneLoadingStep.ShowLoading;
        loadingContext.StepText = "Show Loading";
        loadingContext.Progress = 0.05f;

        if (request.ShowLoadingUI) {
            UIManager.Instance.Open<LoadingPanel>();
            LoadingPanel loadingPanel = UIManager.Instance.GetPanel<LoadingPanel>();
            if (loadingPanel != null) {
                loadingPanel.RefreshByContext(loadingContext);
            }
        }

        if (request.BlockInput && inputFeatureManager != null && inputFeatureManager.InputManager != null) {
            loadingContext.Step = SceneLoadingStep.BlockInput;
            loadingContext.StepText = "Block Input";
            inputFeatureManager.InputManager.SetContextActive(InputContextType.Block, true);
        }

        loadingContext.Step = SceneLoadingStep.ClearUI;
        loadingContext.StepText = "Clear UI";
        loadingContext.Progress = 0.15f;
        if (request.ClearPopupUI) {
            UIManager.Instance.CloseByLayer(UILayer.Popup);
        }
        if (request.ClearNormalUI) {
            UIManager.Instance.CloseByLayer(UILayer.Normal);
        }
        if (!request.KeepHUD) {
            UIManager.Instance.CloseByLayer(UILayer.HUD);
        }

        LoadingPanel loadingPanelAfterClear = UIManager.Instance.GetPanel<LoadingPanel>();
        if (loadingPanelAfterClear != null) {
            loadingPanelAfterClear.RefreshByContext(loadingContext);
        }

        yield return null;

        loadingContext.Step = SceneLoadingStep.LoadTarget;
        loadingContext.StepText = "Load Scene";
        loadingContext.Progress = 0.25f;

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(config.ScenePath, LoadSceneMode.Single);
        if (asyncOperation == null) {
            loadingContext.Step = SceneLoadingStep.Failed;
            loadingContext.StepText = "Load Failed";
            loadingContext.ErrorMessage = "SceneManager.LoadSceneAsync returned null.";
            loadingContext.IsLoading = false;
            isLoading = false;
            yield break;
        }

        while (!asyncOperation.isDone) {
            float sceneProgress = Mathf.Clamp01(asyncOperation.progress / 0.9f);
            loadingContext.Progress = 0.25f + sceneProgress * 0.7f;
            LoadingPanel loadingPanel = UIManager.Instance.GetPanel<LoadingPanel>();
            if (loadingPanel != null) {
                loadingPanel.RefreshByContext(loadingContext);
            }

            yield return null;
        }

        loadingContext.Step = SceneLoadingStep.EnterScene;
        loadingContext.StepText = "Enter Scene";
        loadingContext.Progress = 1f;
        SaveDataManager.Instance.SetLastScene(config.SceneId, config.ScenePath);
        yield return null;

        loadingContext.Step = SceneLoadingStep.Completed;
        loadingContext.StepText = "Completed";
        loadingContext.IsLoading = false;
        isLoading = false;

        Destroy(gameObject);
        instance = null;
    }
}
