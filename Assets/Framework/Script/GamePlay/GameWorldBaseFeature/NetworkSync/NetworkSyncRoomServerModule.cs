using System;
using System.Collections.Generic;

/// <summary>
/// 房间服务端网络模块。
/// 保持服务端权威规则：一个服务端实例只维护一个房间，房主离开即解散。
/// </summary>
public sealed class NetworkSyncRoomServerModule : INetworkSyncServerModule {
    private sealed class RoomSlotRuntime {
        public int SlotIndex;
        public string DisplayName;
        public string AvatarId;
        public string PlayerId;
        public int ConnectionId;
        public bool IsHost;
        public NetworkSyncRoomParticipantType ParticipantType;
    }

    private sealed class RoomRuntimeState {
        public string InviteCode;
        public bool HasStarted;
        public List<RoomSlotRuntime> Slots = new List<RoomSlotRuntime>(ROOM_SLOT_COUNT);
    }

    private const int ROOM_SLOT_COUNT = 8;
    private const string DEFAULT_FAIL_MESSAGE = "连接失败";
    private static readonly string[] AI_NAME_POOL = {
        "青狼[AI]",
        "墨羽[AI]",
        "赤砂[AI]",
        "霜牙[AI]",
        "暮林[AI]",
        "玄石[AI]",
        "惊岚[AI]",
        "逐风[AI]"
    };
    private static readonly string[] AI_AVATAR_POOL = {
        "avatar_ai_01",
        "avatar_ai_02",
        "avatar_ai_03",
        "avatar_ai_04"
    };
    private static readonly string[] PLAYER_NAME_POOL = {
        "青岳",
        "流云",
        "惊鸿",
        "清越",
        "长风",
        "司南",
        "照影",
        "无咎"
    };
    private static readonly string[] PLAYER_AVATAR_POOL = {
        "avatar_player_01",
        "avatar_player_02",
        "avatar_player_03",
        "avatar_player_04"
    };

    private readonly Dictionary<int, int> slotIndexByConnectionId = new Dictionary<int, int>();
    private readonly string battleSceneId;
    private NetworkSyncServer server;
    private RoomRuntimeState roomState;
    private int currentWorldId;

    public NetworkSyncRoomServerModule(string battleSceneId = "BattleTest") {
        this.battleSceneId = string.IsNullOrWhiteSpace(battleSceneId) ? "BattleTest" : battleSceneId;
    }

    public string CurrentInviteCode {
        get {
            if (roomState == null) {
                return string.Empty;
            }

            return roomState.InviteCode;
        }
    }

    public bool HasRoom {
        get {
            return roomState != null;
        }
    }

    public bool HasStarted {
        get {
            return roomState != null && roomState.HasStarted;
        }
    }

    public int PlayerCount {
        get {
            if (roomState == null) {
                return 0;
            }

            int count = 0;
            for (int i = 0; i < roomState.Slots.Count; i++) {
                if (roomState.Slots[i].ParticipantType != NetworkSyncRoomParticipantType.AI) {
                    count++;
                }
            }

            return count;
        }
    }

    public int AICount {
        get {
            if (roomState == null) {
                return 0;
            }

            int count = 0;
            for (int i = 0; i < roomState.Slots.Count; i++) {
                if (roomState.Slots[i].ParticipantType == NetworkSyncRoomParticipantType.AI) {
                    count++;
                }
            }

            return count;
        }
    }

    public void Register(NetworkSyncServer networkServer) {
        server = networkServer ?? throw new ArgumentNullException(nameof(networkServer));
        RegisterProtocolDescriptors();
        server.RegisterCmd<NetworkSyncRoomCreateCmd>(NetworkSyncRoomProtocols.RoomCreateCmd, HandleCreateRoomCmd, ValidateCreateRoomCmd);
        server.RegisterCmd<NetworkSyncRoomJoinCmd>(NetworkSyncRoomProtocols.RoomJoinCmd, HandleJoinRoomCmd, ValidateJoinRoomCmd);
        server.RegisterCmd<NetworkSyncRoomLeaveCmd>(NetworkSyncRoomProtocols.RoomLeaveCmd, HandleLeaveRoomCmd, ValidateLeaveRoomCmd);
        server.RegisterCmd<NetworkSyncRoomStartCmd>(NetworkSyncRoomProtocols.RoomStartCmd, HandleStartRoomCmd, ValidateStartRoomCmd);
    }

    /// <summary>
    /// 服务端除了接收 Cmd，还要发送房间状态、解散通知和目标 Rpc。
    /// 这里把房间协议的全部描述符注册进服务端 registry，保证发送链按消息类型查 descriptor 时能命中。
    /// </summary>
    private void RegisterProtocolDescriptors() {
        if (server == null || server.Registry == null) {
            return;
        }

        server.Registry.Register(NetworkSyncRoomProtocols.RoomCreateCmd);
        server.Registry.Register(NetworkSyncRoomProtocols.RoomJoinCmd);
        server.Registry.Register(NetworkSyncRoomProtocols.RoomLeaveCmd);
        server.Registry.Register(NetworkSyncRoomProtocols.RoomStartCmd);
        server.Registry.Register(NetworkSyncRoomProtocols.RoomStateRpc);
        server.Registry.Register(NetworkSyncRoomProtocols.RoomDisbandRpc);
        server.Registry.Register(NetworkSyncRoomProtocols.RoomStartRpc);
        server.Registry.Register(NetworkSyncRoomProtocols.RoomOperationFailedTargetRpc);
        server.Registry.Register(NetworkSyncRoomProtocols.RoomLocalPlayerAssignedTargetRpc);
    }

    private NetworkSyncValidationResult ValidateCreateRoomCmd(
        NetworkSyncServerContext context,
        NetworkSyncRoomCreateCmd message) {
        if (message == null) {
            return NetworkSyncValidationResult.Fail("room_create_null", "Room create cmd is null.");
        }

        return NetworkSyncValidationResult.Ok();
    }

    private NetworkSyncValidationResult ValidateJoinRoomCmd(
        NetworkSyncServerContext context,
        NetworkSyncRoomJoinCmd message) {
        if (message == null) {
            return NetworkSyncValidationResult.Fail("room_join_null", "Room join cmd is null.");
        }

        return NetworkSyncValidationResult.Ok();
    }

    private NetworkSyncValidationResult ValidateLeaveRoomCmd(
        NetworkSyncServerContext context,
        NetworkSyncRoomLeaveCmd message) {
        if (message == null) {
            return NetworkSyncValidationResult.Fail("room_leave_null", "Room leave cmd is null.");
        }

        return NetworkSyncValidationResult.Ok();
    }

    private NetworkSyncValidationResult ValidateStartRoomCmd(
        NetworkSyncServerContext context,
        NetworkSyncRoomStartCmd message) {
        if (message == null) {
            return NetworkSyncValidationResult.Fail("room_start_null", "Room start cmd is null.");
        }

        return NetworkSyncValidationResult.Ok();
    }

    private void HandleCreateRoomCmd(NetworkSyncServerContext context, NetworkSyncRoomCreateCmd message) {
        currentWorldId = context.CurrentWorldId;
        if (roomState != null || string.IsNullOrWhiteSpace(message.inviteCode)) {
            SendOperationFailed(context, DEFAULT_FAIL_MESSAGE);
            return;
        }

        roomState = new RoomRuntimeState();
        roomState.InviteCode = message.inviteCode.Trim();
        roomState.HasStarted = false;
        slotIndexByConnectionId.Clear();

        RoomSlotRuntime hostSlot = BuildRealPlayerSlot(
            0,
            context.Connection.ConnectionId,
            true,
            message.displayName,
            message.avatarId);
        roomState.Slots.Add(hostSlot);
        slotIndexByConnectionId[context.Connection.ConnectionId] = hostSlot.SlotIndex;
        SendLocalPlayerAssigned(context, hostSlot);

        for (int i = 1; i < ROOM_SLOT_COUNT; i++) {
            roomState.Slots.Add(BuildAISlot(i));
        }

        context.Connection.Metadata.Set("room_invite_code", roomState.InviteCode);
        context.Connection.Metadata.Set("player_id", hostSlot.PlayerId);
        BroadcastRoomState();
    }

    private void HandleJoinRoomCmd(NetworkSyncServerContext context, NetworkSyncRoomJoinCmd message) {
        currentWorldId = context.CurrentWorldId;
        if (roomState == null || roomState.HasStarted) {
            SendOperationFailed(context, DEFAULT_FAIL_MESSAGE);
            return;
        }

        if (!string.Equals(roomState.InviteCode, message.inviteCode, StringComparison.Ordinal)) {
            SendOperationFailed(context, DEFAULT_FAIL_MESSAGE);
            return;
        }

        if (slotIndexByConnectionId.ContainsKey(context.Connection.ConnectionId)) {
            SendOperationFailed(context, DEFAULT_FAIL_MESSAGE);
            return;
        }

        int targetSlotIndex = FindFirstAISlotIndex();
        if (targetSlotIndex < 0) {
            SendOperationFailed(context, DEFAULT_FAIL_MESSAGE);
            return;
        }

        RoomSlotRuntime playerSlot = BuildRealPlayerSlot(
            targetSlotIndex,
            context.Connection.ConnectionId,
            false,
            message.displayName,
            message.avatarId);
        roomState.Slots[targetSlotIndex] = playerSlot;
        slotIndexByConnectionId[context.Connection.ConnectionId] = targetSlotIndex;
        SendLocalPlayerAssigned(context, playerSlot);

        context.Connection.Metadata.Set("room_invite_code", roomState.InviteCode);
        context.Connection.Metadata.Set("player_id", playerSlot.PlayerId);
        BroadcastRoomState();
    }

    private void HandleLeaveRoomCmd(NetworkSyncServerContext context, NetworkSyncRoomLeaveCmd message) {
        currentWorldId = context.CurrentWorldId;
        if (roomState == null) {
            SendOperationFailed(context, DEFAULT_FAIL_MESSAGE);
            return;
        }

        if (!slotIndexByConnectionId.TryGetValue(context.Connection.ConnectionId, out int slotIndex)) {
            SendOperationFailed(context, DEFAULT_FAIL_MESSAGE);
            return;
        }

        RoomSlotRuntime leavingSlot = roomState.Slots[slotIndex];
        if (leavingSlot.IsHost) {
            BroadcastDisband("房主已离开，房间已解散");
            ClearRoom();
            return;
        }

        roomState.Slots[slotIndex] = BuildAISlot(slotIndex);
        slotIndexByConnectionId.Remove(context.Connection.ConnectionId);
        server.RemoveSession(context.Connection);
        BroadcastRoomState();
    }

    private void HandleStartRoomCmd(NetworkSyncServerContext context, NetworkSyncRoomStartCmd message) {
        currentWorldId = context.CurrentWorldId;
        if (roomState == null || roomState.HasStarted) {
            SendOperationFailed(context, DEFAULT_FAIL_MESSAGE);
            return;
        }

        if (!slotIndexByConnectionId.TryGetValue(context.Connection.ConnectionId, out int slotIndex)) {
            SendOperationFailed(context, DEFAULT_FAIL_MESSAGE);
            return;
        }

        RoomSlotRuntime hostSlot = roomState.Slots[slotIndex];
        if (!hostSlot.IsHost) {
            SendOperationFailed(context, DEFAULT_FAIL_MESSAGE);
            return;
        }

        roomState.HasStarted = true;
        BroadcastRoomState();
        server.Rpc(new NetworkSyncRoomStartRpc {
            inviteCode = roomState.InviteCode,
            sceneId = battleSceneId
        }, currentWorldId, IsRoomConnection);
    }

    private void BroadcastRoomState() {
        if (server == null || roomState == null) {
            return;
        }

        server.Rpc(BuildRoomStateRpc(), currentWorldId, IsRoomConnection);
    }

    private void BroadcastDisband(string messageText) {
        if (server == null || roomState == null) {
            return;
        }

        server.Rpc(new NetworkSyncRoomDisbandRpc {
            messageText = string.IsNullOrWhiteSpace(messageText) ? "房间已解散" : messageText
        }, currentWorldId, IsRoomConnection);
    }

    private void SendOperationFailed(NetworkSyncServerContext context, string messageText) {
        if (context == null) {
            return;
        }

        context.TargetRpc(new NetworkSyncRoomOperationFailedTargetRpc {
            messageText = string.IsNullOrWhiteSpace(messageText) ? DEFAULT_FAIL_MESSAGE : messageText
        });
    }

    private void SendLocalPlayerAssigned(NetworkSyncServerContext context, RoomSlotRuntime slot) {
        if (context == null || slot == null) {
            return;
        }

        context.TargetRpc(new NetworkSyncRoomLocalPlayerAssignedTargetRpc {
            playerId = slot.PlayerId,
            isHost = slot.IsHost
        });
    }

    private NetworkSyncRoomStateRpc BuildRoomStateRpc() {
        NetworkSyncRoomStateRpc message = new NetworkSyncRoomStateRpc();
        message.inviteCode = roomState.InviteCode;
        message.hasStarted = roomState.HasStarted;
        message.playerCount = PlayerCount;
        message.aiCount = AICount;
        message.slots = new NetworkSyncRoomSlotData[roomState.Slots.Count];

        for (int i = 0; i < roomState.Slots.Count; i++) {
            RoomSlotRuntime slot = roomState.Slots[i];
            message.slots[i] = new NetworkSyncRoomSlotData {
                slotIndex = slot.SlotIndex,
                displayName = slot.DisplayName,
                avatarId = slot.AvatarId,
                participantType = slot.ParticipantType,
                playerId = slot.PlayerId,
                isHost = slot.IsHost
            };
        }

        return message;
    }

    private int FindFirstAISlotIndex() {
        if (roomState == null) {
            return -1;
        }

        for (int i = 0; i < roomState.Slots.Count; i++) {
            if (roomState.Slots[i].ParticipantType == NetworkSyncRoomParticipantType.AI) {
                return i;
            }
        }

        return -1;
    }

    private RoomSlotRuntime BuildRealPlayerSlot(
        int slotIndex,
        int connectionId,
        bool isHost,
        string displayName,
        string avatarId) {
        RoomSlotRuntime slot = new RoomSlotRuntime();
        slot.SlotIndex = slotIndex;
        slot.ConnectionId = connectionId;
        slot.IsHost = isHost;
        slot.ParticipantType = isHost ? NetworkSyncRoomParticipantType.Host : NetworkSyncRoomParticipantType.Player;
        slot.PlayerId = (isHost ? "host_" : "player_") + connectionId;
        slot.DisplayName = BuildPlayerDisplayName(connectionId, displayName, isHost);
        slot.AvatarId = BuildPlayerAvatarId(connectionId, avatarId);
        return slot;
    }

    private RoomSlotRuntime BuildAISlot(int slotIndex) {
        RoomSlotRuntime slot = new RoomSlotRuntime();
        slot.SlotIndex = slotIndex;
        slot.ConnectionId = 0;
        slot.IsHost = false;
        slot.ParticipantType = NetworkSyncRoomParticipantType.AI;
        slot.PlayerId = "ai_" + slotIndex;
        slot.DisplayName = AI_NAME_POOL[slotIndex % AI_NAME_POOL.Length];
        slot.AvatarId = AI_AVATAR_POOL[slotIndex % AI_AVATAR_POOL.Length];
        return slot;
    }

    private string BuildPlayerDisplayName(int connectionId, string displayName, bool isHost) {
        if (!string.IsNullOrWhiteSpace(displayName)) {
            return displayName.Trim();
        }

        string baseName = PLAYER_NAME_POOL[Math.Abs(connectionId) % PLAYER_NAME_POOL.Length];
        if (isHost) {
            return baseName + "(Host)";
        }

        return baseName;
    }

    private string BuildPlayerAvatarId(int connectionId, string avatarId) {
        if (!string.IsNullOrWhiteSpace(avatarId)) {
            return avatarId.Trim();
        }

        return PLAYER_AVATAR_POOL[Math.Abs(connectionId) % PLAYER_AVATAR_POOL.Length];
    }

    private bool IsRoomConnection(NetworkSyncConnectionRef connection) {
        if (connection == null) {
            return false;
        }

        return slotIndexByConnectionId.ContainsKey(connection.ConnectionId);
    }

    private void ClearRoom() {
        slotIndexByConnectionId.Clear();
        roomState = null;
        currentWorldId = 0;
    }
}
