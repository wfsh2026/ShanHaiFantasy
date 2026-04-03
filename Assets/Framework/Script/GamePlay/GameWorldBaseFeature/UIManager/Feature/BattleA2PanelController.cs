using System.Text;
using UnityEngine;

public sealed class BattleA2PanelController : UIControllerBase<BattleA2Panel> {
    private BattleModeData data;
    private ClientBattleModeLogic logic;

    public BattleA2PanelController(BattleA2Panel targetPanel) : base(targetPanel) { }

    public override void Bind() {
        data = ResolveData();
        logic = ResolveLogic();
        panel.ShowDefault();
        if (data == null) {
            return;
        }

        data.MainStateValue.Bind(OnStateChanged, true);
        data.RoundIndexValue.Bind(OnRoundChanged, false);
        data.RoundPhaseValue.Bind(OnPhaseChanged, false);
        data.StageNameValue.Bind(OnStageChanged, false);
        data.StageEnterCountValue.Bind(OnStageEnterChanged, false);
        data.StateVersionValue.Bind(OnVersionChanged, false);
        data.ParticipantListValue.Bind(OnParticipantsChanged, false);
        data.BattleMatchListValue.Bind(OnMatchesChanged, false);
        data.SectEnvironmentValue.Bind(OnEnvironmentChanged, false);
        data.FormationSnapshotValue.Bind(OnFormationChanged, false);
        data.WinnerPlayerIdValue.Bind(OnWinnerChanged, false);
        RefreshView();
    }

    public override void Unbind() {
        if (data == null) {
            return;
        }

        data.MainStateValue.Unbind(OnStateChanged);
        data.RoundIndexValue.Unbind(OnRoundChanged);
        data.RoundPhaseValue.Unbind(OnPhaseChanged);
        data.StageNameValue.Unbind(OnStageChanged);
        data.StageEnterCountValue.Unbind(OnStageEnterChanged);
        data.StateVersionValue.Unbind(OnVersionChanged);
        data.ParticipantListValue.Unbind(OnParticipantsChanged);
        data.BattleMatchListValue.Unbind(OnMatchesChanged);
        data.SectEnvironmentValue.Unbind(OnEnvironmentChanged);
        data.FormationSnapshotValue.Unbind(OnFormationChanged);
        data.WinnerPlayerIdValue.Unbind(OnWinnerChanged);
        data = null;
        logic = null;
    }

    public void SubmitCandidate(int index) {
        if (logic == null || data == null) {
            return;
        }

        if (data.MainState == BattleMainStateType.HeroSelect) {
            logic.SubmitHeroSelection(index);
            return;
        }

        if (data.MainState == BattleMainStateType.RoundLoop && data.RoundPhase == BattleRoundPhaseType.Cultivation) {
            logic.SubmitCultivationSelection(index);
            return;
        }

        if (data.MainState == BattleMainStateType.RoundLoop && data.RoundPhase == BattleRoundPhaseType.FormationConfirm) {
            logic.SubmitFormationPosition(ResolvePositionByIndex(index));
        }
    }

    private void OnStateChanged(BattleMainStateType _) { RefreshView(); }
    private void OnRoundChanged(int _) { RefreshView(); }
    private void OnPhaseChanged(BattleRoundPhaseType _) { RefreshView(); }
    private void OnStageChanged(string _) { RefreshView(); }
    private void OnStageEnterChanged(int _) { RefreshView(); }
    private void OnVersionChanged(int _) { RefreshView(); }
    private void OnParticipantsChanged(BattleParticipantSnapshot[] _) { RefreshView(); }
    private void OnMatchesChanged(BattleRoundMatchSnapshot[] _) { RefreshView(); }
    private void OnEnvironmentChanged(BattleA2SectEnvironmentSnapshot _) { RefreshView(); }
    private void OnFormationChanged(BattleRoundFormationSnapshot _) { RefreshView(); }
    private void OnWinnerChanged(string _) { RefreshView(); }

    private void RefreshView() {
        if (data == null) {
            panel.ShowDefault();
            return;
        }

        string localPlayerId = logic == null ? string.Empty : logic.LocalPlayerId;
        BattleParticipantSnapshot local = ResolveLocalParticipant(localPlayerId);
        panel.RefreshHeader(BuildTitle(), BuildSubtitle(local));
        panel.RefreshLocalPlayer(BuildLocalPlayerText(localPlayerId, local));
        panel.RefreshStageState(BuildStageText());
        panel.RefreshSection(BuildSectionTitle(), BuildSectionBody(local));
        panel.RefreshHeroProfile(BuildHeroProfileText(local));
        panel.RefreshFormation(BuildFormationText(local));
        panel.RefreshParticipants(BuildParticipantsText());
        panel.RefreshProgress(BuildProgressText(local));
        panel.RefreshFooter(BuildFooterText(local));
        RefreshCards(local);
    }

    private void RefreshCards(BattleParticipantSnapshot local) {
        bool heroSelect = data.MainState == BattleMainStateType.HeroSelect;
        bool cultivation = data.MainState == BattleMainStateType.RoundLoop &&
            data.RoundPhase == BattleRoundPhaseType.Cultivation;
        bool formation = data.MainState == BattleMainStateType.RoundLoop &&
            data.RoundPhase == BattleRoundPhaseType.FormationConfirm;

        for (int i = 0; i < 4; i++) {
            if (heroSelect) {
                BattleA2HeroCandidateSnapshot candidate =
                    local != null && local.heroCandidates != null && i < local.heroCandidates.Length
                        ? local.heroCandidates[i]
                        : null;
                bool selected = local != null && local.selectedCandidateIndex == i;
                bool interactable = local != null && candidate != null && !local.hasLockedHero;
                panel.RefreshCandidateCard(
                    i,
                    candidate == null ? "Candidate " + (i + 1) : Normalize(candidate.displayName, "Candidate " + (i + 1)),
                    candidate == null ? "Waiting for hero candidates." : BuildHeroCardBody(candidate, selected),
                    i < 3,
                    interactable,
                    selected);
                continue;
            }

            if (cultivation) {
                BattleCultivationOptionSnapshot option =
                    local != null && local.cultivationOptions != null && i < local.cultivationOptions.Length
                        ? local.cultivationOptions[i]
                        : null;
                bool selected = local != null && local.selectedCultivationOptionIndex == i;
                bool interactable = local != null && option != null && !local.hasSelectedCultivationOption;
                panel.RefreshCandidateCard(
                    i,
                    option == null ? "Option " + (i + 1) : Normalize(option.displayName, "Option " + (i + 1)),
                    option == null ? "Waiting for cultivation options." : BuildCultivationCardBody(option, selected),
                    true,
                    interactable,
                    selected);
                continue;
            }

            if (formation) {
                BattleFormationPositionType position = ResolvePositionByIndex(i);
                bool selected = local != null && local.currentFormationPosition == position;
                bool interactable = local != null && !local.hasConfirmedFormation && position != BattleFormationPositionType.None;
                panel.RefreshCandidateCard(
                    i,
                    BuildPositionTitle(i),
                    BuildPositionBody(local, i),
                    i < 3,
                    interactable,
                    selected);
                continue;
            }

            panel.RefreshCandidateCard(i, string.Empty, string.Empty, false, false, false);
        }
    }

    private string BuildTitle() {
        if (data.MainState == BattleMainStateType.HeroSelect) {
            return "Main Hero Select";
        }

        if (data.MainState == BattleMainStateType.SectReveal) {
            return "Sect Reveal";
        }

        if (data.MainState == BattleMainStateType.GameOver) {
            return "Battle Result";
        }

        if (data.RoundPhase == BattleRoundPhaseType.Cultivation) {
            return "Cultivation";
        }

        if (data.RoundPhase == BattleRoundPhaseType.FormationConfirm) {
            return "Formation Confirm";
        }

        if (data.RoundPhase == BattleRoundPhaseType.BattleResult) {
            return "Round Result";
        }

        return "Battle Round";
    }

    private string BuildSubtitle(BattleParticipantSnapshot local) {
        if (data.MainState == BattleMainStateType.HeroSelect) {
            return "Choose 1 of 3 hero templates for this match.";
        }

        if (data.MainState == BattleMainStateType.SectReveal) {
            return data.SectEnvironment == null
                ? "Waiting for sect environment."
                : Normalize(data.SectEnvironment.displayName, "Sect Environment") +
                " | Eye: " + Normalize(data.SectEnvironment.arrayEyeLabel, "-");
        }

        if (data.MainState == BattleMainStateType.GameOver) {
            return "Winner: " + Normalize(data.WinnerPlayerId, "Pending");
        }

        if (data.RoundPhase == BattleRoundPhaseType.Cultivation) {
            return local != null && local.hasSelectedCultivationOption
                ? "Selection submitted. Waiting for the rest of the room."
                : "Pick 1 of 4 options to update your carried build.";
        }

        if (data.RoundPhase == BattleRoundPhaseType.FormationConfirm) {
            return "Pick front, middle or rear to contest the formation eye.";
        }

        return "Round " + data.RoundIndex;
    }

    private string BuildLocalPlayerText(string localPlayerId, BattleParticipantSnapshot local) {
        if (string.IsNullOrWhiteSpace(localPlayerId)) {
            return "Local Player: waiting for room binding.";
        }

        if (local == null) {
            return "Local Player: " + localPlayerId + "\nSnapshot not bound to this participant yet.";
        }

        StringBuilder builder = new StringBuilder(256);
        builder.Append("Local Player: ").Append(localPlayerId)
            .Append("\nDisplay: ").Append(Normalize(local.displayName, localPlayerId))
            .Append("\nHero: ").Append(Normalize(local.selectedHeroName, "None"))
            .Append("\nProfession: ").Append(Normalize(local.professionName, "Unknown"))
            .Append("\nLuck: ").Append(local.luckValue)
            .Append(" | SwordIntent: ").Append(local.swordIntentValue)
            .Append("\nPosition: ").Append(BattleBContentCatalog.ResolveFormationLabel(local.currentFormationPosition));
        return builder.ToString();
    }

    private string BuildStageText() {
        return "MainState: " + data.MainState +
            "\nStage: " + Normalize(data.StageName, "BattleModeStage") +
            "\nRound: " + data.RoundIndex + " | Phase: " + data.RoundPhase +
            "\nEye: " + Normalize(
                data.FormationSnapshot == null
                    ? (data.SectEnvironment == null ? string.Empty : data.SectEnvironment.arrayEyeLabel)
                    : BattleBContentCatalog.ResolveFormationLabel(data.FormationSnapshot.eyePosition),
                "Pending") +
            "\nAlive: " + data.AliveCount + " | Version: " + data.StateVersion;
    }

    private string BuildSectionTitle() {
        if (data.MainState == BattleMainStateType.HeroSelect) {
            return "Hero Choice";
        }

        if (data.MainState == BattleMainStateType.SectReveal) {
            return "Environment";
        }

        if (data.MainState == BattleMainStateType.GameOver) {
            return "Final Result";
        }

        if (data.RoundPhase == BattleRoundPhaseType.Battle) {
            return "Current Battle";
        }

        if (data.RoundPhase == BattleRoundPhaseType.BattleResult) {
            return "Battle Result";
        }

        if (data.RoundPhase == BattleRoundPhaseType.Cultivation) {
            return "Cultivation Choice";
        }

        return "Formation";
    }

    private string BuildSectionBody(BattleParticipantSnapshot local) {
        if (data.MainState == BattleMainStateType.HeroSelect) {
            return local != null && local.hasLockedHero
                ? "Hero locked. Waiting for all participants to finish selection."
                : "Hero template decides profession, preferred module, sword intent and opening position.";
        }

        if (data.MainState == BattleMainStateType.SectReveal) {
            return data.SectEnvironment == null
                ? "Waiting for environment snapshot."
                : Normalize(data.SectEnvironment.description, "No description.") + "\n" +
                Normalize(data.SectEnvironment.arrayEyeSummary, string.Empty);
        }

        if (data.MainState == BattleMainStateType.GameOver) {
            return "Match finished. Winner: " + Normalize(data.WinnerPlayerId, "Pending");
        }

        if (data.RoundPhase == BattleRoundPhaseType.Battle) {
            return BuildBattleSectionText(GetLatestRelevantMatch(local), local);
        }

        if (data.RoundPhase == BattleRoundPhaseType.BattleResult) {
            return BuildBattleResultSectionText(GetLatestRelevantMatch(local), local);
        }

        if (data.RoundPhase == BattleRoundPhaseType.Cultivation) {
            return local != null && local.hasSelectedCultivationOption
                ? BuildCultivationSelectionSummary(local)
                : "Each option updates a carried slot and changes next-round module focus.";
        }

        return "Current eye: " + Normalize(
            data.FormationSnapshot == null ? string.Empty : BattleBContentCatalog.ResolveFormationLabel(data.FormationSnapshot.eyePosition),
            "Pending") +
            "\nSuggested position: " + (local == null ? "-" : BattleBContentCatalog.ResolveFormationLabel(ResolveSuggestedFormation(local))) +
            "\nConfirming formation affects the next battle settlement.";
    }

    private string BuildHeroProfileText(BattleParticipantSnapshot local) {
        if (local == null) {
            return "Hero Profile\nWaiting for local participant snapshot.";
        }

        StringBuilder builder = new StringBuilder(512);
        builder.Append("Hero Profile\n")
            .Append("Profession: ").Append(Normalize(local.professionName, "Unknown"))
            .Append(" | Template: ").Append(Normalize(local.heroTemplateName, "Pending")).Append('\n')
            .Append("Template Summary: ").Append(Normalize(local.heroTemplateSummary, "Pending")).Append('\n')
            .Append("Martial: ").Append(BuildModuleSlots(local, BattleCultivationModuleType.Martial)).Append('\n')
            .Append("Spell: ").Append(BuildModuleSlots(local, BattleCultivationModuleType.Spell)).Append('\n')
            .Append("Wealth: ").Append(BuildModuleSlots(local, BattleCultivationModuleType.Wealth)).Append('\n')
            .Append("Companion: ").Append(BuildModuleSlots(local, BattleCultivationModuleType.Companion)).Append('\n')
            .Append("Territory: ").Append(BuildModuleSlots(local, BattleCultivationModuleType.Territory)).Append('\n')
            .Append("Growth Totals M/S/W/C/T: ")
            .Append(local.physicalGrowthTotal).Append('/')
            .Append(local.spellGrowthTotal).Append('/')
            .Append(local.wealthGrowthTotal).Append('/')
            .Append(local.companionGrowthTotal).Append('/')
            .Append(local.territoryGrowthTotal);
        return builder.ToString();
    }

    private string BuildFormationText(BattleParticipantSnapshot local) {
        string eye = Normalize(
            data.FormationSnapshot == null
                ? (data.SectEnvironment == null ? string.Empty : data.SectEnvironment.arrayEyeLabel)
                : BattleBContentCatalog.ResolveFormationLabel(data.FormationSnapshot.eyePosition),
            "Pending");
        string description = Normalize(
            data.FormationSnapshot == null
                ? (data.SectEnvironment == null ? string.Empty : data.SectEnvironment.arrayEyeSummary)
                : data.FormationSnapshot.description,
            "Waiting for formation snapshot.");

        if (local == null) {
            return "Formation\nEye: " + eye + "\n" + description;
        }

        return "Formation\nEye: " + eye +
            "\nCurrent: " + BattleBContentCatalog.ResolveFormationLabel(local.currentFormationPosition) +
            "\nPending: " + BattleBContentCatalog.ResolveFormationLabel(local.pendingFormationPosition) +
            "\nSuggested: " + BattleBContentCatalog.ResolveFormationLabel(ResolveSuggestedFormation(local)) +
            "\nSource: " + Normalize(local.formationSelectionSource, "Pending") +
            "\nCarry: " + Normalize(local.selectedCarrySlotLabel, "-") + " / " + Normalize(local.selectedCarryTitle, "-") +
            "\n" + description;
    }

    private string BuildParticipantsText() {
        if (data.Participants == null || data.Participants.Length == 0) {
            return "Participants\nNo participants yet.";
        }

        StringBuilder builder = new StringBuilder(512);
        builder.Append("Participants\n");
        for (int i = 0; i < data.Participants.Length; i++) {
            BattleParticipantSnapshot participant = data.Participants[i];
            if (participant == null) {
                continue;
            }

            builder.Append('#').Append(participant.slotIndex + 1).Append(' ')
                .Append(Normalize(participant.displayName, participant.playerId))
                .Append(" | Luck ").Append(participant.luckValue)
                .Append(" | Pos ").Append(BattleBContentCatalog.ResolveFormationLabel(participant.currentFormationPosition))
                .Append(" | Hero ").Append(Normalize(participant.heroTemplateName, "-"))
                .Append(" | Style ").Append(Normalize(BattleBContentCatalog.ResolveStyleLabel(participant), "-"))
                .Append(" | Out ").Append(participant.isEliminated)
                .Append('\n');
        }

        return builder.ToString().TrimEnd();
    }

    private string BuildProgressText(BattleParticipantSnapshot local) {
        BattleRoundMatchSnapshot match = GetLatestRelevantMatch(local);
        return "Round Summary\n" +
            "Flow: " + data.MainState + " / " + data.RoundPhase + "\n" +
            "Opponent: " + (local == null ? "-" : Normalize(local.recentBattleOpponentDisplayName, "Pending")) + "\n" +
            "Recent Result: " + (local == null ? "-" : local.recentBattleOutcome.ToString()) + "\n" +
            "Recent Carry: " + (local == null ? "-" : Normalize(local.selectedCarryTitle, Normalize(local.lastGrowthOptionName, "-"))) + "\n" +
            "AI Decision Exists: " + HasAiParticipantDecision() + "\n" +
            "Battle Note: " + (match == null ? "Waiting for settlement." : Normalize(match.eyeSummary, "Waiting for settlement."));
    }

    private string BuildFooterText(BattleParticipantSnapshot local) {
        if (data.MainState == BattleMainStateType.HeroSelect) {
            return "Click a hero card to lock the hero template for this match.";
        }

        if (data.MainState == BattleMainStateType.RoundLoop && data.RoundPhase == BattleRoundPhaseType.Cultivation) {
            return local != null && local.hasSelectedCultivationOption
                ? "Cultivation submitted. Waiting to enter formation confirm."
                : "Cultivation updates carry slots, battle power and next-round preference.";
        }

        if (data.MainState == BattleMainStateType.RoundLoop && data.RoundPhase == BattleRoundPhaseType.FormationConfirm) {
            return local != null && local.hasConfirmedFormation
                ? "Formation confirmed. Waiting for the room."
                : "Click front, middle or rear to confirm formation.";
        }

        return "This panel only consumes the authoritative server snapshot.";
    }

    private string BuildBattleSectionText(BattleRoundMatchSnapshot match, BattleParticipantSnapshot local) {
        if (match == null) {
            return "Waiting for server-side battle settlement.";
        }

        bool isLeft = IsLocalLeftSide(match, local);
        string selfName = isLeft ? Normalize(match.leftDisplayName, match.leftPlayerId) : Normalize(match.rightDisplayName, match.rightPlayerId);
        string enemyName = isLeft ? Normalize(match.rightDisplayName, match.rightPlayerId) : Normalize(match.leftDisplayName, match.leftPlayerId);
        string selfCarry = isLeft ? Normalize(match.leftCarrySummary, "-") : Normalize(match.rightCarrySummary, "-");
        string enemyCarry = isLeft ? Normalize(match.rightCarrySummary, "-") : Normalize(match.leftCarrySummary, "-");
        string selfStyle = isLeft ? Normalize(match.leftStyleSummary, "-") : Normalize(match.rightStyleSummary, "-");
        string enemyStyle = isLeft ? Normalize(match.rightStyleSummary, "-") : Normalize(match.leftStyleSummary, "-");

        return "Self: " + selfName + " | Score " + (isLeft ? match.leftScore : match.rightScore) +
            "\nEnemy: " + enemyName + " | Score " + (isLeft ? match.rightScore : match.leftScore) +
            "\nSelf Carry: " + selfCarry +
            "\nEnemy Carry: " + enemyCarry +
            "\nSelf Style: " + selfStyle +
            "\nEnemy Style: " + enemyStyle +
            "\nEye: " + Normalize(match.eyeSummary, "-");
    }

    private string BuildBattleResultSectionText(BattleRoundMatchSnapshot match, BattleParticipantSnapshot local) {
        if (local == null) {
            return "Waiting for local participant snapshot.";
        }

        if (match == null) {
            return "Waiting for result snapshot.";
        }

        bool isLeft = IsLocalLeftSide(match, local);
        BattleBattleOutcomeType outcome = isLeft ? match.leftOutcome : match.rightOutcome;
        string reason = isLeft ? Normalize(match.leftReasonSummary, "-") : Normalize(match.rightReasonSummary, "-");
        int luckDelta = isLeft ? match.leftLuckDelta : match.rightLuckDelta;

        return "Outcome: " + outcome +
            "\nSummary: " + Normalize(match.summary, "-") +
            "\nReason: " + reason +
            "\nLuck Delta: " + FormatSigned(luckDelta) +
            "\nRecent Battle: " + Normalize(local.recentBattleSummary, "-");
    }

    private string BuildCultivationSelectionSummary(BattleParticipantSnapshot local) {
        return "Selected: " + Normalize(local.selectedCultivationOptionName, "Pending") +
            "\nSummary: " + Normalize(local.selectedCultivationOptionSummary, "-") +
            "\nCarry Transition: " + BattleBContentCatalog.BuildCarryTransitionPreview(local) +
            "\nResolved Entry: " + Normalize(local.selectedCultivationResolvedEntryName, "-") +
            "\nBattle Power: " + FormatSigned(local.selectedCultivationBattlePowerGain) +
            "\nGrowth M/S/W/C/T: " +
            FormatSigned(local.selectedCultivationPhysicalGain) + "/" +
            FormatSigned(local.selectedCultivationSpellGain) + "/" +
            FormatSigned(local.selectedCultivationWealthGain) + "/" +
            FormatSigned(local.selectedCultivationCompanionGain) + "/" +
            FormatSigned(local.selectedCultivationTerritoryGain) +
            "\nLuck: " + FormatSigned(local.selectedCultivationLuckGain);
    }

    private string BuildHeroCardBody(BattleA2HeroCandidateSnapshot candidate, bool selected) {
        return Normalize(candidate.professionName, "Unknown") +
            "\nTemplate: " + Normalize(candidate.templateName, "-") +
            "\nPower: " + candidate.powerScore +
            "\nSwordIntent: " + candidate.swordIntentStart +
            "\nPreferred Pos: " + BattleBContentCatalog.ResolveFormationLabel(candidate.preferredPosition) +
            "\n" + Normalize(candidate.description, "-") +
            (selected ? "\n[Selected]" : string.Empty);
    }

    private string BuildCultivationCardBody(BattleCultivationOptionSnapshot option, bool selected) {
        StringBuilder builder = new StringBuilder(256);
        builder.Append(Normalize(option.description, "-"))
            .Append("\nModule: ").Append(BattleBContentCatalog.ResolveModuleLabel(option.moduleType))
            .Append(" | Slot: ").Append(Normalize(option.carrySlotLabel, "-"))
            .Append("\nPower: ").Append(FormatSigned(option.battlePowerBonus))
            .Append(" | Luck: ").Append(FormatSigned(option.luckGain))
            .Append("\nGrowth M/S/W/C/T: ")
            .Append(FormatSigned(option.physicalGain)).Append('/')
            .Append(FormatSigned(option.spellGain)).Append('/')
            .Append(FormatSigned(option.wealthGain)).Append('/')
            .Append(FormatSigned(option.companionGain)).Append('/')
            .Append(FormatSigned(option.territoryGain))
            .Append("\nRecommended Pos: ").Append(BattleBContentCatalog.ResolveFormationLabel(option.recommendedPosition));

        if (option.secondaryOptions != null && option.secondaryOptions.Length > 0 && option.secondaryOptions[0] != null) {
            builder.Append("\nSecondary: ").Append(Normalize(option.secondaryOptions[0].displayName, "-"));
        }

        if (selected) {
            builder.Append("\n[Selected]");
        }

        return builder.ToString();
    }

    private string BuildPositionTitle(int index) {
        BattleFormationPositionType position = ResolvePositionByIndex(index);
        return position == BattleFormationPositionType.None
            ? string.Empty
            : BattleBContentCatalog.ResolveFormationLabel(position);
    }

    private string BuildPositionBody(BattleParticipantSnapshot local, int index) {
        BattleFormationPositionType position = ResolvePositionByIndex(index);
        if (position == BattleFormationPositionType.None) {
            return string.Empty;
        }

        string suggested = local == null
            ? "-"
            : BattleBContentCatalog.ResolveFormationLabel(ResolveSuggestedFormation(local));
        bool hitsEye = data.FormationSnapshot != null && data.FormationSnapshot.eyePosition == position;
        return "Eye Hit: " + hitsEye +
            "\nSuggested: " + suggested +
            "\nCurrent: " + (local == null ? "-" : BattleBContentCatalog.ResolveFormationLabel(local.currentFormationPosition));
    }

    private BattleParticipantSnapshot ResolveLocalParticipant(string localPlayerId) {
        if (data == null || data.Participants == null || data.Participants.Length == 0) {
            return null;
        }

        if (!string.IsNullOrWhiteSpace(localPlayerId)) {
            for (int i = 0; i < data.Participants.Length; i++) {
                BattleParticipantSnapshot participant = data.Participants[i];
                if (participant != null && string.Equals(participant.playerId, localPlayerId, System.StringComparison.Ordinal)) {
                    return participant;
                }
            }
        }

        for (int i = 0; i < data.Participants.Length; i++) {
            if (data.Participants[i] != null && data.Participants[i].isHost) {
                return data.Participants[i];
            }
        }

        return data.Participants[0];
    }

    private string BuildModuleSlots(BattleParticipantSnapshot local, BattleCultivationModuleType moduleType) {
        if (local == null || local.buildProfile == null) {
            return "-";
        }

        return local.buildProfile.BuildSlotSummary(moduleType, "-");
    }

    private bool HasAiParticipantDecision() {
        if (data == null || data.Participants == null) {
            return false;
        }

        for (int i = 0; i < data.Participants.Length; i++) {
            BattleParticipantSnapshot participant = data.Participants[i];
            if (participant == null || participant.participantType != NetworkSyncRoomParticipantType.AI) {
                continue;
            }

            if (participant.hasLockedHero || participant.hasSelectedCultivationOption || participant.hasConfirmedFormation) {
                return true;
            }
        }

        return false;
    }

    private BattleRoundMatchSnapshot GetLatestRelevantMatch(BattleParticipantSnapshot local) {
        if (data == null || data.BattleMatchList == null || data.BattleMatchList.Length == 0) {
            return null;
        }

        if (local == null) {
            return GetLatestMatch();
        }

        for (int i = data.BattleMatchList.Length - 1; i >= 0; i--) {
            BattleRoundMatchSnapshot match = data.BattleMatchList[i];
            if (match == null) {
                continue;
            }

            if (string.Equals(match.leftPlayerId, local.playerId, System.StringComparison.Ordinal) ||
                string.Equals(match.rightPlayerId, local.playerId, System.StringComparison.Ordinal)) {
                return match;
            }
        }

        return GetLatestMatch();
    }

    private BattleRoundMatchSnapshot GetLatestMatch() {
        if (data == null || data.BattleMatchList == null) {
            return null;
        }

        for (int i = data.BattleMatchList.Length - 1; i >= 0; i--) {
            if (data.BattleMatchList[i] != null) {
                return data.BattleMatchList[i];
            }
        }

        return null;
    }

    private bool IsLocalLeftSide(BattleRoundMatchSnapshot match, BattleParticipantSnapshot local) {
        if (match == null || local == null) {
            return true;
        }

        return string.Equals(match.leftPlayerId, local.playerId, System.StringComparison.Ordinal);
    }

    private BattleFormationPositionType ResolveSuggestedFormation(BattleParticipantSnapshot local) {
        return BattleBContentCatalog.ResolveAutoFormationPosition(local, data == null ? null : data.FormationSnapshot);
    }

    private ClientBattleModeManager ResolveModeManager() {
        if (UIManager.Instance == null || UIManager.Instance.GameWorld == null) {
            return null;
        }

        return UIManager.Instance.GameWorld.GetExtendFeature<ClientBattleModeManager>();
    }

    private BattleModeData ResolveData() {
        ClientBattleModeManager manager = ResolveModeManager();
        return manager == null ? null : manager.GetData<BattleModeData>();
    }

    private ClientBattleModeLogic ResolveLogic() {
        ClientBattleModeManager manager = ResolveModeManager();
        return manager == null ? null : manager.GetLogic<ClientBattleModeLogic>();
    }

    private static BattleFormationPositionType ResolvePositionByIndex(int index) {
        switch (index) {
            case 0:
                return BattleFormationPositionType.Front;
            case 1:
                return BattleFormationPositionType.Middle;
            case 2:
                return BattleFormationPositionType.Rear;
            default:
                return BattleFormationPositionType.None;
        }
    }

    private static string Normalize(string value, string fallback) {
        return string.IsNullOrWhiteSpace(value) ? fallback : value;
    }

    private static string FormatSigned(int value) {
        return value >= 0 ? "+" + value : value.ToString();
    }
}
