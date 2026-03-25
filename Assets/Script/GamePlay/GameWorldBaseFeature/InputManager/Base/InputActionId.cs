/// <summary>
/// 输入动作枚举。
/// 业务层只识别动作，不直接识别底层按键。
/// </summary>
public enum InputActionId {
    None = 0,
    Cancel = 1,
    Confirm = 2,
    ToggleRoleAttrPanel = 3,
    ToggleDebugUI = 4,
    NextStage = 5,
    ReloadScene = 6,
    AddHP = 7,
    ReduceHP = 8,
    AddMP = 9,
    ReduceMP = 10,
    MoveHorizontal = 11,
    MoveVertical = 12,
    MouseLeft = 13,
    MouseRight = 14,
    MouseX = 15,
    MouseY = 16,
    MouseScroll = 17,
}
