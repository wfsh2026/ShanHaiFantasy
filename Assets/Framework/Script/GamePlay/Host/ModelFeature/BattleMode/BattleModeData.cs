using System;

public sealed partial class BattleModeData : AbsModeData {
    private const string DEFAULT_STAGE_NAME = "BattleModeStage";
    private const string DEFAULT_SCENE_ID = "BattleTestScene";
    private const int DEFAULT_QI_LUCK = 3;

    private readonly BindableValue<string> matchIdValue = new BindableValue<string>(string.Empty);
    private readonly BindableValue<string> roomInviteCodeValue = new BindableValue<string>(string.Empty);
    private readonly BindableValue<string> sceneIdValue = new BindableValue<string>(DEFAULT_SCENE_ID);
    private readonly BindableValue<BattleMainStateType> mainStateValue =
        new BindableValue<BattleMainStateType>(BattleMainStateType.MatchInit);
    private readonly BindableValue<int> roundIndexValue = new BindableValue<int>(0);
    private readonly BindableValue<BattleRoundPhaseType> roundPhaseValue =
        new BindableValue<BattleRoundPhaseType>(BattleRoundPhaseType.Battle);
    private readonly BindableValue<int> aliveCountValue = new BindableValue<int>(0);
    private readonly BindableValue<string> winnerPlayerIdValue = new BindableValue<string>(string.Empty);
    private readonly BindableValue<int> stateVersionValue = new BindableValue<int>(0);
    private readonly BindableValue<string> stageNameValue = new BindableValue<string>(DEFAULT_STAGE_NAME);
    private readonly BindableValue<int> stageEnterCountValue = new BindableValue<int>(0);
    private readonly BindableValue<float> runningTimeValue = new BindableValue<float>(0f);
    private readonly BindableValue<BattleA2SectEnvironmentSnapshot> sectEnvironmentValue =
        new BindableValue<BattleA2SectEnvironmentSnapshot>(null);
    private readonly BindableValue<BattleRoundMatchSnapshot[]> battleMatchListValue =
        new BindableValue<BattleRoundMatchSnapshot[]>(new BattleRoundMatchSnapshot[0]);
    private readonly BindableValue<BattleParticipantSnapshot[]> participantListValue =
        new BindableValue<BattleParticipantSnapshot[]>(new BattleParticipantSnapshot[0]);

    public BindableValue<string> MatchIdValue { get { return matchIdValue; } }
    public BindableValue<string> RoomInviteCodeValue { get { return roomInviteCodeValue; } }
    public BindableValue<string> SceneIdValue { get { return sceneIdValue; } }
    public BindableValue<BattleMainStateType> MainStateValue { get { return mainStateValue; } }
    public BindableValue<int> RoundIndexValue { get { return roundIndexValue; } }
    public BindableValue<BattleRoundPhaseType> RoundPhaseValue { get { return roundPhaseValue; } }
    public BindableValue<int> AliveCountValue { get { return aliveCountValue; } }
    public BindableValue<string> WinnerPlayerIdValue { get { return winnerPlayerIdValue; } }
    public BindableValue<int> StateVersionValue { get { return stateVersionValue; } }
    public BindableValue<string> StageNameValue { get { return stageNameValue; } }
    public BindableValue<int> StageEnterCountValue { get { return stageEnterCountValue; } }
    public BindableValue<float> RunningTimeValue { get { return runningTimeValue; } }
    public BindableValue<BattleA2SectEnvironmentSnapshot> SectEnvironmentValue { get { return sectEnvironmentValue; } }
    public BindableValue<BattleRoundMatchSnapshot[]> BattleMatchListValue { get { return battleMatchListValue; } }
    public BindableValue<BattleParticipantSnapshot[]> ParticipantListValue { get { return participantListValue; } }

    public string MatchId { get { return MatchIdValue.Value; } }
    public string RoomInviteCode { get { return RoomInviteCodeValue.Value; } }
    public string SceneId { get { return SceneIdValue.Value; } }
    public BattleMainStateType MainState { get { return MainStateValue.Value; } }
    public int RoundIndex { get { return RoundIndexValue.Value; } }
    public BattleRoundPhaseType RoundPhase { get { return RoundPhaseValue.Value; } }
    public int AliveCount { get { return AliveCountValue.Value; } }
    public string WinnerPlayerId { get { return WinnerPlayerIdValue.Value; } }
    public int StateVersion { get { return StateVersionValue.Value; } }
    public string StageName { get { return StageNameValue.Value; } }
    public int StageEnterCount { get { return StageEnterCountValue.Value; } }
    public float RunningTime { get { return RunningTimeValue.Value; } }
    public BattleA2SectEnvironmentSnapshot SectEnvironment { get { return SectEnvironmentValue.Value; } }
    public BattleRoundMatchSnapshot[] BattleMatchList { get { return BattleMatchListValue.Value; } }
    public BattleParticipantSnapshot[] Participants { get { return ParticipantListValue.Value; } }

    public bool HasSnapshot { get; private set; }
    public bool IsAuthoritative { get; private set; }

    public override void OnInit() {
        ResetState();
    }

    public override void OnClear() {
        MatchIdValue.ClearListeners();
        RoomInviteCodeValue.ClearListeners();
        SceneIdValue.ClearListeners();
        MainStateValue.ClearListeners();
        RoundIndexValue.ClearListeners();
        RoundPhaseValue.ClearListeners();
        AliveCountValue.ClearListeners();
        WinnerPlayerIdValue.ClearListeners();
        StateVersionValue.ClearListeners();
        StageNameValue.ClearListeners();
        StageEnterCountValue.ClearListeners();
        RunningTimeValue.ClearListeners();
        SectEnvironmentValue.ClearListeners();
        FormationSnapshotValue.ClearListeners();
        BattleMatchListValue.ClearListeners();
        ParticipantListValue.ClearListeners();
    }

    public void ResetState() {
        HasSnapshot = false;
        IsAuthoritative = false;
        MatchIdValue.SetValue(string.Empty);
        RoomInviteCodeValue.SetValue(string.Empty);
        SceneIdValue.SetValue(DEFAULT_SCENE_ID);
        MainStateValue.SetValue(BattleMainStateType.MatchInit);
        RoundIndexValue.SetValue(0);
        RoundPhaseValue.SetValue(BattleRoundPhaseType.Battle);
        AliveCountValue.SetValue(0);
        WinnerPlayerIdValue.SetValue(string.Empty);
        StateVersionValue.SetValue(0);
        StageNameValue.SetValue(DEFAULT_STAGE_NAME);
        StageEnterCountValue.SetValue(0);
        RunningTimeValue.SetValue(0f);
        SectEnvironmentValue.SetValue(null);
        FormationSnapshotValue.SetValue(null);
        BattleMatchListValue.SetValue(new BattleRoundMatchSnapshot[0]);
        ParticipantListValue.SetValue(new BattleParticipantSnapshot[0]);
    }

    public void SetAuthoritative(bool isAuthoritative) {
        IsAuthoritative = isAuthoritative;
    }

    public void Tick(float delta) {
        RunningTimeValue.SetValue(RunningTime + delta);
    }

    public void MarkStageEnter(string stageName) {
        StageEnterCountValue.SetValue(StageEnterCount + 1);
        if (!string.IsNullOrWhiteSpace(stageName)) {
            StageNameValue.SetValue(stageName);
        }
    }

    public void ApplyRoomSnapshot(NetworkSyncRoomStateRpc roomState, string sceneId, string matchId) {
        string resolvedMatchId = string.IsNullOrWhiteSpace(matchId)
            ? (roomState == null ? string.Empty : roomState.inviteCode ?? string.Empty)
            : matchId.Trim();

        MatchIdValue.SetValue(resolvedMatchId);
        RoomInviteCodeValue.SetValue(roomState == null ? string.Empty : roomState.inviteCode ?? string.Empty);
        SceneIdValue.SetValue(string.IsNullOrWhiteSpace(sceneId) ? DEFAULT_SCENE_ID : sceneId);
        MainStateValue.SetValue(BattleMainStateType.MatchInit);
        RoundIndexValue.SetValue(0);
        RoundPhaseValue.SetValue(BattleRoundPhaseType.Battle);
        WinnerPlayerIdValue.SetValue(string.Empty);
        StateVersionValue.SetValue(1);
        StageNameValue.SetValue(DEFAULT_STAGE_NAME);
        StageEnterCountValue.SetValue(0);
        RunningTimeValue.SetValue(0f);
        SectEnvironmentValue.SetValue(null);
        FormationSnapshotValue.SetValue(null);
        BattleMatchListValue.SetValue(new BattleRoundMatchSnapshot[0]);
        ParticipantListValue.SetValue(CloneFromRoomState(roomState));
        AliveCountValue.SetValue(CountAliveParticipants(ParticipantListValue.Value));
        HasSnapshot = true;
    }

    public void ApplySnapshot(NetworkSyncBattleStateRpc snapshot) {
        if (snapshot == null) {
            return;
        }

        MatchIdValue.SetValue(snapshot.matchId ?? string.Empty);
        RoomInviteCodeValue.SetValue(snapshot.roomInviteCode ?? string.Empty);
        SceneIdValue.SetValue(string.IsNullOrWhiteSpace(snapshot.sceneId) ? DEFAULT_SCENE_ID : snapshot.sceneId);
        MainStateValue.SetValue(snapshot.mainState);
        RoundIndexValue.SetValue(snapshot.roundIndex);
        RoundPhaseValue.SetValue(snapshot.roundPhase);
        AliveCountValue.SetValue(snapshot.aliveCount);
        WinnerPlayerIdValue.SetValue(snapshot.winnerPlayerId ?? string.Empty);
        StateVersionValue.SetValue(snapshot.stateVersion);
        StageNameValue.SetValue(string.IsNullOrWhiteSpace(snapshot.stageName) ? DEFAULT_STAGE_NAME : snapshot.stageName);
        StageEnterCountValue.SetValue(snapshot.stageEnterCount);
        SectEnvironmentValue.SetValue(snapshot.sectEnvironment == null ? null : snapshot.sectEnvironment.Clone());
        FormationSnapshotValue.SetValue(snapshot.formationSnapshot == null ? null : snapshot.formationSnapshot.Clone());
        BattleMatchListValue.SetValue(CloneBattleMatchArray(snapshot.battleMatchList));
        ParticipantListValue.SetValue(CloneParticipantArray(snapshot.participants));
        HasSnapshot = true;
    }

    public NetworkSyncBattleStateRpc BuildSnapshot() {
        NetworkSyncBattleStateRpc snapshot = new NetworkSyncBattleStateRpc();
        snapshot.matchId = MatchId;
        snapshot.roomInviteCode = RoomInviteCode;
        snapshot.sceneId = SceneId;
        snapshot.mainState = MainState;
        snapshot.roundIndex = RoundIndex;
        snapshot.roundPhase = RoundPhase;
        snapshot.aliveCount = AliveCount;
        snapshot.winnerPlayerId = WinnerPlayerId;
        snapshot.stateVersion = StateVersion;
        snapshot.stageName = StageName;
        snapshot.stageEnterCount = StageEnterCount;
        snapshot.sectEnvironment = SectEnvironment == null ? null : SectEnvironment.Clone();
        snapshot.formationSnapshot = FormationSnapshot == null ? null : FormationSnapshot.Clone();
        snapshot.battleMatchList = CloneBattleMatchArray(BattleMatchList);
        snapshot.participants = CloneParticipantArray(Participants);
        return snapshot;
    }

    public void UpdateMainState(BattleMainStateType mainState) {
        MainStateValue.SetValue(mainState);
        StateVersionValue.SetValue(StateVersion + 1);
        HasSnapshot = true;
    }

    public void UpdateRoundPhase(BattleRoundPhaseType roundPhase) {
        RoundPhaseValue.SetValue(roundPhase);
        StateVersionValue.SetValue(StateVersion + 1);
        HasSnapshot = true;
    }

    public void UpdateRoundIndex(int roundIndex) {
        RoundIndexValue.SetValue(roundIndex);
        StateVersionValue.SetValue(StateVersion + 1);
        HasSnapshot = true;
    }

    public void SetWinner(string winnerPlayerId) {
        WinnerPlayerIdValue.SetValue(winnerPlayerId ?? string.Empty);
        StateVersionValue.SetValue(StateVersion + 1);
        HasSnapshot = true;
    }

    public BattleParticipantSnapshot[] CreateParticipantsClone() {
        return CloneParticipantArray(Participants);
    }

    public void ReplaceParticipants(BattleParticipantSnapshot[] participants, bool incrementVersion = true) {
        BattleParticipantSnapshot[] clonedParticipants = CloneParticipantArray(participants);
        ParticipantListValue.SetValue(clonedParticipants);
        AliveCountValue.SetValue(CountAliveParticipants(clonedParticipants));
        if (incrementVersion) {
            StateVersionValue.SetValue(StateVersion + 1);
        }

        HasSnapshot = true;
    }

    public void PrepareHeroSelectPhase() {
        BattleParticipantSnapshot[] participants = CreateParticipantsClone();
        for (int i = 0; i < participants.Length; i++) {
            if (participants[i].isEliminated) {
                continue;
            }

            participants[i].heroCandidates = BattleA2ContentCatalog.BuildHeroCandidates(
                participants[i].slotIndex,
                RoundIndex);
            participants[i].selectedCandidateIndex = -1;
            participants[i].selectedHeroId = string.Empty;
            participants[i].selectedHeroName = string.Empty;
            participants[i].selectionSource = string.Empty;
            participants[i].hasLockedHero = false;
            participants[i].hasCompletedCurrentPhase = false;
            participants[i].heroCombatBase = 0;
            participants[i].heroCombatBonus = 0;
            participants[i].battlePowerTotal = 0;
            participants[i].luckValue = BattleA2ContentCatalog.ResolveInitialLuck(participants[i].slotIndex);
            participants[i].lastBattleResultType = BattleCombatResultType.None;
            participants[i].lastBattleOpponentPlayerId = string.Empty;
            participants[i].lastBattleOpponentName = string.Empty;
            participants[i].lastBattleScore = 0;
            participants[i].lastBattleOpponentScore = 0;
            participants[i].lastQiLuckDelta = 0;
            participants[i].eliminationRound = 0;
            participants[i].finalRank = 0;
            participants[i].martialValue = 0;
            participants[i].spellValue = 0;
            participants[i].wealthValue = 0;
            participants[i].companionValue = 0;
            participants[i].territoryValue = 0;
            participants[i].physicalGrowthTotal = 0;
            participants[i].spellGrowthTotal = 0;
            participants[i].wealthGrowthTotal = 0;
            participants[i].companionGrowthTotal = 0;
            participants[i].territoryGrowthTotal = 0;
            participants[i].cultivationOptions = new BattleCultivationOptionSnapshot[0];
            participants[i].selectedCultivationOptionIndex = -1;
            participants[i].selectedCultivationOptionId = string.Empty;
            participants[i].selectedCultivationOptionName = string.Empty;
            participants[i].selectedCultivationOptionSummary = string.Empty;
            participants[i].selectedCultivationOptionSource = string.Empty;
            participants[i].selectedCultivationBattlePowerGain = 0;
            participants[i].selectedCultivationPhysicalGain = 0;
            participants[i].selectedCultivationSpellGain = 0;
            participants[i].selectedCultivationWealthGain = 0;
            participants[i].selectedCultivationCompanionGain = 0;
            participants[i].selectedCultivationTerritoryGain = 0;
            participants[i].selectedCultivationLuckGain = 0;
            participants[i].hasSelectedCultivationOption = false;
            participants[i].lastGrowthRoundIndex = -1;
            participants[i].lastGrowthOptionId = string.Empty;
            participants[i].lastGrowthOptionName = string.Empty;
            participants[i].lastGrowthOptionSummary = string.Empty;
            participants[i].lastGrowthBattlePowerGain = 0;
            participants[i].lastGrowthPhysicalGain = 0;
            participants[i].lastGrowthSpellGain = 0;
            participants[i].lastGrowthWealthGain = 0;
            participants[i].lastGrowthCompanionGain = 0;
            participants[i].lastGrowthTerritoryGain = 0;
            participants[i].lastGrowthLuckGain = 0;
            participants[i].recentBattleRoundIndex = -1;
            participants[i].recentBattleOutcome = BattleBattleOutcomeType.Pending;
            participants[i].recentBattleIsBye = false;
            participants[i].recentBattleOpponentPlayerId = string.Empty;
            participants[i].recentBattleOpponentDisplayName = string.Empty;
            participants[i].recentBattleScore = 0;
            participants[i].recentBattleLuckDelta = 0;
            participants[i].recentBattleSummary = string.Empty;
            participants[i].buildProfile = BattleBContentCatalog.BuildDefaultProfile(participants[i].slotIndex);
            participants[i].heroProfessionId = "sword";
            participants[i].professionName = "剑修";
            participants[i].heroTemplateId = string.Empty;
            participants[i].heroTemplateName = string.Empty;
            participants[i].heroTemplateSummary = string.Empty;
            participants[i].preferredPosition = participants[i].buildProfile == null
                ? BattleFormationPositionType.Middle
                : participants[i].buildProfile.preferredPosition;
            participants[i].martialBias = 0;
            participants[i].spellBias = 0;
            participants[i].wealthBias = 0;
            participants[i].companionBias = 0;
            participants[i].territoryBias = 0;
            participants[i].currentFormationPosition = participants[i].buildProfile == null
                ? BattleFormationPositionType.Middle
                : participants[i].buildProfile.preferredPosition;
            participants[i].pendingFormationPosition = participants[i].currentFormationPosition;
            participants[i].hasConfirmedFormation = false;
            participants[i].formationSelectionSource = string.Empty;
            participants[i].swordIntentValue = participants[i].buildProfile == null ? 0 : participants[i].buildProfile.swordIntent;
            participants[i].swordIntentCurrent = participants[i].swordIntentValue;
        }

        ReplaceParticipants(participants);
    }

    public int AutoSelectPendingHeroChoices(string selectionSource, bool includeHumanParticipants) {
        if (MainState != BattleMainStateType.HeroSelect) {
            return 0;
        }

        BattleParticipantSnapshot[] participants = CreateParticipantsClone();
        int changedCount = 0;
        for (int i = 0; i < participants.Length; i++) {
            BattleParticipantSnapshot participant = participants[i];
            if (participant.isEliminated || participant.hasLockedHero) {
                continue;
            }

            if (!includeHumanParticipants &&
                participant.participantType != NetworkSyncRoomParticipantType.AI) {
                continue;
            }

            int choiceCount = participant.heroCandidates == null ? 0 : participant.heroCandidates.Length;
            if (choiceCount <= 0) {
                continue;
            }

            int autoIndex = BattleA2ContentCatalog.ResolveAutoSelectionIndex(participant.slotIndex, RoundIndex, choiceCount);
            if (ApplyHeroSelection(participant, autoIndex, selectionSource, true)) {
                changedCount++;
            }
        }

        if (changedCount > 0) {
            ReplaceParticipants(participants);
        }

        return changedCount;
    }

    public bool TrySubmitHeroSelection(string playerId, int candidateIndex, string selectionSource) {
        if (MainState != BattleMainStateType.HeroSelect || string.IsNullOrWhiteSpace(playerId)) {
            return false;
        }

        BattleParticipantSnapshot[] participants = CreateParticipantsClone();
        bool changed = false;
        for (int i = 0; i < participants.Length; i++) {
            BattleParticipantSnapshot participant = participants[i];
            if (participant.isEliminated || participant.hasLockedHero) {
                continue;
            }

            if (!string.Equals(participant.playerId, playerId, StringComparison.Ordinal)) {
                continue;
            }

            changed = ApplyHeroSelection(participant, candidateIndex, selectionSource, false);
            break;
        }

        if (!changed) {
            return false;
        }

        ReplaceParticipants(participants);
        return true;
    }

    public void AssignSectEnvironment(BattleA2SectEnvironmentSnapshot environment) {
        SectEnvironmentValue.SetValue(environment == null ? null : environment.Clone());
        StateVersionValue.SetValue(StateVersion + 1);
        HasSnapshot = true;
    }

    public void PrepareBattlePhase(int roundIndex) {
        UpdateRoundIndex(roundIndex);
        UpdateRoundPhase(BattleRoundPhaseType.Battle);

        BattleParticipantSnapshot[] participants = CreateParticipantsClone();
        for (int i = 0; i < participants.Length; i++) {
            BattleParticipantSnapshot participant = participants[i];
            if (participant.isEliminated) {
                participant.cultivationOptions = new BattleCultivationOptionSnapshot[0];
                participant.selectedCultivationOptionIndex = -1;
                continue;
            }

            participant.hasCompletedCurrentPhase = false;
            participant.selectedCultivationOptionIndex = -1;
            participant.cultivationOptions = new BattleCultivationOptionSnapshot[0];
            participant.lastBattleResultType = BattleCombatResultType.None;
            participant.lastBattleOpponentPlayerId = string.Empty;
            participant.lastBattleOpponentName = string.Empty;
            participant.lastBattleScore = 0;
            participant.lastBattleOpponentScore = 0;
            participant.lastQiLuckDelta = 0;
            participant.hasConfirmedFormation = false;
            participant.pendingFormationPosition = participant.currentFormationPosition;
            participant.formationSelectionSource = string.Empty;
        }

        ReplaceParticipants(participants);
    }

    public void PrepareCultivationPhase() {
        BattleParticipantSnapshot[] participants = CreateParticipantsClone();
        for (int i = 0; i < participants.Length; i++) {
            BattleParticipantSnapshot participant = participants[i];
            if (participant.isEliminated) {
                participant.cultivationOptions = new BattleCultivationOptionSnapshot[0];
                participant.selectedCultivationOptionIndex = -1;
                continue;
            }

            participant.cultivationOptions =
                BattleA2ContentCatalog.BuildCultivationOptions(participant, RoundIndex, SectEnvironment);
            participant.selectedCultivationOptionIndex = -1;
            participant.selectedSecondaryCultivationOptionIndex = -1;
            participant.selectedSecondaryCultivationOptionId = string.Empty;
            participant.selectedSecondaryCultivationOptionName = string.Empty;
            participant.selectedSecondaryCultivationOptionSummary = string.Empty;
            participant.selectedCarrySlotLabel = string.Empty;
            participant.selectedCarryTitle = string.Empty;
            participant.selectedCultivationModuleType = BattleCultivationModuleType.None;
            participant.selectedCultivationModuleName = string.Empty;
            participant.selectedCultivationSlotId = string.Empty;
            participant.selectedCultivationSlotDisplayName = string.Empty;
            participant.selectedCultivationResolvedEntryId = string.Empty;
            participant.selectedCultivationResolvedEntryName = string.Empty;
            participant.selectedCultivationResolvedEntrySummary = string.Empty;
            participant.selectedCultivationTier = 0;
            participant.selectedCultivationSwordIntentGain = 0;
            participant.selectedRecommendedPosition = BattleFormationPositionType.None;
            participant.hasCompletedCurrentPhase = false;
            participant.hasSelectedCultivationOption = false;
        }

        ReplaceParticipants(participants);
    }

    public int AutoSelectPendingCultivationChoices(string selectionSource, bool includeHumanParticipants) {
        if (MainState != BattleMainStateType.RoundLoop || RoundPhase != BattleRoundPhaseType.Cultivation) {
            return 0;
        }

        BattleParticipantSnapshot[] participants = CreateParticipantsClone();
        int changedCount = 0;
        for (int i = 0; i < participants.Length; i++) {
            BattleParticipantSnapshot participant = participants[i];
            if (participant.isEliminated || participant.hasCompletedCurrentPhase) {
                continue;
            }

            if (!includeHumanParticipants &&
                participant.participantType != NetworkSyncRoomParticipantType.AI) {
                continue;
            }

            int optionCount = participant.cultivationOptions == null
                ? 0
                : participant.cultivationOptions.Length;
            if (optionCount <= 0) {
                continue;
            }

            int autoIndex = BattleA2ContentCatalog.ResolveAutoCultivationSelectionIndex(
                participant,
                RoundIndex,
                participant.cultivationOptions);
            if (ApplyCultivationSelection(participant, autoIndex, selectionSource, true)) {
                changedCount++;
            }
        }

        if (changedCount > 0) {
            ReplaceParticipants(participants);
        }

        return changedCount;
    }

    public bool TrySubmitCultivationSelection(string playerId, int optionIndex, string selectionSource) {
        if (MainState != BattleMainStateType.RoundLoop ||
            RoundPhase != BattleRoundPhaseType.Cultivation ||
            string.IsNullOrWhiteSpace(playerId)) {
            return false;
        }

        BattleParticipantSnapshot[] participants = CreateParticipantsClone();
        bool changed = false;
        for (int i = 0; i < participants.Length; i++) {
            BattleParticipantSnapshot participant = participants[i];
            if (participant.isEliminated || participant.hasCompletedCurrentPhase) {
                continue;
            }

            if (!string.Equals(participant.playerId, playerId, StringComparison.Ordinal)) {
                continue;
            }

            changed = ApplyCultivationSelection(participant, optionIndex, selectionSource, false);
            break;
        }

        if (!changed) {
            return false;
        }

        ReplaceParticipants(participants);
        return true;
    }

    public bool AreAllActiveParticipantsLocked() {
        return AreAllActiveParticipantsCompletedByPredicate(participant => participant.hasLockedHero);
    }

    public bool AreAllActiveParticipantsCompletedCurrentPhase() {
        return AreAllActiveParticipantsCompletedByPredicate(participant => participant.hasCompletedCurrentPhase);
    }

    public int CountPendingHeroSelections() {
        return CountPendingParticipants(participant => !participant.hasLockedHero);
    }

    public int CountPendingCultivationSelections() {
        return CountPendingParticipants(participant => !participant.hasCompletedCurrentPhase);
    }

    public int CountAliveParticipants() {
        return CountAliveParticipants(Participants);
    }

    public string ResolveWinnerFromAliveParticipants() {
        BattleParticipantSnapshot[] participants = Participants;
        BattleParticipantSnapshot bestParticipant = null;
        for (int i = 0; i < participants.Length; i++) {
            BattleParticipantSnapshot participant = participants[i];
            if (participant == null || participant.isEliminated) {
                continue;
            }

            if (bestParticipant == null) {
                bestParticipant = participant;
                continue;
            }

            int currentPower = participant.heroCombatBase + participant.heroCombatBonus + participant.battlePowerTotal + participant.luckValue;
            int bestPower = bestParticipant.heroCombatBase + bestParticipant.heroCombatBonus + bestParticipant.battlePowerTotal + bestParticipant.luckValue;
            if (currentPower > bestPower) {
                bestParticipant = participant;
            }
        }

        return bestParticipant == null ? string.Empty : bestParticipant.playerId ?? string.Empty;
    }

    public void MarkAllActiveParticipantsCompletedCurrentPhase() {
        BattleParticipantSnapshot[] participants = CreateParticipantsClone();
        for (int i = 0; i < participants.Length; i++) {
            if (participants[i] == null || participants[i].isEliminated) {
                continue;
            }

            participants[i].hasCompletedCurrentPhase = true;
        }

        ReplaceParticipants(participants);
    }

    public void ResetCurrentPhaseCompletion() {
        BattleParticipantSnapshot[] participants = CreateParticipantsClone();
        for (int i = 0; i < participants.Length; i++) {
            if (participants[i] == null || participants[i].isEliminated) {
                continue;
            }

            participants[i].hasCompletedCurrentPhase = false;
        }

        ReplaceParticipants(participants);
    }

    public bool AreAllActiveParticipantsCultivationSelected() {
        return AreAllActiveParticipantsCompletedByPredicate(participant => participant.hasSelectedCultivationOption);
    }

    public void PrepareBattleRound() {
        BattleMatchListValue.SetValue(new BattleRoundMatchSnapshot[0]);
        AssignFormationSnapshot(BattleBContentCatalog.BuildFormationSnapshot(RoundIndex, SectEnvironment), false);

        BattleParticipantSnapshot[] participants = CreateParticipantsClone();
        for (int i = 0; i < participants.Length; i++) {
            BattleParticipantSnapshot participant = participants[i];
            if (participant == null) {
                continue;
            }

            participant.hasCompletedCurrentPhase = false;
            participant.hasSelectedCultivationOption = false;
            participant.cultivationOptions = new BattleCultivationOptionSnapshot[0];
            participant.selectedCultivationOptionIndex = -1;
            participant.selectedCultivationOptionId = string.Empty;
            participant.selectedCultivationOptionName = string.Empty;
            participant.selectedCultivationOptionSummary = string.Empty;
            participant.selectedCultivationOptionSource = string.Empty;
            participant.selectedCultivationBattlePowerGain = 0;
            participant.selectedCultivationPhysicalGain = 0;
            participant.selectedCultivationSpellGain = 0;
            participant.selectedCultivationWealthGain = 0;
            participant.selectedCultivationCompanionGain = 0;
            participant.selectedCultivationTerritoryGain = 0;
            participant.selectedCultivationLuckGain = 0;
            participant.selectedCultivationSwordIntentGain = 0;
            participant.selectedCultivationModuleType = BattleCultivationModuleType.None;
            participant.selectedCultivationModuleName = string.Empty;
            participant.selectedCultivationSlotId = string.Empty;
            participant.selectedCultivationSlotDisplayName = string.Empty;
            participant.selectedCultivationResolvedEntryId = string.Empty;
            participant.selectedCultivationResolvedEntryName = string.Empty;
            participant.selectedCultivationResolvedEntrySummary = string.Empty;
            participant.selectedCultivationTier = 0;
            participant.selectedSecondaryCultivationOptionIndex = -1;
            participant.selectedSecondaryCultivationOptionId = string.Empty;
            participant.selectedSecondaryCultivationOptionName = string.Empty;
            participant.selectedSecondaryCultivationOptionSummary = string.Empty;
            participant.selectedCarrySlotLabel = string.Empty;
            participant.selectedCarryTitle = string.Empty;
            participant.selectedRecommendedPosition = BattleFormationPositionType.None;
            participant.recentBattleRoundIndex = -1;
            participant.recentBattleOutcome = BattleBattleOutcomeType.Pending;
            participant.recentBattleIsBye = false;
            participant.recentBattleOpponentPlayerId = string.Empty;
            participant.recentBattleOpponentDisplayName = string.Empty;
            participant.recentBattleScore = 0;
            participant.recentBattleLuckDelta = 0;
            participant.recentBattleSummary = string.Empty;
            participant.lastBattleResultType = BattleCombatResultType.None;
            participant.lastBattleOpponentPlayerId = string.Empty;
            participant.lastBattleOpponentName = string.Empty;
            participant.lastBattleScore = 0;
            participant.lastBattleOpponentScore = 0;
            participant.lastQiLuckDelta = 0;
            participant.hasConfirmedFormation = false;
            participant.pendingFormationPosition = participant.currentFormationPosition;
            participant.formationSelectionSource = string.Empty;
        }

        ReplaceParticipants(participants);
    }

    public bool ApplyBattleResults() {
        BattleParticipantSnapshot[] participants = CreateParticipantsClone();
        BattleRoundMatchSnapshot[] matches = BattleA2ContentCatalog.BuildBattleMatches(
            participants,
            RoundIndex,
            SectEnvironment,
            FormationSnapshot);

        for (int i = 0; i < participants.Length; i++) {
            BattleParticipantSnapshot participant = participants[i];
            if (participant == null) {
                continue;
            }

            participant.recentBattleRoundIndex = -1;
            participant.recentBattleOutcome = BattleBattleOutcomeType.Pending;
            participant.recentBattleIsBye = false;
            participant.recentBattleOpponentPlayerId = string.Empty;
            participant.recentBattleOpponentDisplayName = string.Empty;
            participant.recentBattleScore = 0;
            participant.recentBattleLuckDelta = 0;
            participant.recentBattleSummary = string.Empty;
            participant.lastBattleResultType = BattleCombatResultType.None;
            participant.lastBattleOpponentPlayerId = string.Empty;
            participant.lastBattleOpponentName = string.Empty;
            participant.lastBattleScore = 0;
            participant.lastBattleOpponentScore = 0;
            participant.lastQiLuckDelta = 0;
        }

        for (int matchIndex = 0; matchIndex < matches.Length; matchIndex++) {
            BattleRoundMatchSnapshot match = matches[matchIndex];
            if (match == null) {
                continue;
            }

            ApplyMatchResultToParticipant(participants, match.leftPlayerId, match.leftDisplayName, match.rightPlayerId, match.rightDisplayName, match.leftOutcome, match.leftScore, match.rightScore, match.leftLuckDelta, match.summary, match.isBye, match.roundIndex);
            if (!match.isBye) {
                ApplyMatchResultToParticipant(participants, match.rightPlayerId, match.rightDisplayName, match.leftPlayerId, match.leftDisplayName, match.rightOutcome, match.rightScore, match.leftScore, match.rightLuckDelta, match.summary, false, match.roundIndex);
            }
        }

        ReplaceParticipants(participants);
        BattleMatchListValue.SetValue(CloneBattleMatchArray(matches));
        StateVersionValue.SetValue(StateVersion + 1);

        bool hasWinner = FinalizeEliminationAndWinner();
        return hasWinner;
    }

    public void CommitCultivationGrowth() {
        BattleParticipantSnapshot[] participants = CreateParticipantsClone();
        bool changed = false;
        for (int i = 0; i < participants.Length; i++) {
            BattleParticipantSnapshot participant = participants[i];
            if (participant == null || participant.isEliminated || !participant.hasSelectedCultivationOption) {
                continue;
            }

            participant.battlePowerTotal += participant.selectedCultivationBattlePowerGain;
            participant.heroCombatBonus += participant.selectedCultivationBattlePowerGain;
            participant.luckValue = ClampInt(participant.luckValue + participant.selectedCultivationLuckGain, 0, participant.qiLuckMax);
            participant.qiLuckCurrent = participant.luckValue;
            participant.physicalGrowthTotal += participant.selectedCultivationPhysicalGain;
            participant.spellGrowthTotal += participant.selectedCultivationSpellGain;
            participant.wealthGrowthTotal += participant.selectedCultivationWealthGain;
            participant.companionGrowthTotal += participant.selectedCultivationCompanionGain;
            participant.territoryGrowthTotal += participant.selectedCultivationTerritoryGain;
            participant.martialValue += participant.selectedCultivationPhysicalGain;
            participant.spellValue += participant.selectedCultivationSpellGain;
            participant.wealthValue += participant.selectedCultivationWealthGain;
            participant.companionValue += participant.selectedCultivationCompanionGain;
            participant.territoryValue += participant.selectedCultivationTerritoryGain;
            participant.lastGrowthRoundIndex = RoundIndex;
            participant.lastGrowthOptionId = participant.selectedCultivationOptionId;
            participant.lastGrowthOptionName = participant.selectedCultivationOptionName;
            participant.lastGrowthOptionSummary = participant.selectedCultivationOptionSummary;
            participant.lastGrowthBattlePowerGain = participant.selectedCultivationBattlePowerGain;
            participant.lastGrowthPhysicalGain = participant.selectedCultivationPhysicalGain;
            participant.lastGrowthSpellGain = participant.selectedCultivationSpellGain;
            participant.lastGrowthWealthGain = participant.selectedCultivationWealthGain;
            participant.lastGrowthCompanionGain = participant.selectedCultivationCompanionGain;
            participant.lastGrowthTerritoryGain = participant.selectedCultivationTerritoryGain;
            participant.lastGrowthLuckGain = participant.selectedCultivationLuckGain;
            if (participant.buildProfile != null) {
                participant.buildProfile.swordIntent = ClampInt(
                    participant.buildProfile.swordIntent + participant.selectedCultivationSwordIntentGain,
                    0,
                    participant.buildProfile.swordIntentMax);
                participant.swordIntentValue = participant.buildProfile.swordIntent;
                participant.swordIntentCurrent = participant.buildProfile.swordIntent;

                if (!string.IsNullOrWhiteSpace(participant.selectedCultivationSlotId)) {
                    BattleBuildSlotSnapshot slot = new BattleBuildSlotSnapshot {
                        slotId = participant.selectedCultivationSlotId,
                        slotDisplayName = participant.selectedCultivationSlotDisplayName,
                        moduleType = participant.selectedCultivationModuleType,
                        entryId = participant.selectedCultivationResolvedEntryId,
                        entryDisplayName = participant.selectedCultivationResolvedEntryName,
                        summary = participant.selectedCultivationResolvedEntrySummary,
                        tier = participant.selectedCultivationTier,
                        battlePowerBonus = participant.selectedCultivationBattlePowerGain
                    };
                    participant.buildProfile.UpsertSlot(slot);
                }
            }
            changed = true;
        }

        if (changed) {
            ReplaceParticipants(participants);
        }
    }

    private bool AreAllActiveParticipantsCompletedByPredicate(Func<BattleParticipantSnapshot, bool> predicate) {
        BattleParticipantSnapshot[] participants = Participants;
        if (participants == null || participants.Length == 0) {
            return true;
        }

        for (int i = 0; i < participants.Length; i++) {
            BattleParticipantSnapshot participant = participants[i];
            if (participant == null || participant.isEliminated) {
                continue;
            }

            if (!predicate(participant)) {
                return false;
            }
        }

        return true;
    }

    private int CountPendingParticipants(Func<BattleParticipantSnapshot, bool> isPending) {
        BattleParticipantSnapshot[] participants = Participants;
        if (participants == null || participants.Length == 0) {
            return 0;
        }

        int pendingCount = 0;
        for (int i = 0; i < participants.Length; i++) {
            BattleParticipantSnapshot participant = participants[i];
            if (participant == null || participant.isEliminated) {
                continue;
            }

            if (isPending(participant)) {
                pendingCount++;
            }
        }

        return pendingCount;
    }

    private static BattleParticipantSnapshot[] CloneFromRoomState(NetworkSyncRoomStateRpc roomState) {
        if (roomState == null || roomState.slots == null || roomState.slots.Length == 0) {
            return new BattleParticipantSnapshot[0];
        }

        BattleParticipantSnapshot[] participants = new BattleParticipantSnapshot[roomState.slots.Length];
        for (int i = 0; i < roomState.slots.Length; i++) {
            participants[i] = BuildParticipantSnapshot(roomState.slots[i]);
        }

        return participants;
    }

    private static BattleParticipantSnapshot[] CloneParticipantArray(BattleParticipantSnapshot[] source) {
        if (source == null || source.Length == 0) {
            return new BattleParticipantSnapshot[0];
        }

        BattleParticipantSnapshot[] participants = new BattleParticipantSnapshot[source.Length];
        for (int i = 0; i < source.Length; i++) {
            participants[i] = source[i] == null ? new BattleParticipantSnapshot() : source[i].Clone();
        }

        return participants;
    }

    private static BattleParticipantSnapshot BuildParticipantSnapshot(NetworkSyncRoomSlotData slot) {
        BattleParticipantSnapshot participant = new BattleParticipantSnapshot();
        if (slot == null) {
            participant.qiLuckCurrent = DEFAULT_QI_LUCK;
            participant.qiLuckMax = DEFAULT_QI_LUCK;
            participant.luckValue = DEFAULT_QI_LUCK;
            participant.cultivationOptions = new BattleCultivationOptionSnapshot[0];
            return participant;
        }

        participant.slotIndex = slot.slotIndex;
        participant.playerId = slot.playerId ?? string.Empty;
        participant.displayName = slot.displayName ?? string.Empty;
        participant.avatarId = slot.avatarId ?? string.Empty;
        participant.participantType = slot.participantType;
        participant.isHost = slot.isHost;
        participant.isEliminated = false;
        participant.heroCandidates = new BattleA2HeroCandidateSnapshot[0];
        participant.selectedCandidateIndex = -1;
        participant.selectedHeroId = string.Empty;
        participant.selectedHeroName = string.Empty;
        participant.selectionSource = string.Empty;
        participant.hasLockedHero = false;
        participant.hasCompletedCurrentPhase = false;
        participant.qiLuckCurrent = BattleA2ContentCatalog.ResolveInitialLuck(slot.slotIndex);
        participant.qiLuckMax = participant.qiLuckCurrent;
        participant.luckValue = participant.qiLuckCurrent;
        participant.heroCombatBase = 0;
        participant.heroCombatBonus = 0;
        participant.battlePowerTotal = 0;
        participant.lastBattleResultType = BattleCombatResultType.None;
        participant.lastBattleOpponentPlayerId = string.Empty;
        participant.lastBattleOpponentName = string.Empty;
        participant.lastBattleScore = 0;
        participant.lastBattleOpponentScore = 0;
        participant.lastQiLuckDelta = 0;
        participant.eliminationRound = 0;
        participant.finalRank = 0;
        participant.martialValue = 0;
        participant.spellValue = 0;
        participant.wealthValue = 0;
        participant.companionValue = 0;
        participant.territoryValue = 0;
        participant.physicalGrowthTotal = 0;
        participant.spellGrowthTotal = 0;
        participant.wealthGrowthTotal = 0;
        participant.companionGrowthTotal = 0;
        participant.territoryGrowthTotal = 0;
        participant.cultivationOptions = new BattleCultivationOptionSnapshot[0];
        participant.selectedCultivationOptionIndex = -1;
        participant.selectedCultivationOptionId = string.Empty;
        participant.selectedCultivationOptionName = string.Empty;
        participant.selectedCultivationOptionSummary = string.Empty;
        participant.selectedCultivationOptionSource = string.Empty;
        participant.selectedCultivationBattlePowerGain = 0;
        participant.selectedCultivationPhysicalGain = 0;
        participant.selectedCultivationSpellGain = 0;
        participant.selectedCultivationWealthGain = 0;
        participant.selectedCultivationCompanionGain = 0;
        participant.selectedCultivationTerritoryGain = 0;
        participant.selectedCultivationLuckGain = 0;
        participant.hasSelectedCultivationOption = false;
        participant.lastGrowthRoundIndex = -1;
        participant.lastGrowthOptionId = string.Empty;
        participant.lastGrowthOptionName = string.Empty;
        participant.lastGrowthOptionSummary = string.Empty;
        participant.lastGrowthBattlePowerGain = 0;
        participant.lastGrowthPhysicalGain = 0;
        participant.lastGrowthSpellGain = 0;
        participant.lastGrowthWealthGain = 0;
        participant.lastGrowthCompanionGain = 0;
        participant.lastGrowthTerritoryGain = 0;
        participant.lastGrowthLuckGain = 0;
        participant.recentBattleRoundIndex = -1;
        participant.recentBattleOutcome = BattleBattleOutcomeType.Pending;
        participant.recentBattleIsBye = false;
        participant.recentBattleOpponentPlayerId = string.Empty;
        participant.recentBattleOpponentDisplayName = string.Empty;
        participant.recentBattleScore = 0;
        participant.recentBattleLuckDelta = 0;
        participant.recentBattleSummary = string.Empty;
        return participant;
    }

    private static bool ApplyHeroSelection(
        BattleParticipantSnapshot participant,
        int candidateIndex,
        string selectionSource,
        bool allowFallbackIndex) {
        if (participant == null || participant.isEliminated || participant.hasLockedHero) {
            return false;
        }

        BattleA2HeroCandidateSnapshot[] candidates = participant.heroCandidates;
        if (candidates == null || candidates.Length == 0) {
            return false;
        }

        int resolvedIndex = candidateIndex;
        if (allowFallbackIndex && (resolvedIndex < 0 || resolvedIndex >= candidates.Length)) {
            resolvedIndex = BattleA2ContentCatalog.ResolveAutoSelectionIndex(participant.slotIndex, 0, candidates.Length);
        }

        if (resolvedIndex < 0 || resolvedIndex >= candidates.Length) {
            return false;
        }

        BattleA2HeroCandidateSnapshot selectedCandidate = candidates[resolvedIndex];
        if (selectedCandidate == null) {
            return false;
        }

        participant.selectedCandidateIndex = resolvedIndex;
        participant.selectedHeroId = selectedCandidate.heroId ?? string.Empty;
        participant.selectedHeroName = selectedCandidate.displayName ?? string.Empty;
        participant.selectionSource = string.IsNullOrWhiteSpace(selectionSource) ? string.Empty : selectionSource.Trim();
        participant.hasLockedHero = true;
        participant.hasCompletedCurrentPhase = true;
        participant.heroCombatBase = selectedCandidate.powerScore;
        participant.heroCombatBonus = 0;
        participant.battlePowerTotal = 0;
        participant.buildProfile = BattleBContentCatalog.BuildProfileFromHero(selectedCandidate);
        participant.currentFormationPosition = selectedCandidate.preferredPosition;
        participant.pendingFormationPosition = selectedCandidate.preferredPosition;
        participant.preferredPosition = selectedCandidate.preferredPosition;
        participant.heroProfessionId = selectedCandidate.professionId ?? "sword";
        participant.professionName = selectedCandidate.professionName ?? "剑修";
        participant.heroTemplateId = selectedCandidate.templateId ?? string.Empty;
        participant.heroTemplateName = selectedCandidate.templateName ?? string.Empty;
        participant.heroTemplateSummary = selectedCandidate.templateSummary ?? string.Empty;
        participant.martialBias = selectedCandidate.martialBias;
        participant.spellBias = selectedCandidate.spellBias;
        participant.wealthBias = selectedCandidate.wealthBias;
        participant.companionBias = selectedCandidate.companionBias;
        participant.territoryBias = selectedCandidate.territoryBias;
        participant.swordIntentValue = selectedCandidate.swordIntentStart;
        participant.swordIntentCurrent = selectedCandidate.swordIntentStart;
        return true;
    }

    private static bool ApplyCultivationSelection(
        BattleParticipantSnapshot participant,
        int optionIndex,
        string selectionSource,
        bool allowFallbackIndex) {
        if (participant == null || participant.isEliminated || participant.hasCompletedCurrentPhase) {
            return false;
        }

        BattleCultivationOptionSnapshot[] options = participant.cultivationOptions;
        if (options == null || options.Length == 0) {
            return false;
        }

        int resolvedIndex = optionIndex;
        if (allowFallbackIndex && (resolvedIndex < 0 || resolvedIndex >= options.Length)) {
            resolvedIndex = BattleA2ContentCatalog.ResolveAutoCultivationSelectionIndex(participant, 0, options);
        }

        if (resolvedIndex < 0 || resolvedIndex >= options.Length) {
            return false;
        }

        BattleCultivationOptionSnapshot selectedOption = options[resolvedIndex];
        if (selectedOption == null) {
            return false;
        }

        participant.selectedCultivationOptionIndex = resolvedIndex;
        participant.selectedCultivationOptionId = selectedOption.optionId ?? string.Empty;
        participant.selectedCultivationOptionName = selectedOption.displayName ?? string.Empty;
        participant.selectedCultivationOptionSummary = selectedOption.description ?? string.Empty;
        participant.selectedCultivationOptionSource = string.IsNullOrWhiteSpace(selectionSource) ? string.Empty : selectionSource.Trim();
        participant.selectedCultivationBattlePowerGain = selectedOption.battlePowerBonus;
        participant.selectedCultivationPhysicalGain = selectedOption.physicalGain;
        participant.selectedCultivationSpellGain = selectedOption.spellGain;
        participant.selectedCultivationWealthGain = selectedOption.wealthGain;
        participant.selectedCultivationCompanionGain = selectedOption.companionGain;
        participant.selectedCultivationTerritoryGain = selectedOption.territoryGain;
        participant.selectedCultivationLuckGain = selectedOption.luckGain;
        participant.selectedCultivationSwordIntentGain = selectedOption.swordIntentGain;
        participant.selectedCultivationModuleType = selectedOption.moduleType;
        participant.selectedCultivationModuleName = BattleBContentCatalog.ResolveModuleLabel(selectedOption.moduleType);
        participant.selectedCultivationSlotId = selectedOption.slotType ?? string.Empty;
        participant.selectedCultivationSlotDisplayName = selectedOption.carrySlotLabel ?? string.Empty;
        participant.selectedCultivationTier = 1;
        participant.selectedRecommendedPosition = selectedOption.recommendedPosition;
        BattleCultivationSubOptionSnapshot[] subOptions = selectedOption.secondaryOptions;
        int subIndex = BattleA2ContentCatalog.ResolveAutoSecondarySelectionIndex(participant, 0, selectedOption);
        if (subOptions != null && subOptions.Length > 0 && subIndex >= 0 && subIndex < subOptions.Length) {
            BattleCultivationSubOptionSnapshot subOption = subOptions[subIndex];
            participant.selectedSecondaryCultivationOptionIndex = subIndex;
            participant.selectedSecondaryCultivationOptionId = subOption.subOptionId ?? string.Empty;
            participant.selectedSecondaryCultivationOptionName = subOption.displayName ?? string.Empty;
            participant.selectedSecondaryCultivationOptionSummary = subOption.description ?? string.Empty;
            participant.selectedCarrySlotLabel = subOption.carrySlotLabel ?? string.Empty;
            participant.selectedCarryTitle = subOption.carryTitle ?? string.Empty;
            participant.selectedCultivationResolvedEntryId = subOption.subOptionId ?? string.Empty;
            participant.selectedCultivationResolvedEntryName = subOption.carryTitle ?? string.Empty;
            participant.selectedCultivationResolvedEntrySummary = subOption.description ?? string.Empty;
            participant.selectedCultivationSwordIntentGain += subOption.swordIntentGain;
            participant.selectedCultivationBattlePowerGain += subOption.battlePowerBonus;
            participant.selectedCultivationPhysicalGain += subOption.physicalGain;
            participant.selectedCultivationSpellGain += subOption.spellGain;
            participant.selectedCultivationWealthGain += subOption.wealthGain;
            participant.selectedCultivationCompanionGain += subOption.companionGain;
            participant.selectedCultivationTerritoryGain += subOption.territoryGain;
            participant.selectedCultivationLuckGain += subOption.luckGain;
            participant.selectedRecommendedPosition = subOption.recommendedPosition;
        } else {
            participant.selectedSecondaryCultivationOptionIndex = -1;
            participant.selectedSecondaryCultivationOptionId = string.Empty;
            participant.selectedSecondaryCultivationOptionName = string.Empty;
            participant.selectedSecondaryCultivationOptionSummary = string.Empty;
            participant.selectedCarrySlotLabel = selectedOption.carrySlotLabel ?? string.Empty;
            participant.selectedCarryTitle = selectedOption.displayName ?? string.Empty;
            participant.selectedCultivationResolvedEntryId = selectedOption.optionId ?? string.Empty;
            participant.selectedCultivationResolvedEntryName = selectedOption.displayName ?? string.Empty;
            participant.selectedCultivationResolvedEntrySummary = selectedOption.description ?? string.Empty;
        }
        participant.hasSelectedCultivationOption = true;
        participant.hasCompletedCurrentPhase = true;
        return true;
    }

    private static int CountAliveParticipants(BattleParticipantSnapshot[] participants) {
        if (participants == null || participants.Length == 0) {
            return 0;
        }

        int aliveCount = 0;
        for (int i = 0; i < participants.Length; i++) {
            if (participants[i] != null && !participants[i].isEliminated) {
                aliveCount++;
            }
        }

        return aliveCount;
    }

    private static BattleRoundMatchSnapshot[] CloneBattleMatchArray(BattleRoundMatchSnapshot[] source) {
        if (source == null || source.Length == 0) {
            return new BattleRoundMatchSnapshot[0];
        }

        BattleRoundMatchSnapshot[] matches = new BattleRoundMatchSnapshot[source.Length];
        for (int i = 0; i < source.Length; i++) {
            matches[i] = source[i] == null ? new BattleRoundMatchSnapshot() : source[i].Clone();
        }

        return matches;
    }

    private static void ApplyMatchResultToParticipant(
        BattleParticipantSnapshot[] participants,
        string playerId,
        string displayName,
        string opponentPlayerId,
        string opponentDisplayName,
        BattleBattleOutcomeType outcome,
        int selfScore,
        int opponentScore,
        int luckDelta,
        string summary,
        bool isBye,
        int roundIndex) {
        if (participants == null || string.IsNullOrWhiteSpace(playerId)) {
            return;
        }

        for (int i = 0; i < participants.Length; i++) {
            BattleParticipantSnapshot participant = participants[i];
            if (participant == null || !string.Equals(participant.playerId, playerId, StringComparison.Ordinal)) {
                continue;
            }

            participant.recentBattleRoundIndex = roundIndex;
            participant.recentBattleOutcome = outcome;
            participant.recentBattleIsBye = isBye;
            participant.recentBattleOpponentPlayerId = opponentPlayerId ?? string.Empty;
            participant.recentBattleOpponentDisplayName = opponentDisplayName ?? string.Empty;
            participant.recentBattleScore = selfScore;
            participant.recentBattleLuckDelta = luckDelta;
            participant.recentBattleSummary = summary ?? string.Empty;
            participant.lastBattleOpponentPlayerId = opponentPlayerId ?? string.Empty;
            participant.lastBattleOpponentName = opponentDisplayName ?? string.Empty;
            participant.lastBattleScore = selfScore;
            participant.lastBattleOpponentScore = opponentScore;
            participant.lastQiLuckDelta = luckDelta;
            participant.lastBattleResultType = ConvertBattleOutcome(outcome);
            participant.luckValue = ClampInt(participant.luckValue + luckDelta, 0, participant.qiLuckMax);
            participant.qiLuckCurrent = participant.luckValue;
            break;
        }
    }

    private bool FinalizeEliminationAndWinner() {
        BattleParticipantSnapshot[] participants = CreateParticipantsClone();
        int aliveCount = 0;
        for (int i = 0; i < participants.Length; i++) {
            BattleParticipantSnapshot participant = participants[i];
            if (participant == null || participant.isEliminated) {
                continue;
            }

            if (participant.luckValue <= 0) {
                participant.isEliminated = true;
                participant.eliminationRound = RoundIndex;
                continue;
            }

            aliveCount++;
        }

        if (aliveCount <= 0 && participants.Length > 0) {
            BattleParticipantSnapshot fallbackWinner = ResolveFallbackWinner(participants);
            if (fallbackWinner != null) {
                fallbackWinner.isEliminated = false;
                fallbackWinner.luckValue = 1;
                fallbackWinner.qiLuckCurrent = 1;
                aliveCount = 1;
            }
        }

        ReplaceParticipants(participants);
        if (aliveCount <= 1) {
            string winnerId = ResolveWinnerFromAliveParticipants();
            SetWinner(winnerId);
            UpdateMainState(BattleMainStateType.GameOver);
            return true;
        }

        return false;
    }

    private static BattleParticipantSnapshot ResolveFallbackWinner(BattleParticipantSnapshot[] participants) {
        BattleParticipantSnapshot bestParticipant = null;
        for (int i = 0; i < participants.Length; i++) {
            BattleParticipantSnapshot participant = participants[i];
            if (participant == null) {
                continue;
            }

            if (bestParticipant == null || participant.recentBattleScore > bestParticipant.recentBattleScore) {
                bestParticipant = participant;
            }
        }

        return bestParticipant;
    }

    private static BattleCombatResultType ConvertBattleOutcome(BattleBattleOutcomeType outcome) {
        switch (outcome) {
            case BattleBattleOutcomeType.Win:
                return BattleCombatResultType.Win;
            case BattleBattleOutcomeType.Lose:
                return BattleCombatResultType.Lose;
            case BattleBattleOutcomeType.Draw:
                return BattleCombatResultType.Draw;
            case BattleBattleOutcomeType.Bye:
                return BattleCombatResultType.Bye;
            default:
                return BattleCombatResultType.None;
        }
    }

    private static BattleGrowthLineType ResolveGrowthLine(BattleCultivationOptionSnapshot option) {
        if (option == null) {
            return BattleGrowthLineType.None;
        }

        int bestValue = 0;
        BattleGrowthLineType bestLine = BattleGrowthLineType.None;
        if (option.physicalGain > bestValue) {
            bestValue = option.physicalGain;
            bestLine = BattleGrowthLineType.Martial;
        }

        if (option.spellGain > bestValue) {
            bestValue = option.spellGain;
            bestLine = BattleGrowthLineType.Spell;
        }

        if (option.wealthGain > bestValue) {
            bestValue = option.wealthGain;
            bestLine = BattleGrowthLineType.Wealth;
        }

        if (option.companionGain > bestValue) {
            bestValue = option.companionGain;
            bestLine = BattleGrowthLineType.Companion;
        }

        if (option.territoryGain > bestValue) {
            bestLine = BattleGrowthLineType.Territory;
        }

        return bestLine;
    }

    private static int ClampInt(int value, int minValue, int maxValue) {
        if (value < minValue) {
            return minValue;
        }

        if (value > maxValue) {
            return maxValue;
        }

        return value;
    }
}

public enum BattleMainStateType {
    MatchInit = 0,
    HeroSelect = 1,
    SectReveal = 2,
    RoundLoop = 3,
    GameOver = 4
}

public enum BattleRoundPhaseType {
    Battle = 0,
    BattleResult = 1,
    Cultivation = 2,
    FormationConfirm = 3
}

public enum BattleCombatResultType {
    None = 0,
    Win = 1,
    Lose = 2,
    Draw = 3,
    Bye = 4
}

public enum BattleGrowthLineType {
    None = 0,
    Martial = 1,
    Spell = 2,
    Wealth = 3,
    Companion = 4,
    Territory = 5
}

[Serializable]
public sealed partial class BattleParticipantSnapshot {
    public int slotIndex;
    public string playerId;
    public string displayName;
    public string avatarId;
    public NetworkSyncRoomParticipantType participantType;
    public bool isHost;
    public bool isEliminated;
    public BattleA2HeroCandidateSnapshot[] heroCandidates;
    public int selectedCandidateIndex;
    public string selectedHeroId;
    public string selectedHeroName;
    public string selectionSource;
    public bool hasLockedHero;
    public bool hasCompletedCurrentPhase;
    public int qiLuckCurrent;
    public int qiLuckMax;
    public int luckValue;
    public int heroCombatBase;
    public int heroCombatBonus;
    public int battlePowerTotal;
    public int recentBattleRoundIndex;
    public BattleBattleOutcomeType recentBattleOutcome;
    public bool recentBattleIsBye;
    public string recentBattleOpponentPlayerId;
    public string recentBattleOpponentDisplayName;
    public int recentBattleScore;
    public int recentBattleLuckDelta;
    public string recentBattleSummary;
    public BattleCombatResultType lastBattleResultType;
    public string lastBattleOpponentPlayerId;
    public string lastBattleOpponentName;
    public int lastBattleScore;
    public int lastBattleOpponentScore;
    public int lastQiLuckDelta;
    public int eliminationRound;
    public int finalRank;
    public int martialValue;
    public int spellValue;
    public int wealthValue;
    public int companionValue;
    public int territoryValue;
    public int physicalGrowthTotal;
    public int spellGrowthTotal;
    public int wealthGrowthTotal;
    public int companionGrowthTotal;
    public int territoryGrowthTotal;
    public BattleCultivationOptionSnapshot[] cultivationOptions;
    public int selectedCultivationOptionIndex;
    public string selectedCultivationOptionId;
    public string selectedCultivationOptionName;
    public string selectedCultivationOptionSummary;
    public string selectedCultivationOptionSource;
    public int selectedCultivationBattlePowerGain;
    public int selectedCultivationPhysicalGain;
    public int selectedCultivationSpellGain;
    public int selectedCultivationWealthGain;
    public int selectedCultivationCompanionGain;
    public int selectedCultivationTerritoryGain;
    public int selectedCultivationLuckGain;
    public bool hasSelectedCultivationOption;
    public int lastGrowthRoundIndex;
    public string lastGrowthOptionId;
    public string lastGrowthOptionName;
    public string lastGrowthOptionSummary;
    public int lastGrowthBattlePowerGain;
    public int lastGrowthPhysicalGain;
    public int lastGrowthSpellGain;
    public int lastGrowthWealthGain;
    public int lastGrowthCompanionGain;
    public int lastGrowthTerritoryGain;
    public int lastGrowthLuckGain;

    public BattleParticipantSnapshot Clone() {
        return new BattleParticipantSnapshot {
            slotIndex = slotIndex,
            playerId = playerId,
            displayName = displayName,
            avatarId = avatarId,
            participantType = participantType,
            isHost = isHost,
            isEliminated = isEliminated,
            heroCandidates = CloneCandidates(heroCandidates),
            selectedCandidateIndex = selectedCandidateIndex,
            selectedHeroId = selectedHeroId,
            selectedHeroName = selectedHeroName,
            selectionSource = selectionSource,
            hasLockedHero = hasLockedHero,
            hasCompletedCurrentPhase = hasCompletedCurrentPhase,
            qiLuckCurrent = qiLuckCurrent,
            qiLuckMax = qiLuckMax,
            luckValue = luckValue,
            heroCombatBase = heroCombatBase,
            heroCombatBonus = heroCombatBonus,
            battlePowerTotal = battlePowerTotal,
            recentBattleRoundIndex = recentBattleRoundIndex,
            recentBattleOutcome = recentBattleOutcome,
            recentBattleIsBye = recentBattleIsBye,
            recentBattleOpponentPlayerId = recentBattleOpponentPlayerId,
            recentBattleOpponentDisplayName = recentBattleOpponentDisplayName,
            recentBattleScore = recentBattleScore,
            recentBattleLuckDelta = recentBattleLuckDelta,
            recentBattleSummary = recentBattleSummary,
            lastBattleResultType = lastBattleResultType,
            lastBattleOpponentPlayerId = lastBattleOpponentPlayerId,
            lastBattleOpponentName = lastBattleOpponentName,
            lastBattleScore = lastBattleScore,
            lastBattleOpponentScore = lastBattleOpponentScore,
            lastQiLuckDelta = lastQiLuckDelta,
            eliminationRound = eliminationRound,
            finalRank = finalRank,
            martialValue = martialValue,
            spellValue = spellValue,
            wealthValue = wealthValue,
            companionValue = companionValue,
            territoryValue = territoryValue,
            physicalGrowthTotal = physicalGrowthTotal,
            spellGrowthTotal = spellGrowthTotal,
            wealthGrowthTotal = wealthGrowthTotal,
            companionGrowthTotal = companionGrowthTotal,
            territoryGrowthTotal = territoryGrowthTotal,
            cultivationOptions = CloneCultivationOptions(cultivationOptions),
            selectedCultivationOptionIndex = selectedCultivationOptionIndex,
            selectedCultivationOptionId = selectedCultivationOptionId,
            selectedCultivationOptionName = selectedCultivationOptionName,
            selectedCultivationOptionSummary = selectedCultivationOptionSummary,
            selectedCultivationOptionSource = selectedCultivationOptionSource,
            selectedCultivationBattlePowerGain = selectedCultivationBattlePowerGain,
            selectedCultivationPhysicalGain = selectedCultivationPhysicalGain,
            selectedCultivationSpellGain = selectedCultivationSpellGain,
            selectedCultivationWealthGain = selectedCultivationWealthGain,
            selectedCultivationCompanionGain = selectedCultivationCompanionGain,
            selectedCultivationTerritoryGain = selectedCultivationTerritoryGain,
            selectedCultivationLuckGain = selectedCultivationLuckGain,
            selectedCultivationSwordIntentGain = selectedCultivationSwordIntentGain,
            selectedCultivationModuleType = selectedCultivationModuleType,
            selectedCultivationModuleName = selectedCultivationModuleName,
            selectedCultivationSlotId = selectedCultivationSlotId,
            selectedCultivationSlotDisplayName = selectedCultivationSlotDisplayName,
            selectedCultivationResolvedEntryId = selectedCultivationResolvedEntryId,
            selectedCultivationResolvedEntryName = selectedCultivationResolvedEntryName,
            selectedCultivationResolvedEntrySummary = selectedCultivationResolvedEntrySummary,
            selectedCultivationTier = selectedCultivationTier,
            hasSelectedCultivationOption = hasSelectedCultivationOption,
            lastGrowthRoundIndex = lastGrowthRoundIndex,
            lastGrowthOptionId = lastGrowthOptionId,
            lastGrowthOptionName = lastGrowthOptionName,
            lastGrowthOptionSummary = lastGrowthOptionSummary,
            lastGrowthBattlePowerGain = lastGrowthBattlePowerGain,
            lastGrowthPhysicalGain = lastGrowthPhysicalGain,
            lastGrowthSpellGain = lastGrowthSpellGain,
            lastGrowthWealthGain = lastGrowthWealthGain,
            lastGrowthCompanionGain = lastGrowthCompanionGain,
            lastGrowthTerritoryGain = lastGrowthTerritoryGain,
            lastGrowthLuckGain = lastGrowthLuckGain,
            buildProfile = buildProfile == null ? null : buildProfile.Clone(),
            heroProfessionId = heroProfessionId,
            professionName = professionName,
            heroTemplateId = heroTemplateId,
            heroTemplateName = heroTemplateName,
            heroTemplateSummary = heroTemplateSummary,
            preferredPosition = preferredPosition,
            martialBias = martialBias,
            spellBias = spellBias,
            wealthBias = wealthBias,
            companionBias = companionBias,
            territoryBias = territoryBias,
            currentFormationPosition = currentFormationPosition,
            pendingFormationPosition = pendingFormationPosition,
            hasConfirmedFormation = hasConfirmedFormation,
            formationSelectionSource = formationSelectionSource,
            selectedSecondaryCultivationOptionIndex = selectedSecondaryCultivationOptionIndex,
            selectedSecondaryCultivationOptionId = selectedSecondaryCultivationOptionId,
            selectedSecondaryCultivationOptionName = selectedSecondaryCultivationOptionName,
            selectedSecondaryCultivationOptionSummary = selectedSecondaryCultivationOptionSummary,
            selectedCarrySlotLabel = selectedCarrySlotLabel,
            selectedCarryTitle = selectedCarryTitle,
            selectedRecommendedPosition = selectedRecommendedPosition,
            swordIntentValue = swordIntentValue,
            swordIntentCurrent = swordIntentCurrent
        };
    }

    private static BattleA2HeroCandidateSnapshot[] CloneCandidates(BattleA2HeroCandidateSnapshot[] source) {
        if (source == null || source.Length == 0) {
            return new BattleA2HeroCandidateSnapshot[0];
        }

        BattleA2HeroCandidateSnapshot[] candidates = new BattleA2HeroCandidateSnapshot[source.Length];
        for (int i = 0; i < source.Length; i++) {
            candidates[i] = source[i] == null ? new BattleA2HeroCandidateSnapshot() : source[i].Clone();
        }

        return candidates;
    }

    private static BattleCultivationOptionSnapshot[] CloneCultivationOptions(BattleCultivationOptionSnapshot[] source) {
        if (source == null || source.Length == 0) {
            return new BattleCultivationOptionSnapshot[0];
        }

        BattleCultivationOptionSnapshot[] options = new BattleCultivationOptionSnapshot[source.Length];
        for (int i = 0; i < source.Length; i++) {
            options[i] = source[i] == null ? new BattleCultivationOptionSnapshot() : source[i].Clone();
        }

        return options;
    }
}
