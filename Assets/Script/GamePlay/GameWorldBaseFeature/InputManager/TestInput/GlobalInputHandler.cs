public sealed class GlobalInputHandler : IInputHandler {
    private readonly ClientUIFeatureManager uiManager;

    public GlobalInputHandler(ClientUIFeatureManager targetUIManager) {
        uiManager = targetUIManager;
    }

    public bool HandleInput(InputManager inputManager, InputState inputState) {
        if (uiManager == null || inputManager == null || inputState == null) {
            return false;
        }

        if (!inputManager.GetButtonDown(InputActionId.ToggleRoleAttrPanel)) {
            return false;
        }

        if (uiManager.IsOpen<AdjustAttrPopup>()) {
            return true;
        }

        if (uiManager.IsOpen<RoleAttrPanel>()) {
            uiManager.Close<RoleAttrPanel>();
        } else {
            uiManager.Open<RoleAttrPanel, RoleAttrPanelOpenData>(new RoleAttrPanelOpenData {
                OpenSource = "InputToggle",
            });
        }

        return true;
    }
}
