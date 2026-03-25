public sealed class ClientConfigFeatureManager : AbsExtendGameWorldFeature {
    protected override void OnInit() {
        ConfigManager.Instance.Initialize();
        ConfigManager.Instance.PreloadDefaults();
    }

    protected override void OnRemove() {
        ConfigManager.Instance.Clear();
    }
}
