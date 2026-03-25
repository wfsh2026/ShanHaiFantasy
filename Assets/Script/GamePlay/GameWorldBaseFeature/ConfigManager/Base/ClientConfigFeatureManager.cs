/// <summary>
/// 配置系统的客户端挂载入口。
/// 这里只负责初始化和预加载默认配置，不承载业务配置逻辑。
/// </summary>
public sealed class ClientConfigFeatureManager : AbsExtendGameWorldFeature {
    protected override void OnInit() {
        ConfigManager.Instance.Initialize();
        ConfigManager.Instance.PreloadDefaults();
    }

    protected override void OnRemove() {
        ConfigManager.Instance.Clear();
    }
}
