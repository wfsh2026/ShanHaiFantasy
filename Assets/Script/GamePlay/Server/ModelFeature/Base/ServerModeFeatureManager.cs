public sealed class ServerModeFeatureManager : AbsExtendGameWorldFeature {
    private const ModeType DEFAULT_MODE_TYPE = ModeType.Test;

    protected override void OnInit() {
        ServerModeFactory.InitMode(gameWorld, DEFAULT_MODE_TYPE);
    }

    protected override void OnRemove() {
    }
}
