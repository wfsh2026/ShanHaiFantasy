/// <summary>
/// UI 层级定义。
/// </summary>
public enum UILayer {
    Background = 0,
    Normal = 1,
    Popup = 2,
    HUD = 3,
    Toast = 4,
    Guide = 5,
    System = 6
}

/// <summary>
/// UI 关闭后的缓存策略。
/// </summary>
public enum UICacheMode {
    DestroyOnClose = 0,
    HideOnClose = 1,
    Permanent = 2
}

/// <summary>
/// UI 打开模式定义。
/// </summary>
public enum UIOpenMode {
    Single = 0,
    Multi = 1
}
