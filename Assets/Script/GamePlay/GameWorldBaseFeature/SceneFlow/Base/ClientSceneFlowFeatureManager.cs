public sealed class ClientSceneFlowFeatureManager : AbsExtendGameWorldFeature {
    private SceneRegistry sceneRegistry;
    private SceneFlowManager sceneFlowManager;
    private ClientInputFeatureManager inputFeatureManager;
    private SceneFlowInputHandler inputHandler;

    public SceneFlowManager SceneFlowManager {
        get {
            return sceneFlowManager;
        }
    }

    protected override void OnInit() {
        ClientUIFeatureManager uiManager = gameWorld.GetExtendFeature<ClientUIFeatureManager>();
        inputFeatureManager = gameWorld.GetExtendFeature<ClientInputFeatureManager>();
        sceneRegistry = new SceneRegistry();
        sceneFlowManager = new SceneFlowManager(sceneRegistry, uiManager, inputFeatureManager);
        RegisterInputHandler();
    }

    protected override void OnRemove() {
        UnregisterInputHandler();
        inputFeatureManager = null;
        inputHandler = null;
        sceneFlowManager = null;
        sceneRegistry = null;
    }

    private void RegisterInputHandler() {
        if (inputFeatureManager == null || sceneFlowManager == null) {
            return;
        }

        inputHandler = new SceneFlowInputHandler(sceneFlowManager);
        inputFeatureManager.RegisterHandler(InputContextType.Global, inputHandler, 70);
    }

    private void UnregisterInputHandler() {
        if (inputFeatureManager == null || inputHandler == null) {
            return;
        }

        inputFeatureManager.UnregisterHandler(InputContextType.Global, inputHandler);
    }
}
