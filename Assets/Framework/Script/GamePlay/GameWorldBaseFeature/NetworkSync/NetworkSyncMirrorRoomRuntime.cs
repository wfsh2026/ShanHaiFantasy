using System;
using System.Collections;
using Mirror;
using kcp2k;
using UnityEngine;

/// <summary>
/// A1 房间阶段使用的 Mirror 运行时。
/// 负责启动 Host / Client，并把底层传输接到现有 NetworkSync Feature 上。
/// </summary>
public sealed class NetworkSyncMirrorRoomRuntime : NetworkManager {
    private const string DEFAULT_CONNECT_ADDRESS = "127.0.0.1";
    private const float CONNECT_TIMEOUT_SECONDS = 5f;

    private static NetworkSyncMirrorRoomRuntime instance;

    private ClientNetworkFeatureManager clientFeatureManager;
    private ServerNetworkFeatureManager serverFeatureManager;
    private NetworkSyncMirrorClientTransport clientTransport;
    private NetworkSyncMirrorServerTransport serverTransport;
    private Action connectedCallback;
    private Action<string> failedCallback;
    private Coroutine connectTimeoutCoroutine;
    private bool hasClientConnected;
    private bool isStoppingRuntime;

    public static NetworkSyncMirrorRoomRuntime Instance {
        get {
            if (instance == null) {
                CreateRuntimeObject();
            }

            return instance;
        }
    }

    public static string GenerateInviteCode() {
        int value = UnityEngine.Random.Range(20000, 50000);
        return value.ToString();
    }

    public bool StartHostRuntime(GameWorld gameWorld, string inviteCode, Action onConnected, Action<string> onFailed) {
        ushort port;
        if (!TryParseInviteCode(inviteCode, out port)) {
            onFailed?.Invoke("连接失败");
            return false;
        }

        if (!PrepareWorld(gameWorld, true, onConnected, onFailed)) {
            return false;
        }

        ConfigureTransport(port);
        networkAddress = DEFAULT_CONNECT_ADDRESS;
        hasClientConnected = false;
        isStoppingRuntime = false;
        StartHost();
        StartConnectTimeout();
        return true;
    }

    public bool StartClientRuntime(GameWorld gameWorld, string inviteCode, Action onConnected, Action<string> onFailed) {
        ushort port;
        if (!TryParseInviteCode(inviteCode, out port)) {
            onFailed?.Invoke("连接失败");
            return false;
        }

        if (!PrepareWorld(gameWorld, false, onConnected, onFailed)) {
            return false;
        }

        ConfigureTransport(port);
        networkAddress = DEFAULT_CONNECT_ADDRESS;
        hasClientConnected = false;
        isStoppingRuntime = false;
        StartClient();
        StartConnectTimeout();
        return true;
    }

    public void StopRuntime() {
        StopConnectTimeout();
        connectedCallback = null;
        failedCallback = null;
        isStoppingRuntime = true;

        if (NetworkServer.active && NetworkClient.active) {
            StopHost();
        } else if (NetworkClient.active) {
            StopClient();
        } else if (NetworkServer.active) {
            StopServer();
        }

        UnbindRuntime();
        hasClientConnected = false;
        isStoppingRuntime = false;
    }

    public override void Awake() {
        if (instance != null && instance != this) {
            Destroy(gameObject);
            return;
        }

        instance = this;
        if (GetComponent<KcpTransport>() == null) {
            gameObject.AddComponent<KcpTransport>();
        }

        transport = GetComponent<Transport>();
        autoCreatePlayer = false;
        dontDestroyOnLoad = true;
        runInBackground = true;
        offlineScene = string.Empty;
        onlineScene = string.Empty;
        base.Awake();
    }

    public override void OnStartServer() {
        base.OnStartServer();
        if (serverFeatureManager == null) {
            return;
        }

        serverTransport = new NetworkSyncMirrorServerTransport();
        serverFeatureManager.BindRuntime(serverTransport);
    }

    public override void OnStopServer() {
        if (serverFeatureManager != null) {
            serverFeatureManager.UnbindRuntime();
        }

        serverTransport = null;
        base.OnStopServer();
    }

    public override void OnClientConnect() {
        base.OnClientConnect();
        StopConnectTimeout();
        hasClientConnected = true;

        if (clientFeatureManager != null) {
            clientTransport = new NetworkSyncMirrorClientTransport();
            clientFeatureManager.BindRuntime(clientTransport);
        }

        Action callback = connectedCallback;
        connectedCallback = null;
        callback?.Invoke();
    }

    public override void OnClientDisconnect() {
        base.OnClientDisconnect();
        if (clientFeatureManager != null) {
            clientFeatureManager.UnbindRuntime();
        }

        clientTransport = null;
        StopConnectTimeout();

        if (!isStoppingRuntime && !hasClientConnected) {
            Action<string> callback = failedCallback;
            failedCallback = null;
            callback?.Invoke("连接失败");
        }
    }

    private static void CreateRuntimeObject() {
        GameObject runtimeObject = new GameObject("NetworkSyncMirrorRoomRuntime");
        DontDestroyOnLoad(runtimeObject);
        instance = runtimeObject.AddComponent<NetworkSyncMirrorRoomRuntime>();
    }

    private bool PrepareWorld(
        GameWorld gameWorld,
        bool needServer,
        Action onConnected,
        Action<string> onFailed) {
        if (gameWorld == null) {
            onFailed?.Invoke("连接失败");
            return false;
        }

        StopRuntime();
        clientFeatureManager = gameWorld.GetExtendFeature<ClientNetworkFeatureManager>();
        serverFeatureManager = gameWorld.GetExtendFeature<ServerNetworkFeatureManager>();
        connectedCallback = onConnected;
        failedCallback = onFailed;

        if (clientFeatureManager == null) {
            onFailed?.Invoke("连接失败");
            return false;
        }

        if (needServer && serverFeatureManager == null) {
            onFailed?.Invoke("连接失败");
            return false;
        }

        return true;
    }

    private void ConfigureTransport(ushort port) {
        KcpTransport kcpTransport = transport as KcpTransport;
        if (kcpTransport != null) {
            kcpTransport.Port = port;
        }
    }

    private void StartConnectTimeout() {
        StopConnectTimeout();
        connectTimeoutCoroutine = StartCoroutine(CoConnectTimeout());
    }

    private void StopConnectTimeout() {
        if (connectTimeoutCoroutine != null) {
            StopCoroutine(connectTimeoutCoroutine);
            connectTimeoutCoroutine = null;
        }
    }

    private IEnumerator CoConnectTimeout() {
        yield return new WaitForSeconds(CONNECT_TIMEOUT_SECONDS);
        if (hasClientConnected) {
            connectTimeoutCoroutine = null;
            yield break;
        }

        Action<string> callback = failedCallback;
        failedCallback = null;
        StopRuntime();
        callback?.Invoke("连接失败");
        connectTimeoutCoroutine = null;
    }

    private void UnbindRuntime() {
        if (clientFeatureManager != null) {
            clientFeatureManager.UnbindRuntime();
        }

        if (serverFeatureManager != null) {
            serverFeatureManager.UnbindRuntime();
        }

        clientTransport = null;
        serverTransport = null;
    }

    private static bool TryParseInviteCode(string inviteCode, out ushort port) {
        port = 0;
        if (string.IsNullOrWhiteSpace(inviteCode)) {
            return false;
        }

        int parsedPort;
        if (!int.TryParse(inviteCode.Trim(), out parsedPort)) {
            return false;
        }

        if (parsedPort <= 0 || parsedPort > ushort.MaxValue) {
            return false;
        }

        port = (ushort) parsedPort;
        return true;
    }
}
