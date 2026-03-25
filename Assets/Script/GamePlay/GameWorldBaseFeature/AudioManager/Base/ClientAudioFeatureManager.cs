/// <summary>
/// 音频系统的客户端挂载入口。
/// 负责初始化 AudioManager 并驱动播放实例回收。
/// </summary>
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
