public sealed class ClientTestModeManager : ClientModeManager {
    private ClientTestModeData data;
    private ClientTestModeLogic logic;
    private ClientUIFeatureManager uiManager;
    private ClientInputFeatureManager inputFeatureManager;
    private TestModeInputHandler inputHandler;

    protected override void OnInit() {
        base.OnInit();

        data = AddData<ClientTestModeData>();
        AddLogic<ClientTestModeLogic>();
        AddStage<ClientTestModeStage>();
        ChangeStage<ClientTestModeStage>();

        logic = GetLogic<ClientTestModeLogic>();
        uiManager = gameWorld.GetExtendFeature<ClientUIFeatureManager>();
        inputFeatureManager = gameWorld.GetExtendFeature<ClientInputFeatureManager>();
        AudioManager.Instance.PlayBgm("test_bgm");
        RegisterInputHandler();
        OpenModeUI();
    }

    protected override void OnRemove() {
        UnregisterInputHandler();
        CloseModeUI();
        inputFeatureManager = null;
        inputHandler = null;
        uiManager = null;
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
        if (uiManager == null) {
            return;
        }

        uiManager.Open<RoleAttrHUDPanel, RoleAttrHUDOpenData>(new RoleAttrHUDOpenData());
        uiManager.Open<RoleAttrPanel, RoleAttrPanelOpenData>(new RoleAttrPanelOpenData());
    }

    private void CloseModeUI() {
        if (uiManager == null || uiManager.GameWorld == null) {
            return;
        }

        uiManager.Close<RoleAttrPanel>();
        uiManager.Close<RoleAttrHUDPanel>();
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
