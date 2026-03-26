public sealed class ClientModeFeatureManager : AbsExtendGameWorldFeature {
    private const ModeType DEFAULT_MODE_TYPE = ModeType.Test;

    protected override void OnInit() {
        ClientModeFactory.InitMode(gameWorld, DEFAULT_MODE_TYPE);
    }

    protected override void OnRemove() {
    }
}
