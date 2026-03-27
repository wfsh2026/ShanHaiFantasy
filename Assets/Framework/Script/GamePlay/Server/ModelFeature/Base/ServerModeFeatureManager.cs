public sealed class ServerModeFeatureManager : AbsExtendGameWorldFeature {
    protected override void OnInit() {
        ServerModeFactory.InitMode(gameWorld, ModeType.None);
    }

    protected override void OnRemove() {
    }
}
