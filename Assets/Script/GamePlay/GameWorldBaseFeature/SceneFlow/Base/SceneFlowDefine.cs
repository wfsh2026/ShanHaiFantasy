public enum SceneId {
    None = 0,
    UITest = 1,
}

public enum SceneType {
    Normal = 0,
    Lobby = 1,
    Battle = 2,
    Result = 3,
    Loading = 4,
}

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
