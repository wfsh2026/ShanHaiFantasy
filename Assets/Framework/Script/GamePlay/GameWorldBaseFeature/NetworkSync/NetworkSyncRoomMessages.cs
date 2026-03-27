using System;

public static class NetworkSyncRoomMessageIds {
    public const int ROOM_CREATE_CMD = 11001;
    public const int ROOM_JOIN_CMD = 11002;
    public const int ROOM_LEAVE_CMD = 11003;
    public const int ROOM_START_CMD = 11004;
    public const int ROOM_STATE_RPC = 11005;
    public const int ROOM_DISBAND_RPC = 11006;
    public const int ROOM_START_RPC = 11007;
    public const int ROOM_OPERATION_FAILED_TARGET_RPC = 11008;
    public const int ROOM_LOCAL_PLAYER_ASSIGNED_TARGET_RPC = 11009;
}

[Serializable]
public enum NetworkSyncRoomParticipantType {
    Host = 0,
    Player = 1,
    AI = 2
}

[Serializable]
public sealed class NetworkSyncRoomSlotData {
    public int slotIndex;
    public string displayName;
    public string avatarId;
    public NetworkSyncRoomParticipantType participantType;
    public string playerId;
    public bool isHost;
}

[Serializable]
public sealed class NetworkSyncRoomCreateCmd : INetworkSyncCmd {
    public string inviteCode;
    public string displayName;
    public string avatarId;
}

[Serializable]
public sealed class NetworkSyncRoomJoinCmd : INetworkSyncCmd {
    public string inviteCode;
    public string displayName;
    public string avatarId;
}

[Serializable]
public sealed class NetworkSyncRoomLeaveCmd : INetworkSyncCmd {
}

[Serializable]
public sealed class NetworkSyncRoomStartCmd : INetworkSyncCmd {
}

[Serializable]
public sealed class NetworkSyncRoomStateRpc : INetworkSyncRpc {
    public string inviteCode;
    public bool hasStarted;
    public int playerCount;
    public int aiCount;
    public NetworkSyncRoomSlotData[] slots;
}

[Serializable]
public sealed class NetworkSyncRoomDisbandRpc : INetworkSyncRpc {
    public string messageText;
}

[Serializable]
public sealed class NetworkSyncRoomStartRpc : INetworkSyncRpc {
    public string inviteCode;
    public string sceneId;
}

[Serializable]
public sealed class NetworkSyncRoomOperationFailedTargetRpc : INetworkSyncTargetRpc {
    public string messageText;
}

[Serializable]
public sealed class NetworkSyncRoomLocalPlayerAssignedTargetRpc : INetworkSyncTargetRpc {
    public string playerId;
    public bool isHost;
}

public static class NetworkSyncRoomProtocols {
    public static readonly NetworkSyncMessageDescriptor RoomCreateCmd =
        NetworkSyncMessageDescriptor.Create<NetworkSyncRoomCreateCmd>(
            NetworkSyncRoomMessageIds.ROOM_CREATE_CMD,
            "Room",
            NetworkSyncMessageKind.Command,
            NetworkSyncProtocolType.Cmd,
            NetworkSyncDirection.ClientToServer,
            NetworkSyncDelivery.Reliable,
            NetworkSyncTarget.None,
            NetworkSyncAuthority.AnyClient);

    public static readonly NetworkSyncMessageDescriptor RoomJoinCmd =
        NetworkSyncMessageDescriptor.Create<NetworkSyncRoomJoinCmd>(
            NetworkSyncRoomMessageIds.ROOM_JOIN_CMD,
            "Room",
            NetworkSyncMessageKind.Command,
            NetworkSyncProtocolType.Cmd,
            NetworkSyncDirection.ClientToServer,
            NetworkSyncDelivery.Reliable,
            NetworkSyncTarget.None,
            NetworkSyncAuthority.AnyClient);

    public static readonly NetworkSyncMessageDescriptor RoomLeaveCmd =
        NetworkSyncMessageDescriptor.Create<NetworkSyncRoomLeaveCmd>(
            NetworkSyncRoomMessageIds.ROOM_LEAVE_CMD,
            "Room",
            NetworkSyncMessageKind.Command,
            NetworkSyncProtocolType.Cmd,
            NetworkSyncDirection.ClientToServer,
            NetworkSyncDelivery.Reliable,
            NetworkSyncTarget.None,
            NetworkSyncAuthority.AnyClient);

    public static readonly NetworkSyncMessageDescriptor RoomStartCmd =
        NetworkSyncMessageDescriptor.Create<NetworkSyncRoomStartCmd>(
            NetworkSyncRoomMessageIds.ROOM_START_CMD,
            "Room",
            NetworkSyncMessageKind.Command,
            NetworkSyncProtocolType.Cmd,
            NetworkSyncDirection.ClientToServer,
            NetworkSyncDelivery.Reliable,
            NetworkSyncTarget.None,
            NetworkSyncAuthority.HostOnly);

    public static readonly NetworkSyncMessageDescriptor RoomStateRpc =
        NetworkSyncMessageDescriptor.Create<NetworkSyncRoomStateRpc>(
            NetworkSyncRoomMessageIds.ROOM_STATE_RPC,
            "Room",
            NetworkSyncMessageKind.Event,
            NetworkSyncProtocolType.Rpc,
            NetworkSyncDirection.ServerToClient,
            NetworkSyncDelivery.Reliable,
            NetworkSyncTarget.Broadcast,
            NetworkSyncAuthority.ServerOnly);

    public static readonly NetworkSyncMessageDescriptor RoomDisbandRpc =
        NetworkSyncMessageDescriptor.Create<NetworkSyncRoomDisbandRpc>(
            NetworkSyncRoomMessageIds.ROOM_DISBAND_RPC,
            "Room",
            NetworkSyncMessageKind.Event,
            NetworkSyncProtocolType.Rpc,
            NetworkSyncDirection.ServerToClient,
            NetworkSyncDelivery.Reliable,
            NetworkSyncTarget.Broadcast,
            NetworkSyncAuthority.ServerOnly);

    public static readonly NetworkSyncMessageDescriptor RoomStartRpc =
        NetworkSyncMessageDescriptor.Create<NetworkSyncRoomStartRpc>(
            NetworkSyncRoomMessageIds.ROOM_START_RPC,
            "Room",
            NetworkSyncMessageKind.Event,
            NetworkSyncProtocolType.Rpc,
            NetworkSyncDirection.ServerToClient,
            NetworkSyncDelivery.Reliable,
            NetworkSyncTarget.Broadcast,
            NetworkSyncAuthority.ServerOnly);

    public static readonly NetworkSyncMessageDescriptor RoomOperationFailedTargetRpc =
        NetworkSyncMessageDescriptor.Create<NetworkSyncRoomOperationFailedTargetRpc>(
            NetworkSyncRoomMessageIds.ROOM_OPERATION_FAILED_TARGET_RPC,
            "Room",
            NetworkSyncMessageKind.Event,
            NetworkSyncProtocolType.TargetRpc,
            NetworkSyncDirection.ServerToClient,
            NetworkSyncDelivery.Reliable,
            NetworkSyncTarget.Owner,
            NetworkSyncAuthority.ServerOnly);

    public static readonly NetworkSyncMessageDescriptor RoomLocalPlayerAssignedTargetRpc =
        NetworkSyncMessageDescriptor.Create<NetworkSyncRoomLocalPlayerAssignedTargetRpc>(
            NetworkSyncRoomMessageIds.ROOM_LOCAL_PLAYER_ASSIGNED_TARGET_RPC,
            "Room",
            NetworkSyncMessageKind.Event,
            NetworkSyncProtocolType.TargetRpc,
            NetworkSyncDirection.ServerToClient,
            NetworkSyncDelivery.Reliable,
            NetworkSyncTarget.Owner,
            NetworkSyncAuthority.ServerOnly);
}
