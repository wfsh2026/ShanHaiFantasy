using System.Collections.Generic;

/// <summary>
/// NetworkSync 客户端挂载入口。
/// 只负责把 NetworkSyncClient 的生命周期挂到当前 GameWorld 上。
/// 具体运行时传输由外部显式绑定，避免把测试传输直接写进正式主链。
/// </summary>
public sealed class ClientNetworkFeatureManager : AbsExtendGameWorldFeature {
    private readonly List<INetworkSyncClientModule> pendingModules = new List<INetworkSyncClientModule>();
    private NetworkSyncClient networkClient;

    public NetworkSyncClient NetworkClient {
        get {
            return networkClient;
        }
    }

    public bool IsRuntimeReady {
        get {
            return networkClient != null;
        }
    }

    protected override void OnRemove() {
        UnbindRuntime();
        pendingModules.Clear();
    }

    /// <summary>
    /// 由具体网络运行时把客户端传输绑定进来。
    /// 这样可以保持 NetworkSync 核心结构不变，同时符合当前 GameWorld Feature 接入规范。
    /// </summary>
    public void BindRuntime(
        INetworkSyncClientTransport transport,
        INetworkSyncSerializer serializer = null,
        NetworkSyncRegistry registry = null) {
        if (transport == null) {
            return;
        }

        UnbindRuntime();
        networkClient = new NetworkSyncClient(transport, serializer, registry);
        RegisterPendingModules();
    }

    public void UnbindRuntime() {
        if (networkClient == null) {
            return;
        }

        networkClient.Dispose();
        networkClient = null;
    }

    public void RegisterModule(INetworkSyncClientModule module) {
        if (module == null) {
            return;
        }

        if (!pendingModules.Contains(module)) {
            pendingModules.Add(module);
        }

        if (networkClient != null) {
            networkClient.RegisterModule(module);
        }
    }

    private void RegisterPendingModules() {
        if (networkClient == null) {
            return;
        }

        for (int i = 0; i < pendingModules.Count; i++) {
            networkClient.RegisterModule(pendingModules[i]);
        }
    }
}
