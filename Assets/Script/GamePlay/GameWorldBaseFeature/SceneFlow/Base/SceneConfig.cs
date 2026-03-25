/// <summary>
/// 单个场景的默认流转配置。
/// 这里只描述切场景策略，不放运行时状态。
/// </summary>
public sealed class SceneConfig {
    public SceneId SceneId;
    public string ScenePath;
    public SceneType SceneType;
    public bool DefaultShowLoadingUI;
    public bool DefaultBlockInput;
    public bool DefaultClearNormalUI;
    public bool DefaultClearPopupUI;
    public bool DefaultKeepHUD;
    public bool AllowReload;
    public string Description;
}
