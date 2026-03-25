public sealed class ClientInputFeatureManager : AbsExtendGameWorldFeature {
    private InputManager inputManager;
    private GlobalInputHandler globalInputHandler;
    private PopupInputHandler popupInputHandler;
    private UIInputHandler uiInputHandler;

    public InputManager InputManager {
        get {
            return inputManager;
        }
    }

    protected override void OnInit() {
        inputManager = new InputManager();
        globalInputHandler = new GlobalInputHandler();
        popupInputHandler = new PopupInputHandler();
        uiInputHandler = new UIInputHandler();

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
        bool hasPopup = UIManager.Instance.IsOpen<AdjustAttrPopup>();
        bool hasMainPanel = UIManager.Instance.IsOpen<RoleAttrPanel>();

        inputManager.SetContextActive(InputContextType.Block, false);
        inputManager.SetContextActive(InputContextType.Popup, hasPopup);
        inputManager.SetContextActive(InputContextType.UI, hasPopup || hasMainPanel);
        inputManager.SetContextActive(InputContextType.Global, true);
        inputManager.SetContextActive(InputContextType.Mode, !hasPopup);
    }
}
