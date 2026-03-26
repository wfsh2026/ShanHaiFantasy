/// <summary>
/// 当前场景切换过程的运行时上下文。
/// Loading 面板和调试输出都基于这份数据。
/// </summary>
public sealed class SceneLoadingContext {
    public SceneId CurrentSceneId;
    public SceneId TargetSceneId;
    public SceneId PreviousSceneId;
    public float Progress;
    public bool IsLoading;
    public SceneLoadingStep Step;
    public string StepText;
    public string ErrorMessage;
    public string Reason;
}
