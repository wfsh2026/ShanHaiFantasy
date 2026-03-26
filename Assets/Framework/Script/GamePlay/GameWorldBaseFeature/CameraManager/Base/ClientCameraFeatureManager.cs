/// <summary>
/// 相机系统的客户端挂载入口。
/// 负责初始化 CameraManager 并在每帧驱动基础相机更新。
/// </summary>
public sealed class ClientCameraFeatureManager : AbsExtendGameWorldFeature {
    protected override void OnInit() {
        CameraManager.Instance.Initialize();
        gameWorld.AddUpdate(OnUpdate);
    }

    protected override void OnRemove() {
        if (gameWorld != null) {
            gameWorld.RemoveUpdate(OnUpdate);
        }

        CameraManager.Instance.Clear();
    }

    private void OnUpdate(float delta) {
        CameraManager.Instance.Tick(delta);
    }
}
