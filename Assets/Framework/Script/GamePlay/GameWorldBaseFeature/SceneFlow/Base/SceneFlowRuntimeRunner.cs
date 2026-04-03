using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 鍦烘櫙鍒囨崲杩愯鏃舵墽琛屽櫒銆?/// 浣跨敤甯搁┗ MonoBehaviour 鎵胯浇鍗忕▼锛屼覆璧?Loading銆佽緭鍏ラ樆濉炲拰鍦烘櫙鍒囨崲銆?/// </summary>
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

        try {
            if (request.ShowLoadingUI) {
                UIManager.Instance.Open<LoadingPanel>();
                RefreshLoadingPanel(loadingContext);
            }

            if (request.BlockInput && inputFeatureManager != null) {
                loadingContext.Step = SceneLoadingStep.BlockInput;
                loadingContext.StepText = "Block Input";
                inputFeatureManager.SetExternalBlockActive(true);
            }

            loadingContext.Step = SceneLoadingStep.ClearUI;
            loadingContext.StepText = "Clear UI";
            loadingContext.Progress = 0.15f;
            // 杩涘叆鏂板満鏅墠鍏堟妸鏃?UI 鏀跺彛锛岄伩鍏嶆棫鐣岄潰娈嬬暀鍒版柊鍦烘櫙銆?
            if (request.ClearPopupUI) {
                UIManager.Instance.CloseByLayer(UILayer.Popup);
            }
            if (request.ClearNormalUI) {
                UIManager.Instance.CloseByLayer(UILayer.Normal);
            }
            if (!request.KeepHUD) {
                UIManager.Instance.CloseByLayer(UILayer.HUD);
            }

            RefreshLoadingPanel(loadingContext);
            yield return null;

            loadingContext.Step = SceneLoadingStep.LoadTarget;
            loadingContext.StepText = "Load Scene";
            loadingContext.Progress = 0.25f;

            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(config.ScenePath, LoadSceneMode.Single);
            if (asyncOperation == null) {
                loadingContext.Step = SceneLoadingStep.Failed;
                loadingContext.StepText = "Load Failed";
                loadingContext.ErrorMessage = "SceneManager.LoadSceneAsync returned null.";
                RefreshLoadingPanel(loadingContext);
                yield break;
            }

            while (!asyncOperation.isDone) {
                float sceneProgress = Mathf.Clamp01(asyncOperation.progress / 0.9f);
                loadingContext.Progress = 0.25f + sceneProgress * 0.7f;
                RefreshLoadingPanel(loadingContext);
                yield return null;
            }

            loadingContext.Step = SceneLoadingStep.EnterScene;
            loadingContext.StepText = "Enter Scene";
            loadingContext.Progress = 1f;
            SaveDataManager.Instance.SetLastScene(config.SceneId, config.ScenePath);
            CameraManager.Instance.RefreshSceneCamera();
            RefreshLoadingPanel(loadingContext);
            yield return null;

            loadingContext.Step = SceneLoadingStep.Completed;
            loadingContext.StepText = "Completed";
            loadingContext.Progress = 1f;
            RefreshLoadingPanel(loadingContext);
            yield return null;
        } finally {
            loadingContext.IsLoading = false;
            if (request.ShowLoadingUI) {
                UIManager.Instance.Close<LoadingPanel>();
            }
            if (request.BlockInput && inputFeatureManager != null) {
                inputFeatureManager.SetExternalBlockActive(false);
            }

            isLoading = false;
            Destroy(gameObject);
            instance = null;
        }
    }

    private static void RefreshLoadingPanel(SceneLoadingContext loadingContext) {
        LoadingPanel loadingPanel = UIManager.Instance.GetPanel<LoadingPanel>();
        if (loadingPanel != null) {
            loadingPanel.RefreshByContext(loadingContext);
        }
    }
}
