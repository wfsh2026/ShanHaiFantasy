/// <summary>
/// Popup 输入处理器。
/// 当前负责弹窗打开时的取消返回键。
/// </summary>
public sealed class PopupInputHandler : IInputHandler {
    public bool HandleInput(InputManager inputManager, InputState inputState) {
        if (inputManager == null || inputState == null) {
            return false;
        }

        if (!UIManager.Instance.IsOpen<AdjustAttrPopup>()) {
            return false;
        }

        if (inputManager.GetButtonDown(InputActionId.Cancel)) {
            UIManager.Instance.CloseTop();
            return true;
        }

        return false;
    }
}
