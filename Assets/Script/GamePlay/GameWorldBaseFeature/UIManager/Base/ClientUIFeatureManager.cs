/// <summary>
/// UI 系统的客户端挂载入口。
/// 只负责把 UIManager 与当前 GameWorld 生命周期绑定在一起。
/// </summary>
public sealed class ClientUIFeatureManager : AbsExtendGameWorldFeature {
    protected override void OnInit() {
        UIManager.Instance.Bind(gameWorld);
    }

    protected override void OnRemove() {
        UIManager.Instance.Unbind();
    }
}
