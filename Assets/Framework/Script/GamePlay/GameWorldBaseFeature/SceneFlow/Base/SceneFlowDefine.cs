/// <summary>
/// 已注册场景的逻辑标识。
/// </summary>
public enum SceneId {
    None = 0,
    UITest = 1,
    RoomEntry = 2,
    BattleTest = 3,
}

/// <summary>
/// 场景类型定义，供场景策略和后续扩展使用。
/// </summary>
public enum SceneType {
    Normal = 0,
    Lobby = 1,
    Battle = 2,
    Result = 3,
    Loading = 4,
}

/// <summary>
/// 场景加载流程中的运行步骤。
/// 主要用于 Loading 显示和调试排查。
/// </summary>
public enum SceneLoadingStep {
    None = 0,
    PrepareLeave = 1,
    ShowLoading = 2,
    ClearUI = 3,
    BlockInput = 4,
    LoadTarget = 5,
    EnterScene = 6,
    Completed = 7,
    Failed = 8,
}
