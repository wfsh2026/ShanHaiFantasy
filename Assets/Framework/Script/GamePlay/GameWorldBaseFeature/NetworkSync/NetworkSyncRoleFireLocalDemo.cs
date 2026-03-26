using UnityEngine;

/// <summary>
/// NetworkSync 本地回环测试入口。
/// 使用本地 server/client 传输，验证 Cmd -> Server -> Rpc/Delta -> Client 的完整链路。
/// </summary>
public sealed class NetworkSyncRoleFireLocalDemo : MonoBehaviour {
    private const string DEMO_OBJECT_NAME = "NetworkSyncRoleFireLocalDemo";

    [SerializeField] private int worldId = 1;
    [SerializeField] private int localConnectionId = 1;
    [SerializeField] private int remoteConnectionId = 2;
    [SerializeField] private int localRoleEntityId = 1001;
    [SerializeField] private int remoteRoleEntityId = 1002;
    [SerializeField] private int initialAmmo = 6;

    private NetworkSyncLocalLoopbackHub hub;
    private NetworkSyncServer server;
    private NetworkSyncRoleFireServerModule serverModule;
    private NetworkSyncClient localClient;
    private NetworkSyncRoleFireClientModule localClientModule;
    private NetworkSyncClient remoteClient;
    private NetworkSyncRoleFireClientModule remoteClientModule;
    private int nextFireSequence;
    private bool isInitialized;
    private string lastDemoSummary = "Network demo not executed.";

    public static NetworkSyncRoleFireLocalDemo EnsureInstance() {
        NetworkSyncRoleFireLocalDemo instance = FindObjectOfType<NetworkSyncRoleFireLocalDemo>();
        if (instance != null) {
            return instance;
        }

        GameObject demoObject = new GameObject(DEMO_OBJECT_NAME);
        return demoObject.AddComponent<NetworkSyncRoleFireLocalDemo>();
    }

    public string LastDemoSummary {
        get {
            return lastDemoSummary;
        }
    }

    [ContextMenu("Setup Role Fire Demo")]
    public void SetupRoleFireDemo() {
        DisposeRuntime();

        hub = new NetworkSyncLocalLoopbackHub();

        NetworkSyncLocalLoopbackServerTransport serverTransport = hub.CreateServerTransport();
        server = new NetworkSyncServer(serverTransport);
        serverModule = new NetworkSyncRoleFireServerModule();
        server.RegisterModule(serverModule);

        NetworkSyncLocalLoopbackClientTransport localTransport = hub.CreateClientTransport(localConnectionId);
        localClient = new NetworkSyncClient(localTransport);
        localClientModule = new NetworkSyncRoleFireClientModule();
        localClient.RegisterModule(localClientModule);

        NetworkSyncLocalLoopbackClientTransport remoteTransport = hub.CreateClientTransport(remoteConnectionId);
        remoteClient = new NetworkSyncClient(remoteTransport);
        remoteClientModule = new NetworkSyncRoleFireClientModule();
        remoteClient.RegisterModule(remoteClientModule);
        remoteClientModule.RoleFireRpcReceived += OnRemoteRoleFireRpcReceived;

        NetworkSyncConnectionRef localConnection = hub.GetConnection(localConnectionId);
        NetworkSyncConnectionRef remoteConnection = hub.GetConnection(remoteConnectionId);
        serverModule.BindRole(localConnection, localRoleEntityId, "LocalHero", initialAmmo);
        serverModule.BindRole(remoteConnection, remoteRoleEntityId, "RemoteHero", initialAmmo);

        server.SendFullSnapshot(localConnection, worldId);
        server.SendFullSnapshot(remoteConnection, worldId);
        server.Replication.ClearDirty();

        nextFireSequence = 0;
        isInitialized = true;
        lastDemoSummary = "NetworkSync local demo setup completed.";
        Debug.Log(lastDemoSummary);
    }

    [ContextMenu("Run Role Fire Demo Once")]
    public string RunRoleFireDemoOnce() {
        EnsureInitialized();

        nextFireSequence++;
        Vector3 muzzleWorldPosition = transform.position + transform.forward;
        localClientModule.SendRoleFireCmd(
            localRoleEntityId,
            nextFireSequence,
            muzzleWorldPosition,
            transform.forward,
            worldId);

        server.FlushDelta(worldId);
        int localAmmo = ReadRoleAmmo(localClient.WorldState, localRoleEntityId);
        int remoteAmmo = ReadRoleAmmo(remoteClient.WorldState, localRoleEntityId);
        lastDemoSummary =
            "NetworkSync Demo OK | FireSequence: " + nextFireSequence +
            " | LocalAmmo: " + localAmmo +
            " | RemoteAmmo: " + remoteAmmo;
        Debug.Log(lastDemoSummary);
        return lastDemoSummary;
    }

    private void EnsureInitialized() {
        if (!isInitialized) {
            SetupRoleFireDemo();
        }
    }

    private void OnDestroy() {
        DisposeRuntime();
    }

    private void DisposeRuntime() {
        if (remoteClientModule != null) {
            remoteClientModule.RoleFireRpcReceived -= OnRemoteRoleFireRpcReceived;
        }

        if (localClient != null) {
            localClient.Dispose();
        }

        if (remoteClient != null) {
            remoteClient.Dispose();
        }

        if (server != null) {
            server.Dispose();
        }

        hub = null;
        server = null;
        serverModule = null;
        localClient = null;
        localClientModule = null;
        remoteClient = null;
        remoteClientModule = null;
        isInitialized = false;
    }

    private void OnRemoteRoleFireRpcReceived(NetworkSyncRoleFireRpc message) {
        Debug.Log(
            "Remote client received role fire rpc. RoleEntityId: " + message.roleEntityId +
            ", FireSequence: " + message.fireSequence +
            ", ShooterConnectionId: " + message.shooterConnectionId);
    }

    private static int ReadRoleAmmo(NetworkSyncClientWorldState worldState, int roleEntityId) {
        if (!TryGetIntField(worldState, roleEntityId, NetworkSyncRoleFireFieldIds.AMMO, out int ammo)) {
            return -1;
        }

        return ammo;
    }

    private static bool TryGetIntField(
        NetworkSyncClientWorldState worldState,
        int roleEntityId,
        string fieldId,
        out int value) {
        value = 0;
        if (worldState == null) {
            return false;
        }

        if (!worldState.TryGetEntity(roleEntityId, out NetworkSyncReplicatedEntityState entityState)) {
            return false;
        }

        for (int i = 0; i < entityState.fields.Count; i++) {
            NetworkSyncReplicatedFieldState fieldState = entityState.fields[i];
            if (fieldState.fieldId != fieldId) {
                continue;
            }

            NetworkSyncIntValue wrapper = JsonUtility.FromJson<NetworkSyncIntValue>(fieldState.payloadJson);
            if (wrapper == null) {
                return false;
            }

            value = wrapper.value;
            return true;
        }

        return false;
    }
}
