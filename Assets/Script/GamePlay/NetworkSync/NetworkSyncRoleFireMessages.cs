using System;
using UnityEngine;

public static class NetworkSyncRoleFireMessageIds {
    public const int ROLE_FIRE_CMD = 10001;
    public const int ROLE_FIRE_RPC = 10002;
}

public static class NetworkSyncRoleFireFieldIds {
    public const string ROLE_NAME = "role_name";
    public const string AMMO = "ammo";
    public const string LAST_FIRE_SEQUENCE = "last_fire_sequence";
    public const string LAST_FIRE_POSITION = "last_fire_position";
    public const string LAST_FIRE_DIRECTION = "last_fire_direction";
}

[Serializable]
public sealed class NetworkSyncStringValue {
    public string value;
}

[Serializable]
public sealed class NetworkSyncIntValue {
    public int value;
}

[Serializable]
public sealed class NetworkSyncVector3Value {
    public Vector3 value;
}

[Serializable]
public sealed class NetworkSyncRoleFireCmd : INetworkSyncCmd {
    public int roleEntityId;
    public int fireSequence;
    public Vector3 muzzleWorldPosition;
    public Vector3 fireDirection;
}

[Serializable]
public sealed class NetworkSyncRoleFireRpc : INetworkSyncRpc {
    public int roleEntityId;
    public int fireSequence;
    public int shooterConnectionId;
    public Vector3 muzzleWorldPosition;
    public Vector3 fireDirection;
}

public static class NetworkSyncRoleFireProtocols {
    public static readonly NetworkSyncMessageDescriptor RoleFireCmd =
        NetworkSyncMessageDescriptor.Create<NetworkSyncRoleFireCmd>(
            NetworkSyncRoleFireMessageIds.ROLE_FIRE_CMD,
            "RoleFire",
            NetworkSyncMessageKind.Command,
            NetworkSyncProtocolType.Cmd,
            NetworkSyncDirection.ClientToServer,
            NetworkSyncDelivery.Reliable,
            NetworkSyncTarget.None,
            NetworkSyncAuthority.OwnerOnly,
            "RoleFire.ValidateCmd");

    public static readonly NetworkSyncMessageDescriptor RoleFireRpc =
        NetworkSyncMessageDescriptor.Create<NetworkSyncRoleFireRpc>(
            NetworkSyncRoleFireMessageIds.ROLE_FIRE_RPC,
            "RoleFire",
            NetworkSyncMessageKind.Event,
            NetworkSyncProtocolType.Rpc,
            NetworkSyncDirection.ServerToClient,
            NetworkSyncDelivery.Reliable,
            NetworkSyncTarget.Broadcast,
            NetworkSyncAuthority.ServerOnly);
}
