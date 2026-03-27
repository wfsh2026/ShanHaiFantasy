using System;

/// <summary>
/// 房间网络客户端模块。
/// 负责发送房间命令，并缓存服务端同步下来的房间状态。
/// </summary>
public sealed class NetworkSyncRoomClientModule : INetworkSyncClientModule {
    private NetworkSyncClient client;

    public event Action<NetworkSyncRoomStateRpc> RoomStateReceived;
    public event Action<NetworkSyncRoomStartRpc> RoomStarted;
    public event Action<NetworkSyncRoomDisbandRpc> RoomDisbanded;
    public event Action<NetworkSyncRoomOperationFailedTargetRpc> OperationFailed;

    public NetworkSyncRoomStateRpc LastRoomState { get; private set; }
    public NetworkSyncRoomStartRpc LastRoomStart { get; private set; }
    public NetworkSyncRoomDisbandRpc LastRoomDisband { get; private set; }
    public string LastFailureMessage { get; private set; }
    public string LocalPlayerId { get; private set; }
    public bool IsLocalHost { get; private set; }

    public void Register(NetworkSyncClient networkClient) {
        client = networkClient ?? throw new ArgumentNullException(nameof(networkClient));
        RegisterProtocolDescriptors();
        client.RegisterRpc<NetworkSyncRoomStateRpc>(NetworkSyncRoomProtocols.RoomStateRpc, OnRoomStateReceivedInternal);
        client.RegisterRpc<NetworkSyncRoomStartRpc>(NetworkSyncRoomProtocols.RoomStartRpc, OnRoomStartReceivedInternal);
        client.RegisterRpc<NetworkSyncRoomDisbandRpc>(NetworkSyncRoomProtocols.RoomDisbandRpc, OnRoomDisbandReceivedInternal);
        client.RegisterTargetRpc<NetworkSyncRoomOperationFailedTargetRpc>(
            NetworkSyncRoomProtocols.RoomOperationFailedTargetRpc,
            OnRoomOperationFailedReceivedInternal);
        client.RegisterTargetRpc<NetworkSyncRoomLocalPlayerAssignedTargetRpc>(
            NetworkSyncRoomProtocols.RoomLocalPlayerAssignedTargetRpc,
            OnRoomLocalPlayerAssignedReceivedInternal);
    }

    public void ResetRuntimeState() {
        LastRoomState = null;
        LastRoomStart = null;
        LastRoomDisband = null;
        LastFailureMessage = string.Empty;
        LocalPlayerId = string.Empty;
        IsLocalHost = false;
    }

    public void SendCreateRoomCmd(string inviteCode, string displayName, string avatarId, int worldId = 0) {
        EnsureRegistered();
        client.SendCmd(new NetworkSyncRoomCreateCmd {
            inviteCode = inviteCode ?? string.Empty,
            displayName = displayName ?? string.Empty,
            avatarId = avatarId ?? string.Empty
        }, worldId);
    }

    public void SendJoinRoomCmd(string inviteCode, string displayName, string avatarId, int worldId = 0) {
        EnsureRegistered();
        client.SendCmd(new NetworkSyncRoomJoinCmd {
            inviteCode = inviteCode ?? string.Empty,
            displayName = displayName ?? string.Empty,
            avatarId = avatarId ?? string.Empty
        }, worldId);
    }

    public void SendLeaveRoomCmd(int worldId = 0) {
        EnsureRegistered();
        client.SendCmd(new NetworkSyncRoomLeaveCmd(), worldId);
    }

    public void SendStartRoomCmd(int worldId = 0) {
        EnsureRegistered();
        client.SendCmd(new NetworkSyncRoomStartCmd(), worldId);
    }

    private void EnsureRegistered() {
        if (client == null) {
            throw new InvalidOperationException("NetworkSyncRoomClientModule is not registered.");
        }
    }

    /// <summary>
    /// 房间模块的客户端既要接收 Rpc，也要主动发送 Cmd。
    /// 这里统一把房间协议的全部描述符注册进客户端 registry，避免发送时按类型查不到 descriptor。
    /// </summary>
    private void RegisterProtocolDescriptors() {
        if (client == null || client.Registry == null) {
            return;
        }

        client.Registry.Register(NetworkSyncRoomProtocols.RoomCreateCmd);
        client.Registry.Register(NetworkSyncRoomProtocols.RoomJoinCmd);
        client.Registry.Register(NetworkSyncRoomProtocols.RoomLeaveCmd);
        client.Registry.Register(NetworkSyncRoomProtocols.RoomStartCmd);
        client.Registry.Register(NetworkSyncRoomProtocols.RoomStateRpc);
        client.Registry.Register(NetworkSyncRoomProtocols.RoomDisbandRpc);
        client.Registry.Register(NetworkSyncRoomProtocols.RoomStartRpc);
        client.Registry.Register(NetworkSyncRoomProtocols.RoomOperationFailedTargetRpc);
        client.Registry.Register(NetworkSyncRoomProtocols.RoomLocalPlayerAssignedTargetRpc);
    }

    private void OnRoomStateReceivedInternal(NetworkSyncClientContext context, NetworkSyncRoomStateRpc message) {
        LastRoomState = message;
        RoomStateReceived?.Invoke(message);
    }

    private void OnRoomStartReceivedInternal(NetworkSyncClientContext context, NetworkSyncRoomStartRpc message) {
        LastRoomStart = message;
        RoomStarted?.Invoke(message);
    }

    private void OnRoomDisbandReceivedInternal(NetworkSyncClientContext context, NetworkSyncRoomDisbandRpc message) {
        LastRoomDisband = message;
        RoomDisbanded?.Invoke(message);
    }

    private void OnRoomOperationFailedReceivedInternal(
        NetworkSyncClientContext context,
        NetworkSyncRoomOperationFailedTargetRpc message) {
        LastFailureMessage = message == null ? string.Empty : message.messageText ?? string.Empty;
        OperationFailed?.Invoke(message);
    }

    private void OnRoomLocalPlayerAssignedReceivedInternal(
        NetworkSyncClientContext context,
        NetworkSyncRoomLocalPlayerAssignedTargetRpc message) {
        if (message == null) {
            LocalPlayerId = string.Empty;
            IsLocalHost = false;
            return;
        }

        LocalPlayerId = message.playerId ?? string.Empty;
        IsLocalHost = message.isHost;
    }
}
