/// <summary>
/// 测试模式管理器。
/// 负责组装测试模式的 Data、Logic、Stage，并拉起对应的 UI、输入和音频演示。
/// </summary>
public sealed class ClientTestModeManager : ClientModeManager {
    private ClientTestModeData data;
    private ClientTestModeLogic logic;
    private ClientInputFeatureManager inputFeatureManager;
    private TestModeInputHandler inputHandler;

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
}
