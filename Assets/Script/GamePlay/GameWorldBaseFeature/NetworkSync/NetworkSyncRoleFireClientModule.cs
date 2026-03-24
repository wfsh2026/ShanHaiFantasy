using System;
using UnityEngine;

public sealed class NetworkSyncRoleFireClientModule : INetworkSyncClientModule {
    private NetworkSyncClient client;

    public event Action<NetworkSyncRoleFireRpc> RoleFireRpcReceived;

    public void Register(NetworkSyncClient networkClient) {
        client = networkClient ?? throw new ArgumentNullException(nameof(networkClient));
        client.RegisterRpc<NetworkSyncRoleFireRpc>(
            NetworkSyncRoleFireProtocols.RoleFireRpc,
            OnRoleFireRpcReceived);
    }

    public void SendRoleFireCmd(
        int roleEntityId,
        int fireSequence,
        Vector3 muzzleWorldPosition,
        Vector3 fireDirection,
        int worldId = 0) {
        if (client == null) {
            throw new InvalidOperationException("NetworkSyncRoleFireClientModule is not registered.");
        }

        client.SendCmd(new NetworkSyncRoleFireCmd {
            roleEntityId = roleEntityId,
            fireSequence = fireSequence,
            muzzleWorldPosition = muzzleWorldPosition,
            fireDirection = fireDirection
        }, worldId);
    }

    private void OnRoleFireRpcReceived(NetworkSyncClientContext context, NetworkSyncRoleFireRpc message) {
        RoleFireRpcReceived?.Invoke(message);
    }
}
