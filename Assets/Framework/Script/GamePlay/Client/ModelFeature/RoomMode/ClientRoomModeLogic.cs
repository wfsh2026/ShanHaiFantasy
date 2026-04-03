using System;
using UnityEngine;

public sealed class ClientRoomModeLogic : AbsModeLogic {
    private static readonly string[] PLAYER_NAME_POOL = {
        "QingYue",
        "LiuYun",
        "JingHong",
        "QingYue2",
        "ChangFeng",
        "SiNan",
        "ZhaoYing",
        "WuQue"
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
    private bool isEnteringBattleScene;

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
        if (!isEnteringBattleScene) {
            NetworkSyncMirrorRoomRuntime.Instance.StopRuntime();
        }

        roomProxy = null;
        roomClientModule = null;
        roomServerModule = null;
        data = null;
        isEnteringBattleScene = false;
    }

    public void CreateRoom() {
        if (data == null) {
            return;
        }

        RoomLocalProfile localProfile = BuildLocalProfile(true);
        data.SetWaitingStatus("Creating room...");

        NetworkSyncMirrorRoomRuntime.Instance.StartHostRuntime(
            gameWorld,
            string.Empty,
            () => {
                if (roomProxy != null) {
                    roomProxy.CreateRoom(string.Empty, localProfile.DisplayName, localProfile.AvatarId);
                }
            },
            HandleOperationFailed);
    }

    public void JoinRoom(string inviteCode) {
        if (data == null) {
            return;
        }

        if (string.IsNullOrWhiteSpace(inviteCode)) {
            HandleOperationFailed("Connect failed");
            return;
        }

        RoomLocalProfile localProfile = BuildLocalProfile(false);
        data.SetWaitingStatus("Joining room...");
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
        isEnteringBattleScene = false;

        if (!isHost) {
            NetworkSyncMirrorRoomRuntime.Instance.StopRuntime();
            data.SetEntryState();
        }
    }

    public void StartRoom() {
        if (roomProxy == null || data == null || !data.CanStartValue.Value) {
            return;
        }

        data.SetWaitingStatus("Entering battle scene...");
        isEnteringBattleScene = true;
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

        data.SetWaitingStatus("Entering battle scene...");
        NetworkSyncMirrorRoomRuntime.Instance.PreserveRoomModules(roomClientModule, roomServerModule);

        ClientSceneFlowFeatureManager sceneFlowFeatureManager = gameWorld.GetExtendFeature<ClientSceneFlowFeatureManager>();
        if (sceneFlowFeatureManager == null || sceneFlowFeatureManager.SceneFlowManager == null) {
            return;
        }

        SceneConfig config = ResolveStartSceneConfig(sceneFlowFeatureManager.SceneFlowManager, startMessage);
        if (config == null) {
            return;
        }

        SceneRequest request = sceneFlowFeatureManager.SceneFlowManager.BuildDefaultRequest(config, "EnterBattleFromRoom");
        sceneFlowFeatureManager.SceneFlowManager.LoadScene(request);
    }

    private static SceneConfig ResolveStartSceneConfig(SceneFlowManager sceneFlowManager, NetworkSyncRoomStartRpc startMessage) {
        if (sceneFlowManager == null || startMessage == null || string.IsNullOrWhiteSpace(startMessage.sceneId)) {
            return null;
        }

        SceneId targetSceneId;
        if (!Enum.TryParse(startMessage.sceneId.Trim(), true, out targetSceneId)) {
            return null;
        }

        if (sceneFlowManager.GetCurrentSceneId() == targetSceneId) {
            return null;
        }

        return new SceneRegistry().GetConfig(targetSceneId);
    }

    private void OnRoomDisbanded(NetworkSyncRoomDisbandRpc message) {
        if (data == null) {
            return;
        }

        isEnteringBattleScene = false;
        NetworkSyncMirrorRoomRuntime.Instance.StopRuntime();
        data.SetEntryState();
        data.SetNoticeRequest("Room Notice", message == null ? "Room has been disbanded" : message.messageText, true);
    }

    private void OnRoomOperationFailed(NetworkSyncRoomOperationFailedTargetRpc message) {
        HandleOperationFailed(message == null ? "Connect failed" : message.messageText);
    }

    private void HandleOperationFailed(string messageText) {
        if (data == null) {
            return;
        }

        isEnteringBattleScene = false;
        NetworkSyncMirrorRoomRuntime.Instance.StopRuntime();
        data.SetEntryState();
        data.SetNoticeRequest("Notice", string.IsNullOrWhiteSpace(messageText) ? "Connect failed" : messageText, true);
    }

    private static RoomLocalProfile BuildLocalProfile(bool isHost) {
        RoomLocalProfile profile = new RoomLocalProfile();
        int randomIndex = UnityEngine.Random.Range(0, PLAYER_NAME_POOL.Length);
        profile.DisplayName = PLAYER_NAME_POOL[randomIndex] + "_" + UnityEngine.Random.Range(10, 99);
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
