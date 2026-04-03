using System;
using System.Collections;
using Mirror;
using kcp2k;
using UnityEngine;

/// <summary>
/// A1 鎴块棿闃舵浣跨敤鐨?Mirror 杩愯鏃躲€?/// 璐熻矗鍚姩 Host / Client锛屽苟鎶婂簳灞備紶杈撴帴鍒扮幇鏈?NetworkSync Feature 涓娿€?/// </summary>
public sealed class NetworkSyncMirrorRoomRuntime : NetworkManager {
    private const string DEFAULT_CONNECT_ADDRESS = "127.0.0.1";
    private const float CONNECT_TIMEOUT_SECONDS = 5f;

    private static NetworkSyncMirrorRoomRuntime instance;

    private ClientNetworkFeatureManager clientFeatureManager;
    private ServerNetworkFeatureManager serverFeatureManager;
    private NetworkSyncMirrorClientTransport clientTransport;
    private NetworkSyncMirrorServerTransport serverTransport;
    private NetworkSyncRoomClientModule preservedRoomClientModule;
    private NetworkSyncRoomServerModule preservedRoomServerModule;
    private NetworkSyncRoomStateRpc preservedRoomState;
    private NetworkSyncRoomStartRpc preservedRoomStart;
    private int preservedRoomWorldId;
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

    public string CurrentInviteCode { get; private set; }

    public static bool HasLiveInstance {
        get {
            return instance != null;
        }
    }

    public static bool HasActiveServerRuntime {
        get {
            return instance != null && NetworkServer.active;
        }
    }

    public static bool HasActiveClientRuntime {
        get {
            return instance != null && NetworkClient.active;
        }
    }

    public static string GenerateInviteCode() {
        int value = UnityEngine.Random.Range(20000, 50000);
        return value.ToString();
    }

    public static void TryAttachWorld(GameWorld gameWorld) {
        if (instance == null || gameWorld == null) {
            return;
        }

        instance.AttachWorld(gameWorld);
    }

    public static bool TryGetPreservedRoomBootstrap(
        out NetworkSyncRoomStateRpc roomState,
        out NetworkSyncRoomStartRpc roomStart,
        out int worldId) {
        roomState = null;
        roomStart = null;
        worldId = 0;
        if (instance == null) {
            return false;
        }

        roomState = CloneRoomState(instance.preservedRoomState);
        roomStart = CloneRoomStart(instance.preservedRoomStart);
        worldId = instance.preservedRoomWorldId;
        return roomState != null || roomStart != null;
    }

    public void PreserveRoomModules(NetworkSyncRoomClientModule roomClientModule, NetworkSyncRoomServerModule roomServerModule) {
        preservedRoomClientModule = roomClientModule;
        preservedRoomServerModule = roomServerModule;
        CachePreservedRoomBootstrap(roomClientModule, roomServerModule);
    }

    public bool StartHostRuntime(GameWorld gameWorld, string inviteCode, Action onConnected, Action<string> onFailed) {
        string resolvedInviteCode = string.IsNullOrWhiteSpace(inviteCode) ? GenerateInviteCode() : inviteCode.Trim();
        ushort port;
        if (!TryParseInviteCode(resolvedInviteCode, out port)) {
            onFailed?.Invoke("杩炴帴澶辫触");
            return false;
        }

        if (!PrepareWorld(gameWorld, true, onConnected, onFailed)) {
            return false;
        }

        CurrentInviteCode = resolvedInviteCode;
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
            onFailed?.Invoke("杩炴帴澶辫触");
            return false;
        }

        if (!PrepareWorld(gameWorld, false, onConnected, onFailed)) {
            return false;
        }

        CurrentInviteCode = inviteCode.Trim();
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
        ClearPreservedModules();
        CurrentInviteCode = string.Empty;
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
        RegisterPreservedServerModules();
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
            RegisterPreservedClientModules();
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
            callback?.Invoke("杩炴帴澶辫触");
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
            onFailed?.Invoke("杩炴帴澶辫触");
            return false;
        }

        StopRuntime();
        AttachWorld(gameWorld);
        connectedCallback = onConnected;
        failedCallback = onFailed;

        if (clientFeatureManager == null) {
            onFailed?.Invoke("杩炴帴澶辫触");
            return false;
        }

        if (needServer && serverFeatureManager == null) {
            onFailed?.Invoke("杩炴帴澶辫触");
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
        callback?.Invoke("杩炴帴澶辫触");
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

    private void AttachWorld(GameWorld gameWorld) {
        if (gameWorld == null) {
            return;
        }

        clientFeatureManager = gameWorld.GetExtendFeature<ClientNetworkFeatureManager>();
        serverFeatureManager = gameWorld.GetExtendFeature<ServerNetworkFeatureManager>();

        if (clientFeatureManager != null) {
            RegisterPreservedClientModules();
            if (clientTransport != null && NetworkClient.active) {
                clientFeatureManager.BindRuntime(clientTransport);
            }
        }

        if (serverFeatureManager != null) {
            RegisterPreservedServerModules();
            if (serverTransport != null && NetworkServer.active) {
                serverFeatureManager.BindRuntime(serverTransport);
            }
        }
    }

    private void RegisterPreservedClientModules() {
        if (clientFeatureManager == null || preservedRoomClientModule == null) {
            return;
        }

        clientFeatureManager.RegisterModule(preservedRoomClientModule);
    }

    private void RegisterPreservedServerModules() {
        if (serverFeatureManager == null || preservedRoomServerModule == null) {
            return;
        }

        serverFeatureManager.RegisterModule(preservedRoomServerModule);
    }

    private void ClearPreservedModules() {
        preservedRoomClientModule = null;
        preservedRoomServerModule = null;
        preservedRoomState = null;
        preservedRoomStart = null;
        preservedRoomWorldId = 0;
    }

    private void CachePreservedRoomBootstrap(
        NetworkSyncRoomClientModule roomClientModule,
        NetworkSyncRoomServerModule roomServerModule) {
        NetworkSyncRoomStateRpc roomStateFromClient = roomClientModule == null ? null : roomClientModule.LastRoomState;
        NetworkSyncRoomStateRpc roomStateFromServer = roomServerModule == null ? null : roomServerModule.GetRoomStateSnapshot();
        preservedRoomState = CloneRoomState(roomStateFromServer ?? roomStateFromClient);

        NetworkSyncRoomStartRpc roomStart = roomClientModule == null ? null : roomClientModule.LastRoomStart;
        if (roomStart == null && roomServerModule != null && !string.IsNullOrWhiteSpace(roomServerModule.CurrentMatchId)) {
            roomStart = new NetworkSyncRoomStartRpc {
                matchId = roomServerModule.CurrentMatchId,
                inviteCode = roomServerModule.CurrentInviteCode,
                sceneId = "BattleTestScene"
            };
        }

        preservedRoomStart = CloneRoomStart(roomStart);
        preservedRoomWorldId = roomServerModule == null ? 0 : roomServerModule.CurrentWorldId;
    }

    private static NetworkSyncRoomStateRpc CloneRoomState(NetworkSyncRoomStateRpc source) {
        if (source == null) {
            return null;
        }

        NetworkSyncRoomStateRpc cloned = new NetworkSyncRoomStateRpc();
        cloned.inviteCode = source.inviteCode;
        cloned.hasStarted = source.hasStarted;
        cloned.playerCount = source.playerCount;
        cloned.aiCount = source.aiCount;
        if (source.slots == null || source.slots.Length == 0) {
            cloned.slots = new NetworkSyncRoomSlotData[0];
            return cloned;
        }

        cloned.slots = new NetworkSyncRoomSlotData[source.slots.Length];
        for (int i = 0; i < source.slots.Length; i++) {
            NetworkSyncRoomSlotData slot = source.slots[i];
            cloned.slots[i] = slot == null
                ? new NetworkSyncRoomSlotData()
                : new NetworkSyncRoomSlotData {
                    slotIndex = slot.slotIndex,
                    displayName = slot.displayName,
                    avatarId = slot.avatarId,
                    participantType = slot.participantType,
                    playerId = slot.playerId,
                    isHost = slot.isHost
                };
        }

        return cloned;
    }

    private static NetworkSyncRoomStartRpc CloneRoomStart(NetworkSyncRoomStartRpc source) {
        if (source == null) {
            return null;
        }

        return new NetworkSyncRoomStartRpc {
            matchId = source.matchId,
            inviteCode = source.inviteCode,
            sceneId = source.sceneId
        };
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
