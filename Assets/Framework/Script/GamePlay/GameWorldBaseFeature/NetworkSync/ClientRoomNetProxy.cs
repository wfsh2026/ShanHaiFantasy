/// <summary>
/// 房间客户端网络代理。
/// 对外只暴露房间行为，不让 UI 或 Logic 直接碰 NetworkSyncClient。
/// </summary>
public sealed class ClientRoomNetProxy {
    private readonly NetworkSyncRoomClientModule roomModule;
    private readonly int worldId;

    public ClientRoomNetProxy(NetworkSyncRoomClientModule roomModule, int worldId) {
        this.roomModule = roomModule;
        this.worldId = worldId;
    }

    public NetworkSyncRoomClientModule RoomModule {
        get {
            return roomModule;
        }
    }

    public void CreateRoom(string inviteCode, string displayName, string avatarId) {
        if (roomModule == null) {
            return;
        }

        roomModule.SendCreateRoomCmd(inviteCode, displayName, avatarId, worldId);
    }

    public void JoinRoom(string inviteCode, string displayName, string avatarId) {
        if (roomModule == null) {
            return;
        }

        roomModule.SendJoinRoomCmd(inviteCode, displayName, avatarId, worldId);
    }

    public void LeaveRoom() {
        if (roomModule == null) {
            return;
        }

        roomModule.SendLeaveRoomCmd(worldId);
    }

    public void StartRoom() {
        if (roomModule == null) {
            return;
        }

        roomModule.SendStartRoomCmd(worldId);
    }
}
