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
