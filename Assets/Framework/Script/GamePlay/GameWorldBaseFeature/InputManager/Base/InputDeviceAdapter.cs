using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 旧版 Unity 输入系统适配层。
/// 项目内只有这一层允许直接读取 UnityEngine.Input。
/// </summary>
public sealed class InputDeviceAdapter {
    public void UpdateState(InputMap inputMap, InputState inputState) {
        if (inputMap == null || inputState == null) {
            return;
        }

        inputState.BeginFrame();

        List<InputBinding> bindings = inputMap.GetBindings();
        for (int i = 0; i < bindings.Count; ++i) {
            InputBinding binding = bindings[i];
            switch (binding.BindingType) {
                case InputBindingType.Key:
                    UpdateKeyBinding(inputState, binding);
                    break;
                case InputBindingType.Axis:
                    UpdateAxisBinding(inputState, binding);
                    break;
                case InputBindingType.MouseButton:
                    UpdateMouseButtonBinding(inputState, binding);
                    break;
            }
        }
    }

    private static void UpdateKeyBinding(InputState inputState, InputBinding binding) {
        bool isDown = false;
        bool isPressing = false;
        bool isUp = false;

        if (binding.PrimaryKey != KeyCode.None) {
            isDown = isDown || Input.GetKeyDown(binding.PrimaryKey);
            isPressing = isPressing || Input.GetKey(binding.PrimaryKey);
            isUp = isUp || Input.GetKeyUp(binding.PrimaryKey);
        }

        if (binding.SecondaryKey != KeyCode.None) {
            isDown = isDown || Input.GetKeyDown(binding.SecondaryKey);
            isPressing = isPressing || Input.GetKey(binding.SecondaryKey);
            isUp = isUp || Input.GetKeyUp(binding.SecondaryKey);
        }

        inputState.SetButtonState(binding.ActionId, isDown, isPressing, isUp);
    }

    private static void UpdateAxisBinding(InputState inputState, InputBinding binding) {
        if (string.IsNullOrEmpty(binding.AxisName)) {
            return;
        }

        inputState.SetAxisValue(binding.ActionId, Input.GetAxisRaw(binding.AxisName));
    }

    private static void UpdateMouseButtonBinding(InputState inputState, InputBinding binding) {
        if (binding.MouseButton < 0) {
            return;
        }

        bool isDown = Input.GetMouseButtonDown(binding.MouseButton);
        bool isPressing = Input.GetMouseButton(binding.MouseButton);
        bool isUp = Input.GetMouseButtonUp(binding.MouseButton);
        inputState.SetButtonState(binding.ActionId, isDown, isPressing, isUp);
    }
}
