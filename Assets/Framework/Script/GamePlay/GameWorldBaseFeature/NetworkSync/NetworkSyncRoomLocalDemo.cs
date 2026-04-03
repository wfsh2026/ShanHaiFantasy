using UnityEngine;

/// <summary>
/// 房间网络本地回环测试入口。
/// 使用服务端权威简化规则验证：创建房间、客户端加入、房主开始、开始后禁止加入、房主离开解散房间。
/// </summary>
public sealed class NetworkSyncRoomLocalDemo : MonoBehaviour {
    private const string DEMO_OBJECT_NAME = "NetworkSyncRoomLocalDemo";
    private const int WORLD_ID = 1;
    private const int HOST_CONNECTION_ID = 1;
    private const int GUEST_A_CONNECTION_ID = 2;
    private const int GUEST_B_CONNECTION_ID = 3;
    private const int LATE_GUEST_CONNECTION_ID = 4;

    private NetworkSyncLocalLoopbackHub hub;
    private NetworkSyncServer server;
    private NetworkSyncRoomServerModule serverModule;
    private ServerRoomNetHandler serverHandler;
    private NetworkSyncClient hostClient;
    private NetworkSyncRoomClientModule hostRoomModule;
    private ClientRoomNetProxy hostRoomProxy;
    private NetworkSyncClient guestAClient;
    private NetworkSyncRoomClientModule guestARoomModule;
    private ClientRoomNetProxy guestARoomProxy;
    private NetworkSyncClient guestBClient;
    private NetworkSyncRoomClientModule guestBRoomModule;
    private ClientRoomNetProxy guestBRoomProxy;
    private NetworkSyncClient lateGuestClient;
    private NetworkSyncRoomClientModule lateGuestRoomModule;
    private ClientRoomNetProxy lateGuestRoomProxy;
    private bool isInitialized;
    private string lastDemoSummary = "Room network demo not executed.";

    public static NetworkSyncRoomLocalDemo EnsureInstance() {
        NetworkSyncRoomLocalDemo instance = FindObjectOfType<NetworkSyncRoomLocalDemo>();
        if (instance != null) {
            return instance;
        }

        GameObject demoObject = new GameObject(DEMO_OBJECT_NAME);
        return demoObject.AddComponent<NetworkSyncRoomLocalDemo>();
    }

    public string LastDemoSummary {
        get {
            return lastDemoSummary;
        }
    }

    [ContextMenu("Setup Room Demo")]
    public void SetupRoomDemo() {
        DisposeRuntime();

        hub = new NetworkSyncLocalLoopbackHub();

        NetworkSyncLocalLoopbackServerTransport serverTransport = hub.CreateServerTransport();
        server = new NetworkSyncServer(serverTransport);
        serverModule = new NetworkSyncRoomServerModule("BattleTestScene", allowCreateInviteCodeFallback: true);
        server.RegisterModule(serverModule);
        serverHandler = new ServerRoomNetHandler(serverModule);

        hostClient = CreateClient(HOST_CONNECTION_ID, out hostRoomModule, out hostRoomProxy);
        guestAClient = CreateClient(GUEST_A_CONNECTION_ID, out guestARoomModule, out guestARoomProxy);
        guestBClient = CreateClient(GUEST_B_CONNECTION_ID, out guestBRoomModule, out guestBRoomProxy);
        lateGuestClient = CreateClient(LATE_GUEST_CONNECTION_ID, out lateGuestRoomModule, out lateGuestRoomProxy);

        isInitialized = true;
        lastDemoSummary = "Room network demo setup completed.";
        Debug.Log(lastDemoSummary);
    }

    [ContextMenu("Run Room Lifecycle Demo Once")]
    public string RunRoomLifecycleDemoOnce() {
        EnsureInitialized();
        ResetClientRuntimeStates();

        hostRoomProxy.CreateRoom("28888", "Host_Player", "avatar_player_host");
        string inviteCode = serverHandler.CurrentInviteCode;
        if (string.IsNullOrEmpty(inviteCode)) {
            lastDemoSummary = "Room demo failed: invite code is empty.";
            Debug.LogError(lastDemoSummary);
            return lastDemoSummary;
        }

        guestARoomProxy.JoinRoom(inviteCode, "Guest_A", "avatar_player_guest_a");
        guestBRoomProxy.JoinRoom(inviteCode, "Guest_B", "avatar_player_guest_b");

        NetworkSyncRoomStateRpc latestRoomState = hostRoomModule.LastRoomState;
        int joinedPlayerCount = latestRoomState == null ? 0 : latestRoomState.playerCount;
        int joinedAICount = latestRoomState == null ? 0 : latestRoomState.aiCount;

        hostRoomProxy.StartRoom();
        lateGuestRoomProxy.JoinRoom(inviteCode, "Late_Guest", "avatar_player_late");
        bool lateJoinFailed = !string.IsNullOrEmpty(lateGuestRoomModule.LastFailureMessage);
        bool startBroadcasted = hostRoomModule.LastRoomStart != null && guestARoomModule.LastRoomStart != null;
        bool matchIdReady = hostRoomModule.LastRoomStart != null &&
            !string.IsNullOrWhiteSpace(hostRoomModule.LastRoomStart.matchId);

        hostRoomProxy.LeaveRoom();
        bool disbandBroadcasted = hostRoomModule.LastRoomDisband != null &&
            guestARoomModule.LastRoomDisband != null &&
            guestBRoomModule.LastRoomDisband != null;

        lastDemoSummary =
            "Room Demo OK | InviteCode: " + inviteCode +
            " | Players: " + joinedPlayerCount +
            " | AI: " + joinedAICount +
            " | Started: " + startBroadcasted +
            " | MatchIdReady: " + matchIdReady +
            " | LateJoinFailed: " + lateJoinFailed +
            " | Disbanded: " + disbandBroadcasted;
        Debug.Log(lastDemoSummary);
        return lastDemoSummary;
    }

    private void EnsureInitialized() {
        if (!isInitialized) {
            SetupRoomDemo();
        }
    }

    private NetworkSyncClient CreateClient(
        int connectionId,
        out NetworkSyncRoomClientModule roomModule,
        out ClientRoomNetProxy roomProxy) {
        NetworkSyncLocalLoopbackClientTransport transport = hub.CreateClientTransport(connectionId);
        NetworkSyncClient client = new NetworkSyncClient(transport);
        roomModule = new NetworkSyncRoomClientModule();
        client.RegisterModule(roomModule);
        roomProxy = new ClientRoomNetProxy(roomModule, WORLD_ID);
        return client;
    }

    private void ResetClientRuntimeStates() {
        if (hostRoomModule != null) {
            hostRoomModule.ResetRuntimeState();
        }

        if (guestARoomModule != null) {
            guestARoomModule.ResetRuntimeState();
        }

        if (guestBRoomModule != null) {
            guestBRoomModule.ResetRuntimeState();
        }

        if (lateGuestRoomModule != null) {
            lateGuestRoomModule.ResetRuntimeState();
        }
    }

    private void OnDestroy() {
        DisposeRuntime();
    }

    private void DisposeRuntime() {
        DisposeClient(hostClient);
        DisposeClient(guestAClient);
        DisposeClient(guestBClient);
        DisposeClient(lateGuestClient);

        if (server != null) {
            server.Dispose();
        }

        hub = null;
        server = null;
        serverModule = null;
        serverHandler = null;
        hostClient = null;
        hostRoomModule = null;
        hostRoomProxy = null;
        guestAClient = null;
        guestARoomModule = null;
        guestARoomProxy = null;
        guestBClient = null;
        guestBRoomModule = null;
        guestBRoomProxy = null;
        lateGuestClient = null;
        lateGuestRoomModule = null;
        lateGuestRoomProxy = null;
        isInitialized = false;
    }

    private static void DisposeClient(NetworkSyncClient client) {
        if (client != null) {
            client.Dispose();
        }
    }
}
