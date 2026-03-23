using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class NetworkSyncRoleFireServerModule : INetworkSyncServerModule {
    private sealed class RoleRuntimeState {
        public int ConnectionId;
        public int RoleEntityId;
        public string RoleName;
        public int Ammo;
        public int LastFireSequence;
        public Vector3 LastFirePosition;
        public Vector3 LastFireDirection = Vector3.forward;
    }

    private readonly Dictionary<int, RoleRuntimeState> rolesByConnection =
        new Dictionary<int, RoleRuntimeState>();

    private NetworkSyncServer server;

    public void Register(NetworkSyncServer networkServer) {
        server = networkServer ?? throw new ArgumentNullException(nameof(networkServer));
        server.RegisterCmd<NetworkSyncRoleFireCmd>(
            NetworkSyncRoleFireProtocols.RoleFireCmd,
            HandleRoleFireCmd,
            ValidateRoleFireCmd);
    }

    public void BindRole(NetworkSyncConnectionRef connection, int roleEntityId, string roleName, int ammo) {
        if (connection == null) {
            throw new ArgumentNullException(nameof(connection));
        }

        RoleRuntimeState roleState = new RoleRuntimeState {
            ConnectionId = connection.ConnectionId,
            RoleEntityId = roleEntityId,
            RoleName = string.IsNullOrWhiteSpace(roleName) ? "Role_" + roleEntityId : roleName,
            Ammo = Mathf.Max(0, ammo),
            LastFireSequence = 0,
            LastFirePosition = Vector3.zero,
            LastFireDirection = Vector3.forward
        };

        rolesByConnection[connection.ConnectionId] = roleState;
        connection.Metadata.Set("role_entity_id", roleEntityId.ToString());
        ApplyRoleStateToReplication(roleState, true);
    }

    private NetworkSyncValidationResult ValidateRoleFireCmd(
        NetworkSyncServerContext context,
        NetworkSyncRoleFireCmd message) {
        if (message == null) {
            return NetworkSyncValidationResult.Fail("role_fire_cmd_null", "Role fire cmd is null.");
        }

        if (!rolesByConnection.TryGetValue(context.Connection.ConnectionId, out RoleRuntimeState roleState)) {
            return NetworkSyncValidationResult.Fail("role_not_bound", "Connection has no bound role.");
        }

        if (message.roleEntityId != roleState.RoleEntityId) {
            return NetworkSyncValidationResult.Fail("role_mismatch", "Cmd role entity does not match bound role.");
        }

        if (message.fireSequence <= roleState.LastFireSequence) {
            return NetworkSyncValidationResult.Fail("fire_sequence_invalid", "Cmd fire sequence must increase.");
        }

        if (roleState.Ammo <= 0) {
            return NetworkSyncValidationResult.Fail("ammo_not_enough", "Role ammo is not enough.");
        }

        if (message.fireDirection.sqrMagnitude <= 0.0001f) {
            return NetworkSyncValidationResult.Fail("direction_invalid", "Fire direction is invalid.");
        }

        return NetworkSyncValidationResult.Ok();
    }

    private void HandleRoleFireCmd(NetworkSyncServerContext context, NetworkSyncRoleFireCmd message) {
        RoleRuntimeState roleState = rolesByConnection[context.Connection.ConnectionId];
        roleState.Ammo = Mathf.Max(0, roleState.Ammo - 1);
        roleState.LastFireSequence = message.fireSequence;
        roleState.LastFirePosition = message.muzzleWorldPosition;
        roleState.LastFireDirection = message.fireDirection.normalized;

        ApplyRoleStateToReplication(roleState, false);

        context.Rpc(new NetworkSyncRoleFireRpc {
            roleEntityId = roleState.RoleEntityId,
            fireSequence = roleState.LastFireSequence,
            shooterConnectionId = context.Connection.ConnectionId,
            muzzleWorldPosition = roleState.LastFirePosition,
            fireDirection = roleState.LastFireDirection
        }, connection => connection.ConnectionId != context.Connection.ConnectionId);
    }

    private void ApplyRoleStateToReplication(RoleRuntimeState roleState, bool includeStaticFields) {
        if (server == null || roleState == null) {
            return;
        }

        server.Replication.UpsertEntity(roleState.RoleEntityId, "Role", roleState.ConnectionId);
        if (includeStaticFields) {
            server.Replication.SetFieldJson(
                roleState.RoleEntityId,
                "Role",
                NetworkSyncRoleFireFieldIds.ROLE_NAME,
                JsonUtility.ToJson(new NetworkSyncStringValue { value = roleState.RoleName }),
                NetworkSyncDelivery.Reliable,
                roleState.ConnectionId,
                0);
        }

        server.Replication.SetFieldJson(
            roleState.RoleEntityId,
            "Role",
            NetworkSyncRoleFireFieldIds.AMMO,
            JsonUtility.ToJson(new NetworkSyncIntValue { value = roleState.Ammo }),
            NetworkSyncDelivery.Reliable,
            roleState.ConnectionId,
            10);

        server.Replication.SetFieldJson(
            roleState.RoleEntityId,
            "Role",
            NetworkSyncRoleFireFieldIds.LAST_FIRE_SEQUENCE,
            JsonUtility.ToJson(new NetworkSyncIntValue { value = roleState.LastFireSequence }),
            NetworkSyncDelivery.Reliable,
            roleState.ConnectionId,
            20);

        server.Replication.SetFieldJson(
            roleState.RoleEntityId,
            "Role",
            NetworkSyncRoleFireFieldIds.LAST_FIRE_POSITION,
            JsonUtility.ToJson(new NetworkSyncVector3Value { value = roleState.LastFirePosition }),
            NetworkSyncDelivery.Unreliable,
            roleState.ConnectionId,
            30);

        server.Replication.SetFieldJson(
            roleState.RoleEntityId,
            "Role",
            NetworkSyncRoleFireFieldIds.LAST_FIRE_DIRECTION,
            JsonUtility.ToJson(new NetworkSyncVector3Value { value = roleState.LastFireDirection }),
            NetworkSyncDelivery.Unreliable,
            roleState.ConnectionId,
            40);
    }
}
