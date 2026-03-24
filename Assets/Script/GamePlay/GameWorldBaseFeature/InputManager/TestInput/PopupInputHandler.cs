public sealed class PopupInputHandler : IInputHandler {
    private readonly ClientUIFeatureManager uiManager;

    public PopupInputHandler(ClientUIFeatureManager targetUIManager) {
        uiManager = targetUIManager;
    }

    public bool HandleInput(InputManager inputManager, InputState inputState) {
        if (uiManager == null || inputManager == null || inputState == null) {
            return false;
        }

        if (!uiManager.IsOpen<AdjustAttrPopup>()) {
            return false;
        }

        if (inputManager.GetButtonDown(InputActionId.Cancel)) {
            uiManager.CloseTop();
            return true;
        }

        return false;
    }
}
