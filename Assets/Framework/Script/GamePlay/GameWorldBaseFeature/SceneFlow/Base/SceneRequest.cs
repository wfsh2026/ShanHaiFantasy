/// <summary>
/// 一次场景切换请求。
/// 这里描述目标场景和本次切换使用的策略开关。
/// </summary>
public sealed class SceneRequest {
    public SceneId TargetSceneId;
    public bool ReloadIfSame;
    public bool ShowLoadingUI;
    public bool BlockInput;
    public bool ClearNormalUI;
    public bool ClearPopupUI;
    public bool KeepHUD;
    public string Reason;
}
