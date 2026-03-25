public sealed class GlobalInputHandler : IInputHandler {
    public bool HandleInput(InputManager inputManager, InputState inputState) {
        if (inputManager == null || inputState == null) {
            return false;
        }

        if (!inputManager.GetButtonDown(InputActionId.ToggleRoleAttrPanel)) {
            return false;
        }

        if (UIManager.Instance.IsOpen<AdjustAttrPopup>()) {
            return true;
        }

        if (UIManager.Instance.IsOpen<RoleAttrPanel>()) {
            UIManager.Instance.Close<RoleAttrPanel>();
        } else {
            UIManager.Instance.Open<RoleAttrPanel>();
        }

        return true;
    }
}
