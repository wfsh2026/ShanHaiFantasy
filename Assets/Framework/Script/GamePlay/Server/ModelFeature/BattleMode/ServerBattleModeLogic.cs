using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class ServerBattleModeLogic : AbsModeLogic {
    private const float MATCH_INIT_SECONDS = 0.2f;
    private const float HERO_SELECT_TIMEOUT_SECONDS = 12f;
    private const float SECT_REVEAL_SECONDS = 0.7f;
    private const float ROUND_LOOP_STEP_SECONDS = 0.2f;
    private const float CULTIVATION_SELECT_TIMEOUT_SECONDS = 12f;

    private BattleModeData data;
    private ServerBattleModeManager modeManager;
    private NetworkSyncBattleServerModule battleModule;
    private ServerRoomNetHandler roomHandler;
    private int broadcastWorldId;
    private float phaseTimer;
    private float roundPhaseElapsedTime;

    public override void OnInit() {
        modeManager = manager as ServerBattleModeManager;
        if (modeManager == null) {
            return;
        }

        data = manager.GetData<BattleModeData>();
        if (data != null) {
            data.SetAuthoritative(true);
        }

        phaseTimer = 0f;
        roundPhaseElapsedTime = 0f;
        broadcastWorldId = 0;

        NetworkSyncRoomStateRpc roomSnapshot = null;
        string matchId = string.Empty;

        ServerNetworkFeatureManager serverNetwork = modeManager.GameWorld.GetExtendFeature<ServerNetworkFeatureManager>();
        if (serverNetwork != null) {
            NetworkSyncRoomServerModule roomModule;
            if (serverNetwork.TryGetModule<NetworkSyncRoomServerModule>(out roomModule) && roomModule != null) {
                roomHandler = new ServerRoomNetHandler(roomModule);
                roomSnapshot = roomHandler.GetRoomStateSnapshot();
                matchId = roomHandler.CurrentMatchId;
                broadcastWorldId = roomHandler.CurrentWorldId;
            }

            if (!serverNetwork.TryGetModule<NetworkSyncBattleServerModule>(out battleModule) || battleModule == null) {
                battleModule = new NetworkSyncBattleServerModule();
                serverNetwork.RegisterModule(battleModule);
            }

            if (battleModule != null) {
                battleModule.SetPlayerIdResolver(connectionId => {
                    if (roomModule == null) {
                        return string.Empty;
                    }

                    roomModule.TryGetPlayerIdByConnectionId(connectionId, out string playerId);
                    return playerId;
                });
                battleModule.BattleHeroSelectRequested += OnBattleHeroSelectRequested;
                battleModule.BattleCultivationSelectRequested += OnBattleCultivationSelectRequested;
                battleModule.BattleFormationConfirmRequested += OnBattleFormationConfirmRequested;
                battleModule.BattleDebugActionRequested += OnBattleDebugActionRequested;
            }
        }

        NetworkSyncRoomStartRpc preservedRoomStart;
        int preservedWorldId;
        if (IsRoomSnapshotEmpty(roomSnapshot) &&
            NetworkSyncMirrorRoomRuntime.TryGetPreservedRoomBootstrap(out NetworkSyncRoomStateRpc preservedRoomState, out preservedRoomStart, out preservedWorldId)) {
            roomSnapshot = preservedRoomState;
            if (string.IsNullOrWhiteSpace(matchId) && preservedRoomStart != null) {
                matchId = preservedRoomStart.matchId ?? string.Empty;
            }

            if (broadcastWorldId <= 0) {
                broadcastWorldId = preservedWorldId;
            }

            Debug.Log("ServerBattleModeLogic fallback to preserved room bootstrap. slots=" +
                GetRoomSlotCount(roomSnapshot) + ", matchId=" + matchId);
        }

        if (data != null) {
            data.ApplyRoomSnapshot(roomSnapshot, ResolveBattleSceneId(), matchId);
            Debug.Log("ServerBattleModeLogic initialized battle snapshot. slots=" +
                GetBattleParticipantCount(data.Participants) + ", matchId=" + data.MatchId);
        }

        if (data != null && !data.HasSnapshot) {
            data.ResetState();
            data.SetAuthoritative(true);
            data.MatchIdValue.SetValue(matchId);
            data.RoomInviteCodeValue.SetValue(roomHandler == null ? string.Empty : roomHandler.CurrentInviteCode);
            data.SceneIdValue.SetValue(ResolveBattleSceneId());
            data.ParticipantListValue.SetValue(new BattleParticipantSnapshot[0]);
            data.AliveCountValue.SetValue(0);
            data.StateVersionValue.SetValue(1);
            data.ApplySnapshot(data.BuildSnapshot());
        }

    }

    public override void OnClear() {
        if (battleModule != null) {
            battleModule.BattleHeroSelectRequested -= OnBattleHeroSelectRequested;
            battleModule.BattleCultivationSelectRequested -= OnBattleCultivationSelectRequested;
            battleModule.BattleFormationConfirmRequested -= OnBattleFormationConfirmRequested;
            battleModule.BattleDebugActionRequested -= OnBattleDebugActionRequested;
        }

        data = null;
        modeManager = null;
        battleModule = null;
        roomHandler = null;
        phaseTimer = 0f;
        roundPhaseElapsedTime = 0f;
    }

    public void Tick(float delta) {
        if (data == null || battleModule == null) {
            return;
        }

        data.Tick(delta);
        phaseTimer += delta;
        if (data.MainState == BattleMainStateType.RoundLoop) {
            roundPhaseElapsedTime += delta;
        }

        switch (data.MainState) {
            case BattleMainStateType.MatchInit:
                TickMatchInit();
                break;
            case BattleMainStateType.HeroSelect:
                TickHeroSelect();
                break;
            case BattleMainStateType.SectReveal:
                TickSectReveal();
                break;
            case BattleMainStateType.RoundLoop:
                TickRoundLoop();
                break;
            case BattleMainStateType.GameOver:
                break;
        }
    }

    public void BroadcastCurrentSnapshot() {
        PushCurrentSnapshot();
    }

    private void TickMatchInit() {
        if (phaseTimer < MATCH_INIT_SECONDS) {
            return;
        }

        EnterHeroSelect();
    }

    private void TickHeroSelect() {
        if (data.AreAllActiveParticipantsLocked()) {
            EnterSectReveal();
            return;
        }

        if (phaseTimer < HERO_SELECT_TIMEOUT_SECONDS) {
            return;
        }

        data.AutoSelectPendingHeroChoices("Timeout", true);
        EnterSectReveal();
    }

    private void TickSectReveal() {
        if (phaseTimer < SECT_REVEAL_SECONDS) {
            return;
        }

        EnterRoundLoop();
    }

    private void TickRoundLoop() {
        if (phaseTimer < ROUND_LOOP_STEP_SECONDS) {
            return;
        }

        phaseTimer = 0f;
        switch (data.RoundPhase) {
            case BattleRoundPhaseType.Battle:
                CompleteBattlePhase();
                break;
            case BattleRoundPhaseType.BattleResult:
                CompleteBattleResultPhase();
                break;
            case BattleRoundPhaseType.Cultivation:
                if (data.AreAllActiveParticipantsCultivationSelected()) {
                    CompleteCultivationPhase();
                    break;
                }

                if (roundPhaseElapsedTime >= CULTIVATION_SELECT_TIMEOUT_SECONDS) {
                    data.AutoSelectPendingCultivationChoices("Timeout", true);
                    CompleteCultivationPhase();
                }
                break;
            case BattleRoundPhaseType.FormationConfirm:
                if (data.AreAllActiveParticipantsFormationConfirmed()) {
                    CompleteFormationConfirmPhase();
                    break;
                }

                if (roundPhaseElapsedTime >= CULTIVATION_SELECT_TIMEOUT_SECONDS) {
                    data.AutoConfirmPendingFormationChoices("Timeout", true);
                    CompleteFormationConfirmPhase();
                }
                break;
        }
    }

    private void EnterHeroSelect() {
        phaseTimer = 0f;
        data.UpdateMainState(BattleMainStateType.HeroSelect);
        data.UpdateRoundIndex(0);
        data.UpdateRoundPhase(BattleRoundPhaseType.Battle);
        data.PrepareHeroSelectPhase();
        data.AutoSelectPendingHeroChoices("AI", false);
        ApplyStageAndBroadcast(BattleMainStateType.HeroSelect);
    }

    private void EnterSectReveal() {
        phaseTimer = 0f;
        data.AssignSectEnvironment(BattleA2ContentCatalog.BuildSectEnvironment(data.RoundIndex, data.AliveCount));
        data.UpdateMainState(BattleMainStateType.SectReveal);
        data.MarkAllActiveParticipantsCompletedCurrentPhase();
        ApplyStageAndBroadcast(BattleMainStateType.SectReveal);
    }

    private void EnterRoundLoop() {
        phaseTimer = 0f;
        roundPhaseElapsedTime = 0f;
        data.UpdateMainState(BattleMainStateType.RoundLoop);
        data.UpdateRoundIndex(1);
        data.UpdateRoundPhase(BattleRoundPhaseType.Battle);
        data.ResetCurrentPhaseCompletion();
        data.PrepareBattleRound();
        ApplyStageAndBroadcast(BattleMainStateType.RoundLoop);
    }

    private void OnBattleHeroSelectRequested(
        NetworkSyncServerContext context,
        NetworkSyncBattleHeroSelectCmd message) {
        if (data == null || message == null) {
            return;
        }

        if (!string.Equals(data.MatchId ?? string.Empty, message.matchId ?? string.Empty, System.StringComparison.Ordinal)) {
            return;
        }

        if (data.MainState != BattleMainStateType.HeroSelect) {
            return;
        }

        if (!data.TrySubmitHeroSelection(message.playerId, message.candidateIndex, "Manual")) {
            PushCurrentSnapshot();
            return;
        }

        if (data.AreAllActiveParticipantsLocked()) {
            EnterSectReveal();
            return;
        }

        PushCurrentSnapshot();
    }

    private void OnBattleCultivationSelectRequested(
        NetworkSyncServerContext context,
        NetworkSyncBattleCultivationSelectCmd message) {
        if (data == null || message == null) {
            return;
        }

        if (!string.Equals(data.MatchId ?? string.Empty, message.matchId ?? string.Empty, System.StringComparison.Ordinal)) {
            return;
        }

        if (data.MainState != BattleMainStateType.RoundLoop ||
            data.RoundPhase != BattleRoundPhaseType.Cultivation) {
            return;
        }

        if (!data.TrySubmitCultivationSelection(message.playerId, message.optionIndex, "Manual")) {
            PushCurrentSnapshot();
            return;
        }

        if (data.AreAllActiveParticipantsCultivationSelected()) {
            CompleteCultivationPhase();
            return;
        }

        PushCurrentSnapshot();
    }

    private void OnBattleFormationConfirmRequested(
        NetworkSyncServerContext context,
        NetworkSyncBattleFormationConfirmCmd message) {
        if (data == null || message == null) {
            return;
        }

        if (!string.Equals(data.MatchId ?? string.Empty, message.matchId ?? string.Empty, System.StringComparison.Ordinal)) {
            return;
        }

        if (!data.TrySubmitFormationChoice(message.playerId, message.position, "Manual")) {
            PushCurrentSnapshot();
            return;
        }

        if (data.AreAllActiveParticipantsFormationConfirmed()) {
            CompleteFormationConfirmPhase();
            return;
        }

        PushCurrentSnapshot();
    }

    private void OnBattleDebugActionRequested(
        NetworkSyncServerContext context,
        NetworkSyncBattleDebugActionCmd message) {
        if (data == null || message == null) {
            return;
        }

        if (!string.Equals(data.MatchId ?? string.Empty, message.matchId ?? string.Empty, System.StringComparison.Ordinal)) {
            return;
        }

        ApplyDebugAction(message.playerId, message.actionType, message.intValue);
        PushCurrentSnapshot();
    }

    private void CompleteBattlePhase() {
        if (data.ApplyBattleResults()) {
            ApplyStageAndBroadcast(BattleMainStateType.GameOver);
            return;
        }

        roundPhaseElapsedTime = 0f;
        data.UpdateRoundPhase(BattleRoundPhaseType.BattleResult);
        data.MarkAllActiveParticipantsCompletedCurrentPhase();
        PushCurrentSnapshot();
    }

    private void CompleteBattleResultPhase() {
        roundPhaseElapsedTime = 0f;
        data.UpdateRoundPhase(BattleRoundPhaseType.Cultivation);
        data.PrepareCultivationPhase();
        if (data.AreAllActiveParticipantsCultivationSelected()) {
            CompleteCultivationPhase();
            return;
        }

        PushCurrentSnapshot();
    }

    private void CompleteCultivationPhase() {
        roundPhaseElapsedTime = 0f;
        data.UpdateRoundPhase(BattleRoundPhaseType.FormationConfirm);
        data.PrepareFormationConfirmPhase();
        data.AutoConfirmPendingFormationChoices("AI", false);
        PushCurrentSnapshot();
    }

    private void CompleteFormationConfirmPhase() {
        roundPhaseElapsedTime = 0f;
        data.CommitCultivationGrowth();
        data.UpdateRoundIndex(data.RoundIndex + 1);
        data.UpdateRoundPhase(BattleRoundPhaseType.Battle);
        data.ResetCurrentPhaseCompletion();
        data.PrepareBattleRound();
        PushCurrentSnapshot();
    }

    private void ApplyDebugAction(string playerId, BattleDebugActionType actionType, int intValue) {
        switch (actionType) {
            case BattleDebugActionType.SkipPhase:
                ForceSkipCurrentPhase();
                break;
            case BattleDebugActionType.CycleEnvironment:
                data.AssignSectEnvironment(BattleA2ContentCatalog.BuildSectEnvironment(data.RoundIndex + 1 + intValue, data.AliveCount));
                break;
            case BattleDebugActionType.CycleFormationEye:
                data.AssignFormationSnapshot(BattleBContentCatalog.BuildFormationSnapshot(data.RoundIndex + 1 + intValue, data.SectEnvironment));
                break;
            case BattleDebugActionType.AddLuck:
                ModifyParticipantLuck(playerId, Mathf.Max(1, intValue == 0 ? 1 : intValue));
                break;
            case BattleDebugActionType.AddSwordIntent:
                ModifyParticipantSwordIntent(playerId, Mathf.Max(1, intValue == 0 ? 1 : intValue));
                break;
            case BattleDebugActionType.ForceFront:
                data.TrySubmitFormationChoice(playerId, BattleFormationPositionType.Front, "Debug");
                break;
            case BattleDebugActionType.ForceMiddle:
                data.TrySubmitFormationChoice(playerId, BattleFormationPositionType.Middle, "Debug");
                break;
            case BattleDebugActionType.ForceRear:
                data.TrySubmitFormationChoice(playerId, BattleFormationPositionType.Rear, "Debug");
                break;
            case BattleDebugActionType.RefreshCultivation:
                if (data.MainState == BattleMainStateType.RoundLoop && data.RoundPhase == BattleRoundPhaseType.Cultivation) {
                    data.PrepareCultivationPhase();
                }
                break;
        }
    }

    private void ForceSkipCurrentPhase() {
        if (data.MainState == BattleMainStateType.MatchInit) {
            EnterHeroSelect();
            return;
        }

        if (data.MainState == BattleMainStateType.HeroSelect) {
            data.AutoSelectPendingHeroChoices("Debug", true);
            EnterSectReveal();
            return;
        }

        if (data.MainState == BattleMainStateType.SectReveal) {
            EnterRoundLoop();
            return;
        }

        if (data.MainState != BattleMainStateType.RoundLoop) {
            return;
        }

        switch (data.RoundPhase) {
            case BattleRoundPhaseType.Battle:
                CompleteBattlePhase();
                break;
            case BattleRoundPhaseType.BattleResult:
                CompleteBattleResultPhase();
                break;
            case BattleRoundPhaseType.Cultivation:
                data.AutoSelectPendingCultivationChoices("Debug", true);
                CompleteCultivationPhase();
                break;
            case BattleRoundPhaseType.FormationConfirm:
                data.AutoConfirmPendingFormationChoices("Debug", true);
                CompleteFormationConfirmPhase();
                break;
        }
    }

    private void ModifyParticipantLuck(string playerId, int delta) {
        BattleParticipantSnapshot[] participants = data.CreateParticipantsClone();
        for (int i = 0; i < participants.Length; i++) {
            BattleParticipantSnapshot participant = participants[i];
            if (participant == null || !string.Equals(participant.playerId ?? string.Empty, playerId ?? string.Empty, System.StringComparison.Ordinal)) {
                continue;
            }

            participant.luckValue = Mathf.Max(0, participant.luckValue + delta);
            participant.qiLuckCurrent = participant.luckValue;
            break;
        }

        data.ReplaceParticipants(participants);
    }

    private void ModifyParticipantSwordIntent(string playerId, int delta) {
        BattleParticipantSnapshot[] participants = data.CreateParticipantsClone();
        for (int i = 0; i < participants.Length; i++) {
            BattleParticipantSnapshot participant = participants[i];
            if (participant == null || !string.Equals(participant.playerId ?? string.Empty, playerId ?? string.Empty, System.StringComparison.Ordinal)) {
                continue;
            }

            participant.swordIntentCurrent += delta;
            participant.swordIntentValue += delta;
            if (participant.buildProfile != null) {
                participant.buildProfile.swordIntent += delta;
            }
            break;
        }

        data.ReplaceParticipants(participants);
    }

    private void ApplyStageAndBroadcast(BattleMainStateType state) {
        if (modeManager == null) {
            return;
        }

        switch (state) {
            case BattleMainStateType.MatchInit:
                modeManager.ChangeStage<BattleMatchInitStage>();
                break;
            case BattleMainStateType.HeroSelect:
                modeManager.ChangeStage<BattleHeroSelectStage>();
                break;
            case BattleMainStateType.SectReveal:
                modeManager.ChangeStage<BattleSectRevealStage>();
                break;
            case BattleMainStateType.RoundLoop:
                modeManager.ChangeStage<BattleRoundLoopStage>();
                break;
            case BattleMainStateType.GameOver:
                modeManager.ChangeStage<BattleGameOverStage>();
                break;
        }

        PushCurrentSnapshot();
    }

    private void PushCurrentSnapshot() {
        if (battleModule == null || data == null) {
            return;
        }

        battleModule.SetCurrentState(data.BuildSnapshot());
        battleModule.BroadcastCurrentState(broadcastWorldId);
    }

    private static bool IsRoomSnapshotEmpty(NetworkSyncRoomStateRpc roomSnapshot) {
        return roomSnapshot == null || roomSnapshot.slots == null || roomSnapshot.slots.Length == 0;
    }

    private static int GetRoomSlotCount(NetworkSyncRoomStateRpc roomSnapshot) {
        return roomSnapshot == null || roomSnapshot.slots == null ? 0 : roomSnapshot.slots.Length;
    }

    private static int GetBattleParticipantCount(BattleParticipantSnapshot[] participants) {
        return participants == null ? 0 : participants.Length;
    }

    private static string ResolveBattleSceneId() {
        string sceneName = SceneManager.GetActiveScene().name;
        return string.IsNullOrWhiteSpace(sceneName) ? "BattleTestScene" : sceneName;
    }
}
