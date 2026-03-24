public sealed class UIInputHandler : IInputHandler {
    private readonly ClientUIFeatureManager uiManager;

    public UIInputHandler(ClientUIFeatureManager targetUIManager) {
        uiManager = targetUIManager;
    }

    public bool HandleInput(InputManager inputManager, InputState inputState) {
        if (uiManager == null || inputManager == null || inputState == null) {
            return false;
        }

        if (uiManager.IsOpen<AdjustAttrPopup>()) {
            return false;
        }

        if (!uiManager.IsOpen<RoleAttrPanel>()) {
            return false;
        }

        if (inputManager.GetButtonDown(InputActionId.Cancel)) {
            uiManager.CloseTop();
            return true;
        }

        return false;
    }
}
