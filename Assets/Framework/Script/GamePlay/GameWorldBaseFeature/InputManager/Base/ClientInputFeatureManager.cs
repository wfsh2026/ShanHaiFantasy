/// <summary>
/// 杈撳叆绯荤粺鐨勫鎴风鎸傝浇鍏ュ彛銆?/// 璐熻矗鍒涘缓杈撳叆绠＄悊鍣ㄣ€佹敞鍐岄粯璁?Handler锛屽苟鏍规嵁 UI 鐘舵€佸垏鎹笂涓嬫枃銆?/// </summary>
public sealed class ClientInputFeatureManager : AbsExtendGameWorldFeature {
    private InputManager inputManager;
    private GlobalInputHandler globalInputHandler;
    private PopupInputHandler popupInputHandler;
    private UIInputHandler uiInputHandler;
    private bool isExternalBlockActive;

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
        isExternalBlockActive = false;

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
        isExternalBlockActive = false;
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

    public void SetExternalBlockActive(bool isActive) {
        isExternalBlockActive = isActive;
        if (inputManager != null) {
            inputManager.SetContextActive(InputContextType.Block, isExternalBlockActive);
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

        // 褰撳墠绀轰緥閲屼笂涓嬫枃浼樺厛绾у浐瀹氫负 Block > Popup > UI > Global > Mode銆?
        inputManager.SetContextActive(InputContextType.Block, isExternalBlockActive);
        inputManager.SetContextActive(InputContextType.Popup, hasPopup);
        inputManager.SetContextActive(InputContextType.UI, hasPopup || hasMainPanel);
        inputManager.SetContextActive(InputContextType.Global, true);
        inputManager.SetContextActive(InputContextType.Mode, !hasPopup);
    }
}
