using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class ClientBattleModeLogic : AbsModeLogic {

    private BattleModeData data;
    private ClientBattleModeManager modeManager;
    private NetworkSyncBattleClientModule battleModule;
    private NetworkSyncRoomClientModule roomModule;

    public string LocalPlayerId {
        get {
            return roomModule == null ? string.Empty : roomModule.LocalPlayerId;
        }
    }

    public override void OnInit() {
        modeManager = manager as ClientBattleModeManager;
        if (modeManager == null) {
            return;
        }

        data = manager.GetData<BattleModeData>();
        data.SetAuthoritative(false);

        ClientNetworkFeatureManager clientNetwork = modeManager.GameWorld.GetExtendFeature<ClientNetworkFeatureManager>();
        if (clientNetwork != null) {
            clientNetwork.TryGetModule<NetworkSyncRoomClientModule>(out roomModule);
            if (!clientNetwork.TryGetModule<NetworkSyncBattleClientModule>(out battleModule) || battleModule == null) {
                battleModule = new NetworkSyncBattleClientModule();
                clientNetwork.RegisterModule(battleModule);
            }

            battleModule.BattleStateReceived += OnBattleStateReceived;
        }

        BootstrapFromRoom();
        RequestSnapshot();
    }

    public override void OnClear() {
        if (battleModule != null) {
            battleModule.BattleStateReceived -= OnBattleStateReceived;
        }

        data = null;
        modeManager = null;
        battleModule = null;
        roomModule = null;
    }

    public void Tick(float delta) {
        if (data == null) {
            return;
        }

        data.Tick(delta);
    }

    public void RequestBattleSnapshot() {
        RequestSnapshot();
    }

    public void SubmitHeroSelection(int candidateIndex) {
        if (battleModule == null || data == null) {
            return;
        }

        if (data.MainState != BattleMainStateType.HeroSelect) {
            Debug.LogWarning("ClientBattleModeLogic skipped hero selection because state is not HeroSelect.");
            return;
        }

        if (roomModule == null || string.IsNullOrWhiteSpace(roomModule.LocalPlayerId)) {
            Debug.LogWarning("ClientBattleModeLogic skipped hero selection because local player id is empty.");
            return;
        }

        if (string.IsNullOrWhiteSpace(data.MatchId)) {
            Debug.LogWarning("ClientBattleModeLogic skipped hero selection because matchId is empty.");
            return;
        }

        battleModule.SendHeroSelect(data.MatchId, roomModule.LocalPlayerId, candidateIndex);
    }

    public void SubmitCultivationSelection(int optionIndex) {
        if (battleModule == null || data == null) {
            return;
        }

        if (data.MainState != BattleMainStateType.RoundLoop || data.RoundPhase != BattleRoundPhaseType.Cultivation) {
            Debug.LogWarning("ClientBattleModeLogic skipped cultivation selection because state is not Cultivation.");
            return;
        }

        if (roomModule == null || string.IsNullOrWhiteSpace(roomModule.LocalPlayerId)) {
            Debug.LogWarning("ClientBattleModeLogic skipped cultivation selection because local player id is empty.");
            return;
        }

        if (string.IsNullOrWhiteSpace(data.MatchId)) {
            Debug.LogWarning("ClientBattleModeLogic skipped cultivation selection because matchId is empty.");
            return;
        }

        battleModule.SendCultivationSelect(data.MatchId, roomModule.LocalPlayerId, optionIndex);
    }

    public void SubmitFormationChoice(BattleFormationPositionType position) {
        if (battleModule == null || data == null) {
            return;
        }

        if (data.MainState != BattleMainStateType.RoundLoop || data.RoundPhase != BattleRoundPhaseType.FormationConfirm) {
            Debug.LogWarning("ClientBattleModeLogic skipped formation confirm because state is not FormationConfirm.");
            return;
        }

        if (roomModule == null || string.IsNullOrWhiteSpace(roomModule.LocalPlayerId)) {
            Debug.LogWarning("ClientBattleModeLogic skipped formation confirm because local player id is empty.");
            return;
        }

        if (string.IsNullOrWhiteSpace(data.MatchId)) {
            Debug.LogWarning("ClientBattleModeLogic skipped formation confirm because matchId is empty.");
            return;
        }

        battleModule.SendFormationConfirm(data.MatchId, roomModule.LocalPlayerId, position);
    }

    public void SubmitDebugAction(BattleDebugActionType actionType, int intValue = 0) {
        if (battleModule == null || data == null) {
            return;
        }

        if (roomModule == null || string.IsNullOrWhiteSpace(roomModule.LocalPlayerId)) {
            Debug.LogWarning("ClientBattleModeLogic skipped debug action because local player id is empty.");
            return;
        }

        if (string.IsNullOrWhiteSpace(data.MatchId)) {
            Debug.LogWarning("ClientBattleModeLogic skipped debug action because matchId is empty.");
            return;
        }

        battleModule.SendDebugAction(data.MatchId, roomModule.LocalPlayerId, actionType, intValue);
    }

    public void SubmitFormationPosition(BattleFormationPositionType position) {
        if (battleModule == null || data == null) {
            return;
        }

        if (data.MainState != BattleMainStateType.RoundLoop || data.RoundPhase != BattleRoundPhaseType.FormationConfirm) {
            Debug.LogWarning("ClientBattleModeLogic skipped formation selection because state is not FormationConfirm.");
            return;
        }

        if (roomModule == null || string.IsNullOrWhiteSpace(roomModule.LocalPlayerId) || string.IsNullOrWhiteSpace(data.MatchId)) {
            Debug.LogWarning("ClientBattleModeLogic skipped formation selection because local player or match is empty.");
            return;
        }

        battleModule.SendFormationConfirm(data.MatchId, roomModule.LocalPlayerId, position);
    }

    public void SendDebugAction(BattleDebugActionType actionType, int intValue = 0) {
        if (battleModule == null || data == null || roomModule == null) {
            return;
        }

        if (string.IsNullOrWhiteSpace(roomModule.LocalPlayerId) || string.IsNullOrWhiteSpace(data.MatchId)) {
            Debug.LogWarning("ClientBattleModeLogic skipped debug action because local player or match is empty.");
            return;
        }

        battleModule.SendDebugAction(data.MatchId, roomModule.LocalPlayerId, actionType, intValue);
    }

    private void BootstrapFromRoom() {
        if (data == null) {
            return;
        }

        NetworkSyncRoomStateRpc roomState = roomModule == null ? null : roomModule.LastRoomState;
        NetworkSyncRoomStartRpc roomStart = roomModule == null ? null : roomModule.LastRoomStart;
        if (IsRoomSnapshotEmpty(roomState)) {
            NetworkSyncRoomStartRpc preservedRoomStart;
            int _;
            if (NetworkSyncMirrorRoomRuntime.TryGetPreservedRoomBootstrap(out NetworkSyncRoomStateRpc preservedRoomState, out preservedRoomStart, out _)) {
                roomState = preservedRoomState;
                if (roomStart == null) {
                    roomStart = preservedRoomStart;
                }
            }
        }

        if (roomState != null) {
            string matchId = roomStart == null ? string.Empty : roomStart.matchId;
            data.ApplyRoomSnapshot(roomState, ResolveBattleSceneId(), matchId);
            ApplyStageForState(data.MainState);
            return;
        }

        if (roomStart != null) {
            data.MatchIdValue.SetValue(roomStart.matchId ?? string.Empty);
            data.RoomInviteCodeValue.SetValue(roomStart.inviteCode ?? string.Empty);
            data.SceneIdValue.SetValue(ResolveBattleSceneId());
        }
    }

    private void RequestSnapshot() {
        if (battleModule == null || data == null) {
            return;
        }

        if (string.IsNullOrWhiteSpace(data.MatchId)) {
            Debug.LogWarning("ClientBattleModeLogic skipped battle snapshot request because matchId is empty.");
            return;
        }

        try {
            battleModule.RequestSnapshot(data.MatchId, 0);
        } catch (System.InvalidOperationException exception) {
            Debug.LogWarning("ClientBattleModeLogic snapshot request skipped because battle module is not ready.\n" + exception);
        }
    }

    private void OnBattleStateReceived(NetworkSyncBattleStateRpc snapshot) {
        if (data == null || snapshot == null) {
            return;
        }

        BattleMainStateType previousMainState = data.MainState;
        data.ApplySnapshot(snapshot);
        ApplyStageForState(snapshot.mainState);

        if (previousMainState != snapshot.mainState) {
            Debug.Log("Client battle main state changed: " + previousMainState + " -> " + snapshot.mainState);
        }
    }

    private void ApplyStageForState(BattleMainStateType state) {
        if (modeManager == null) {
            return;
        }

        switch (state) {
            case BattleMainStateType.MatchInit:
                if (!modeManager.RunningStageIs<BattleMatchInitStage>()) {
                    modeManager.ChangeStage<BattleMatchInitStage>();
                }
                break;
            case BattleMainStateType.HeroSelect:
                if (!modeManager.RunningStageIs<BattleHeroSelectStage>()) {
                    modeManager.ChangeStage<BattleHeroSelectStage>();
                }
                break;
            case BattleMainStateType.SectReveal:
                if (!modeManager.RunningStageIs<BattleSectRevealStage>()) {
                    modeManager.ChangeStage<BattleSectRevealStage>();
                }
                break;
            case BattleMainStateType.RoundLoop:
                if (!modeManager.RunningStageIs<BattleRoundLoopStage>()) {
                    modeManager.ChangeStage<BattleRoundLoopStage>();
                }
                break;
            case BattleMainStateType.GameOver:
                if (!modeManager.RunningStageIs<BattleGameOverStage>()) {
                    modeManager.ChangeStage<BattleGameOverStage>();
                }
                break;
        }
    }

    private static bool IsRoomSnapshotEmpty(NetworkSyncRoomStateRpc roomState) {
        return roomState == null || roomState.slots == null || roomState.slots.Length == 0;
    }

    private static string ResolveBattleSceneId() {
        string sceneName = SceneManager.GetActiveScene().name;
        return string.IsNullOrWhiteSpace(sceneName) ? "BattleTestScene" : sceneName;
    }
}
