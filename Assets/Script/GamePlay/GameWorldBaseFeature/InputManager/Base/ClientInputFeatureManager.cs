public sealed class ClientInputFeatureManager : AbsExtendGameWorldFeature {
    private InputManager inputManager;
    private ClientUIFeatureManager uiManager;
    private GlobalInputHandler globalInputHandler;
    private PopupInputHandler popupInputHandler;
    private UIInputHandler uiInputHandler;

    public InputManager InputManager {
        get {
            return inputManager;
        }
    }

    protected override void OnInit() {
        uiManager = gameWorld.GetExtendFeature<ClientUIFeatureManager>();
        inputManager = new InputManager();
        globalInputHandler = new GlobalInputHandler(uiManager);
        popupInputHandler = new PopupInputHandler(uiManager);
        uiInputHandler = new UIInputHandler(uiManager);

        inputManager.RegisterHandler(InputContextType.Popup, popupInputHandler, 100);
        inputManager.RegisterHandler(InputContextType.UI, uiInputHandler, 90);
        inputManager.RegisterHandler(InputContextType.Global, globalInputHandler, 80);
        gameWorld.AddUpdate(OnUpdate);
    }

    protected override void OnRemove() {
        if (gameWorld != null) {
            gameWorld.RemoveUpdate(OnUpdate);
        }

        if (inputManager != null) {
            inputManager.UnregisterHandler(InputContextType.Popup, popupInputHandler);
            inputManager.UnregisterHandler(InputContextType.UI, uiInputHandler);
            inputManager.UnregisterHandler(InputContextType.Global, globalInputHandler);
            inputManager.Clear();
        }

        uiManager = null;
        globalInputHandler = null;
        popupInputHandler = null;
        uiInputHandler = null;
        inputManager = null;
    }

    public void RegisterHandler(InputContextType contextType, IInputHandler handler, int priority) {
        if (inputManager != null) {
            inputManager.RegisterHandler(contextType, handler, priority);
        }
    }

    public void UnregisterHandler(InputContextType contextType, IInputHandler handler) {
        if (inputManager != null) {
            inputManager.UnregisterHandler(contextType, handler);
        }
    }

    private void OnUpdate(float delta) {
        if (inputManager == null) {
            return;
        }

        UpdateContexts();
        inputManager.Tick(delta);
    }

    private void UpdateContexts() {
        bool hasPopup = uiManager != null && uiManager.IsOpen<AdjustAttrPopup>();
        bool hasMainPanel = uiManager != null && uiManager.IsOpen<RoleAttrPanel>();

        inputManager.SetContextActive(InputContextType.Block, false);
        inputManager.SetContextActive(InputContextType.Popup, hasPopup);
        inputManager.SetContextActive(InputContextType.UI, hasPopup || hasMainPanel);
        inputManager.SetContextActive(InputContextType.Global, true);
        inputManager.SetContextActive(InputContextType.Mode, !hasPopup);
    }
}
