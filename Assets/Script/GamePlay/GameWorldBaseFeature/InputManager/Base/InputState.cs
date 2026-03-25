using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 输入快照。
/// 只保存当前帧的按钮状态、轴值和鼠标相关数据。
/// </summary>
public sealed class InputState {
    private readonly Dictionary<InputActionId, InputButtonState> buttonStates = new Dictionary<InputActionId, InputButtonState>();
    private readonly Dictionary<InputActionId, float> axisValues = new Dictionary<InputActionId, float>();

    public Vector3 MousePosition {
        get;
        private set;
    }

    public float MouseScrollValue {
        get;
        private set;
    }

    public void BeginFrame() {
        buttonStates.Clear();
        axisValues.Clear();
        MousePosition = Input.mousePosition;
        MouseScrollValue = 0f;
    }

    public void SetButtonState(InputActionId actionId, bool isDown, bool isPressing, bool isUp) {
        InputButtonState state;
        buttonStates.TryGetValue(actionId, out state);
        // 同一动作可能被多个按键映射命中，因此这里做按位合并。
        state.IsDown = state.IsDown || isDown;
        state.IsPressing = state.IsPressing || isPressing;
        state.IsUp = state.IsUp || isUp;
        buttonStates[actionId] = state;
    }

    public void SetAxisValue(InputActionId actionId, float value) {
        axisValues[actionId] = value;
        if (actionId == InputActionId.MouseScroll) {
            MouseScrollValue = value;
        }
    }

    public bool GetButtonDown(InputActionId actionId) {
        InputButtonState state;
        if (buttonStates.TryGetValue(actionId, out state)) {
            return state.IsDown;
        }

        return false;
    }

    public bool GetButton(InputActionId actionId) {
        InputButtonState state;
        if (buttonStates.TryGetValue(actionId, out state)) {
            return state.IsPressing;
        }

        return false;
    }

    public bool GetButtonUp(InputActionId actionId) {
        InputButtonState state;
        if (buttonStates.TryGetValue(actionId, out state)) {
            return state.IsUp;
        }

        return false;
    }

    public float GetAxis(InputActionId actionId) {
        float value;
        if (axisValues.TryGetValue(actionId, out value)) {
            return value;
        }

        return 0f;
    }
}
