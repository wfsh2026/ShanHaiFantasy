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
