using UnityEngine;

/// <summary>
/// 输入绑定类型。
/// 分别对应键盘按键、轴输入和鼠标按键。
/// </summary>
public enum InputBindingType {
    Key = 0,
    Axis = 1,
    MouseButton = 2,
}

/// <summary>
/// 单个输入动作的绑定定义。
/// 负责描述某个动作如何映射到底层输入设备。
/// </summary>
public sealed class InputBinding {
    public InputActionId ActionId {
        get;
        private set;
    }

    public InputBindingType BindingType {
        get;
        private set;
    }

    public KeyCode PrimaryKey {
        get;
        private set;
    }

    public KeyCode SecondaryKey {
        get;
        private set;
    }

    public int MouseButton {
        get;
        private set;
    }

    public string AxisName {
        get;
        private set;
    }

    private InputBinding() {
        PrimaryKey = KeyCode.None;
        SecondaryKey = KeyCode.None;
        MouseButton = -1;
        AxisName = string.Empty;
    }

    public static InputBinding CreateKey(InputActionId actionId, KeyCode primaryKey, KeyCode secondaryKey = KeyCode.None) {
        InputBinding binding = new InputBinding();
        binding.ActionId = actionId;
        binding.BindingType = InputBindingType.Key;
        binding.PrimaryKey = primaryKey;
        binding.SecondaryKey = secondaryKey;
        return binding;
    }

    public static InputBinding CreateMouseButton(InputActionId actionId, int mouseButton) {
        InputBinding binding = new InputBinding();
        binding.ActionId = actionId;
        binding.BindingType = InputBindingType.MouseButton;
        binding.MouseButton = mouseButton;
        return binding;
    }

    public static InputBinding CreateAxis(InputActionId actionId, string axisName) {
        InputBinding binding = new InputBinding();
        binding.ActionId = actionId;
        binding.BindingType = InputBindingType.Axis;
        binding.AxisName = axisName;
        return binding;
    }
}
