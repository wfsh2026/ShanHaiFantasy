using UnityEngine;

/// <summary>
/// 测试模式管理器。
/// 负责组装测试模式的 Data、Logic、Stage，并拉起对应的 UI、输入和音频演示。
/// </summary>
public sealed class ClientTestModeManager : ClientModeManager {
    private const string CAMERA_TARGET_NAME = "UITestCameraTarget";

    private ClientTestModeData data;
    private ClientTestModeLogic logic;
    private ClientInputFeatureManager inputFeatureManager;
    private TestModeInputHandler inputHandler;
    private GameObject cameraTargetObject;

    protected override void OnInit() {
        base.OnInit();

        data = AddData<ClientTestModeData>();
        AddLogic<ClientTestModeLogic>();
        AddStage<ClientTestModeStage>();
        ChangeStage<ClientTestModeStage>();

        logic = GetLogic<ClientTestModeLogic>();
        inputFeatureManager = gameWorld.GetExtendFeature<ClientInputFeatureManager>();
        SaveDataManager.Instance.SetLastMode(GetType().Name);
        SaveDataManager.Instance.MarkLaunch();
        AudioManager.Instance.PlayBgm("test_bgm");
        BindCameraTarget();
        RegisterInputHandler();
        OpenModeUI();
    }

    protected override void OnRemove() {
        UnregisterInputHandler();
        CloseModeUI();
        inputFeatureManager = null;
        inputHandler = null;
        data = null;
        logic = null;
        cameraTargetObject = null;
        base.OnRemove();
    }

    protected override void OnUpdate(float delta) {
        base.OnUpdate(delta);
        if (logic != null) {
            logic.Tick(delta);
        }
    }

    protected override void OnChangeStage(string stageName) {
        base.OnChangeStage(stageName);
        if (data != null) {
            data.SetStageName(stageName);
        }
    }

    private void OpenModeUI() {
        // 测试模式启动后默认同时打开 HUD 和主属性面板，方便一起验证绑定链。
        UIManager.Instance.Open<RoleAttrHUDPanel>();
        UIManager.Instance.Open<RoleAttrPanel>();
    }

    private void CloseModeUI() {
        UIManager.Instance.Close<RoleAttrPanel>();
        UIManager.Instance.Close<RoleAttrHUDPanel>();
    }

    private void RegisterInputHandler() {
        if (inputFeatureManager == null) {
            return;
        }

        inputHandler = new TestModeInputHandler(new TestModeInputService(gameWorld));
        inputFeatureManager.RegisterHandler(InputContextType.Mode, inputHandler, 60);
    }

    private void UnregisterInputHandler() {
        if (inputFeatureManager == null || inputHandler == null) {
            return;
        }

        inputFeatureManager.UnregisterHandler(InputContextType.Mode, inputHandler);
    }

    private void BindCameraTarget() {
        cameraTargetObject = EnsureCameraTargetObject();
        if (cameraTargetObject == null) {
            return;
        }

        CameraManager.Instance.RefreshSceneCamera();
        CameraManager.Instance.SetFollowTarget(cameraTargetObject.transform);
        CameraManager.Instance.SetLookAtTarget(cameraTargetObject.transform);
    }

    private static GameObject EnsureCameraTargetObject() {
        GameObject targetObject = GameObject.Find(CAMERA_TARGET_NAME);
        if (targetObject != null) {
            return targetObject;
        }

        targetObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        targetObject.name = CAMERA_TARGET_NAME;
        targetObject.transform.position = new UnityEngine.Vector3(0f, 1.2f, 0f);
        targetObject.transform.localScale = new UnityEngine.Vector3(0.75f, 0.75f, 0.75f);

        Renderer renderer = targetObject.GetComponent<Renderer>();
        if (renderer != null) {
            renderer.material.color = new UnityEngine.Color(1f, 0.78f, 0.22f, 1f);
        }

        return targetObject;
    }
}
