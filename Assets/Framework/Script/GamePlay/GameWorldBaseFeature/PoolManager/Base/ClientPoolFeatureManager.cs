/// <summary>
/// 对象池系统客户端接入入口。
/// 负责初始化和清理 PoolManager。
/// </summary>
public sealed class ClientPoolFeatureManager : AbsExtendGameWorldFeature {
    protected override void OnInit() {
        PoolManager.Instance.Initialize();
    }

    protected override void OnRemove() {
        PoolManager.Instance.ClearAll();
    }
}
