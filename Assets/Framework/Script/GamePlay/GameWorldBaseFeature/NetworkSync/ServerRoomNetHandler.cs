/// <summary>
/// 房间服务端网络门面。
/// 当前只暴露服务端房间运行时的关键状态，供测试和后续房间业务层读取。
/// </summary>
public sealed class ServerRoomNetHandler {
    private readonly NetworkSyncRoomServerModule roomModule;

    public ServerRoomNetHandler(NetworkSyncRoomServerModule roomModule) {
        this.roomModule = roomModule;
    }

    public string CurrentInviteCode {
        get {
            if (roomModule == null) {
                return string.Empty;
            }

            return roomModule.CurrentInviteCode;
        }
    }

    public int PlayerCount {
        get {
            if (roomModule == null) {
                return 0;
            }

            return roomModule.PlayerCount;
        }
    }

    public int AICount {
        get {
            if (roomModule == null) {
                return 0;
            }

            return roomModule.AICount;
        }
    }

    public bool HasStarted {
        get {
            if (roomModule == null) {
                return false;
            }

            return roomModule.HasStarted;
        }
    }

    public int CurrentWorldId {
        get {
            if (roomModule == null) {
                return 0;
            }

            return roomModule.CurrentWorldId;
        }
    }

    public string CurrentMatchId {
        get {
            if (roomModule == null) {
                return string.Empty;
            }

            return roomModule.CurrentMatchId;
        }
    }

    public NetworkSyncRoomStateRpc GetRoomStateSnapshot() {
        if (roomModule == null) {
            return null;
        }

        return roomModule.GetRoomStateSnapshot();
    }
}
