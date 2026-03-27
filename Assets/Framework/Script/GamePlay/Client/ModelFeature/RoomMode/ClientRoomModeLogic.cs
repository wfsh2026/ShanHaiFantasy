using UnityEngine;

/// <summary>
/// A1 房间模式逻辑。
/// 负责启动 Host / Client、发送房间命令，并把服务端状态转换为客户端 Data。
/// </summary>
public sealed class ClientRoomModeLogic : AbsModeLogic {
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

    private ClientRoomModeData data;
    private ClientRoomNetProxy roomProxy;
    private NetworkSyncRoomClientModule roomClientModule;
    private NetworkSyncRoomServerModule roomServerModule;

    public override void OnInit() {
        ClientRoomModeManager modeManager = manager as ClientRoomModeManager;
        if (modeManager == null) {
            return;
        }

        data = modeManager.GetData<ClientRoomModeData>();
        RegisterNetworkModules();
    }

    public override void OnClear() {
        UnregisterRoomModuleCallbacks();
        NetworkSyncMirrorRoomRuntime.Instance.StopRuntime();
        roomProxy = null;
        roomClientModule = null;
        roomServerModule = null;
        data = null;
    }

    public void CreateRoom() {
        if (data == null) {
            return;
        }

        string inviteCode = NetworkSyncMirrorRoomRuntime.GenerateInviteCode();
        RoomLocalProfile localProfile = BuildLocalProfile(true);
        data.SetWaitingStatus("正在创建房间...");

        NetworkSyncMirrorRoomRuntime.Instance.StartHostRuntime(
            gameWorld,
            inviteCode,
            () => {
                if (roomProxy != null) {
                    roomProxy.CreateRoom(inviteCode, localProfile.DisplayName, localProfile.AvatarId);
                }
            },
            HandleOperationFailed);
    }

    public void JoinRoom(string inviteCode) {
        if (data == null) {
            return;
        }

        if (string.IsNullOrWhiteSpace(inviteCode)) {
            HandleOperationFailed("连接失败");
            return;
        }

        RoomLocalProfile localProfile = BuildLocalProfile(false);
        data.SetWaitingStatus("正在连接房间...");
        NetworkSyncMirrorRoomRuntime.Instance.StartClientRuntime(
            gameWorld,
            inviteCode.Trim(),
            () => {
                if (roomProxy != null) {
                    roomProxy.JoinRoom(inviteCode.Trim(), localProfile.DisplayName, localProfile.AvatarId);
                }
            },
            HandleOperationFailed);
    }

    public void LeaveRoom() {
        if (roomProxy == null || data == null) {
            return;
        }

        bool isHost = data.IsHostValue.Value;
        roomProxy.LeaveRoom();

        if (!isHost) {
            NetworkSyncMirrorRoomRuntime.Instance.StopRuntime();
            data.SetEntryState();
        }
    }

    public void StartRoom() {
        if (roomProxy == null || data == null || !data.CanStartValue.Value) {
            return;
        }

        data.SetWaitingStatus("正在进入战斗场景...");
        roomProxy.StartRoom();
    }

    public void ConfirmNotice() {
        if (data == null) {
            return;
        }

        bool shouldReturnToEntry = data.ShouldReturnToEntryOnNoticeConfirm;
        data.ClearNoticeRequest();
        if (shouldReturnToEntry) {
            NetworkSyncMirrorRoomRuntime.Instance.StopRuntime();
            data.SetEntryState();
        }
    }

    private void RegisterNetworkModules() {
        ClientNetworkFeatureManager clientNetwork = gameWorld.GetExtendFeature<ClientNetworkFeatureManager>();
        if (clientNetwork != null) {
            roomClientModule = new NetworkSyncRoomClientModule();
            clientNetwork.RegisterModule(roomClientModule);
            roomProxy = new ClientRoomNetProxy(roomClientModule, 0);
            RegisterRoomModuleCallbacks();
        }

        ServerNetworkFeatureManager serverNetwork = gameWorld.GetExtendFeature<ServerNetworkFeatureManager>();
        if (serverNetwork != null) {
            roomServerModule = new NetworkSyncRoomServerModule(SceneId.BattleTest.ToString());
            serverNetwork.RegisterModule(roomServerModule);
        }
    }

    private void RegisterRoomModuleCallbacks() {
        if (roomClientModule == null) {
            return;
        }

        roomClientModule.RoomStateReceived += OnRoomStateReceived;
        roomClientModule.RoomStarted += OnRoomStarted;
        roomClientModule.RoomDisbanded += OnRoomDisbanded;
        roomClientModule.OperationFailed += OnRoomOperationFailed;
    }

    private void UnregisterRoomModuleCallbacks() {
        if (roomClientModule == null) {
            return;
        }

        roomClientModule.RoomStateReceived -= OnRoomStateReceived;
        roomClientModule.RoomStarted -= OnRoomStarted;
        roomClientModule.RoomDisbanded -= OnRoomDisbanded;
        roomClientModule.OperationFailed -= OnRoomOperationFailed;
    }

    private void OnRoomStateReceived(NetworkSyncRoomStateRpc roomState) {
        if (data == null || roomClientModule == null) {
            return;
        }

        data.SetRoomState(roomState, roomClientModule.LocalPlayerId);
    }

    private void OnRoomStarted(NetworkSyncRoomStartRpc startMessage) {
        if (data == null || gameWorld == null) {
            return;
        }

        data.SetWaitingStatus("正在进入战斗场景...");

        ClientSceneFlowFeatureManager sceneFlowFeatureManager = gameWorld.GetExtendFeature<ClientSceneFlowFeatureManager>();
        if (sceneFlowFeatureManager == null || sceneFlowFeatureManager.SceneFlowManager == null) {
            return;
        }

        SceneConfig config = sceneFlowFeatureManager.SceneFlowManager.GetCurrentSceneId() == SceneId.BattleTest
            ? null
            : new SceneRegistry().GetConfig(SceneId.BattleTest);
        if (config == null) {
            return;
        }

        SceneRequest request = sceneFlowFeatureManager.SceneFlowManager.BuildDefaultRequest(config, "EnterBattleFromRoom");
        sceneFlowFeatureManager.SceneFlowManager.LoadScene(request);
    }

    private void OnRoomDisbanded(NetworkSyncRoomDisbandRpc message) {
        if (data == null) {
            return;
        }

        NetworkSyncMirrorRoomRuntime.Instance.StopRuntime();
        data.SetEntryState();
        data.SetNoticeRequest("房间通知", message == null ? "房间已解散" : message.messageText, true);
    }

    private void OnRoomOperationFailed(NetworkSyncRoomOperationFailedTargetRpc message) {
        HandleOperationFailed(message == null ? "连接失败" : message.messageText);
    }

    private void HandleOperationFailed(string messageText) {
        if (data == null) {
            return;
        }

        NetworkSyncMirrorRoomRuntime.Instance.StopRuntime();
        data.SetEntryState();
        data.SetNoticeRequest("提示", string.IsNullOrWhiteSpace(messageText) ? "连接失败" : messageText, true);
    }

    private static RoomLocalProfile BuildLocalProfile(bool isHost) {
        RoomLocalProfile profile = new RoomLocalProfile();
        int randomIndex = Random.Range(0, PLAYER_NAME_POOL.Length);
        profile.DisplayName = PLAYER_NAME_POOL[randomIndex] + "_" + Random.Range(10, 99);
        if (isHost) {
            profile.DisplayName = profile.DisplayName + "(Host)";
        }

        profile.AvatarId = PLAYER_AVATAR_POOL[randomIndex % PLAYER_AVATAR_POOL.Length];
        return profile;
    }

    private struct RoomLocalProfile {
        public string DisplayName;
        public string AvatarId;
    }
}
