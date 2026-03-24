using UnityEngine;

public sealed class NetworkSyncRoleFireLocalDemo : MonoBehaviour {
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

    [ContextMenu("Setup Role Fire Demo")]
    public void SetupRoleFireDemo() {
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
        Debug.Log("NetworkSyncRoleFireLocalDemo setup completed.");
    }

    [ContextMenu("Run Role Fire Demo Once")]
    public void RunRoleFireDemoOnce() {
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
        PrintRoleAmmo("LocalClient", localClient.WorldState, localRoleEntityId);
        PrintRoleAmmo("RemoteClient", remoteClient.WorldState, localRoleEntityId);
    }

    private void EnsureInitialized() {
        if (!isInitialized) {
            SetupRoleFireDemo();
        }
    }

    private void OnRemoteRoleFireRpcReceived(NetworkSyncRoleFireRpc message) {
        Debug.Log(
            "Remote client received role fire rpc. RoleEntityId: " + message.roleEntityId +
            ", FireSequence: " + message.fireSequence +
            ", ShooterConnectionId: " + message.shooterConnectionId);
    }

    private void PrintRoleAmmo(string clientName, NetworkSyncClientWorldState worldState, int roleEntityId) {
        if (!TryGetIntField(worldState, roleEntityId, NetworkSyncRoleFireFieldIds.AMMO, out int ammo)) {
            Debug.LogWarning(clientName + " ammo field not found. RoleEntityId: " + roleEntityId);
            return;
        }

        Debug.Log(clientName + " observes role ammo: " + ammo + ", roleEntityId: " + roleEntityId);
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
