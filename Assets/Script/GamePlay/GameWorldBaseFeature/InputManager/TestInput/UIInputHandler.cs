/// <summary>
/// 普通 UI 输入处理器。
/// 当前主要处理主面板的关闭逻辑。
/// </summary>
public sealed class UIInputHandler : IInputHandler {
    public bool HandleInput(InputManager inputManager, InputState inputState) {
        if (inputManager == null || inputState == null) {
            return false;
        }

        if (UIManager.Instance.IsOpen<AdjustAttrPopup>()) {
            return false;
        }

        if (!UIManager.Instance.IsOpen<RoleAttrPanel>()) {
            return false;
        }

        if (inputManager.GetButtonDown(InputActionId.Cancel)) {
            UIManager.Instance.CloseTop();
            return true;
        }

        return false;
    }
}
