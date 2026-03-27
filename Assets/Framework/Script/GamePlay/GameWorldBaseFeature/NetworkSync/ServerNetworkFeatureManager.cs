using System.Collections.Generic;

/// <summary>
/// NetworkSync 服务端挂载入口。
/// 只负责把 NetworkSyncServer 的生命周期挂到当前 GameWorld 上。
/// 业务层通过 RegisterModule 注册协议模块，不直接操作底层传输。
/// </summary>
public sealed class ServerNetworkFeatureManager : AbsExtendGameWorldFeature {
    private readonly List<INetworkSyncServerModule> pendingModules = new List<INetworkSyncServerModule>();
    private NetworkSyncServer networkServer;

    public NetworkSyncServer NetworkServer {
        get {
            return networkServer;
        }
    }

    public bool IsRuntimeReady {
        get {
            return networkServer != null;
        }
    }

    protected override void OnRemove() {
        UnbindRuntime();
        pendingModules.Clear();
    }

    /// <summary>
    /// 由具体网络运行时把服务端传输绑定进来。
    /// 这样正式主链和测试传输可以分层管理。
    /// </summary>
    public void BindRuntime(
        INetworkSyncServerTransport transport,
        INetworkSyncSerializer serializer = null,
        NetworkSyncRegistry registry = null) {
        if (transport == null) {
            return;
        }

        UnbindRuntime();
        networkServer = new NetworkSyncServer(transport, serializer, registry);
        RegisterPendingModules();
    }

    public void UnbindRuntime() {
        if (networkServer == null) {
            return;
        }

        networkServer.Dispose();
        networkServer = null;
    }

    public void RegisterModule(INetworkSyncServerModule module) {
        if (module == null) {
            return;
        }

        if (!pendingModules.Contains(module)) {
            pendingModules.Add(module);
        }

        if (networkServer != null) {
            networkServer.RegisterModule(module);
        }
    }

    private void RegisterPendingModules() {
        if (networkServer == null) {
            return;
        }

        for (int i = 0; i < pendingModules.Count; i++) {
            networkServer.RegisterModule(pendingModules[i]);
        }
    }
}
