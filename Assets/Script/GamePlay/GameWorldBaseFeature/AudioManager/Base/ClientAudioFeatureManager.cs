public sealed class ClientAudioFeatureManager : AbsExtendGameWorldFeature {
    protected override void OnInit() {
        AudioManager.Instance.Initialize();
        gameWorld.AddUpdate(OnUpdate);
    }

    protected override void OnRemove() {
        if (gameWorld != null) {
            gameWorld.RemoveUpdate(OnUpdate);
        }
    }

    private void OnUpdate(float delta) {
        AudioManager.Instance.Tick(delta);
    }
}
