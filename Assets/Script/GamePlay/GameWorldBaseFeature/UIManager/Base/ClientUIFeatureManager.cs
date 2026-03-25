public sealed class ClientUIFeatureManager : AbsExtendGameWorldFeature {
    protected override void OnInit() {
        UIManager.Instance.Bind(gameWorld);
    }

    protected override void OnRemove() {
        UIManager.Instance.Unbind();
    }
}
