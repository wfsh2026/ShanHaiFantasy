using System;

public enum BattleDebugActionType {
    None = 0,
    SkipPhase = 1,
    CycleEnvironment = 2,
    CycleFormationEye = 3,
    AddLuck = 4,
    AddSwordIntent = 5,
    ForceFront = 6,
    ForceMiddle = 7,
    ForceRear = 8,
    RefreshCultivation = 9
}

[Serializable]
public sealed class BattleRoundFormationSnapshot {
    public int roundIndex;
    public BattleFormationPositionType eyePosition;
    public string formationName;
    public string description;
    public int hitBattlePowerBonus;
    public int hitLuckBonus;

    public BattleRoundFormationSnapshot Clone() {
        return new BattleRoundFormationSnapshot {
            roundIndex = roundIndex,
            eyePosition = eyePosition,
            formationName = formationName,
            description = description,
            hitBattlePowerBonus = hitBattlePowerBonus,
            hitLuckBonus = hitLuckBonus
        };
    }
}

[Serializable]
public sealed class BattleBuildSlotSnapshot {
    public string slotId;
    public string slotDisplayName;
    public BattleCultivationModuleType moduleType;
    public string entryId;
    public string entryDisplayName;
    public string summary;
    public int tier;
    public int battlePowerBonus;

    public BattleBuildSlotSnapshot Clone() {
        return new BattleBuildSlotSnapshot {
            slotId = slotId,
            slotDisplayName = slotDisplayName,
            moduleType = moduleType,
            entryId = entryId,
            entryDisplayName = entryDisplayName,
            summary = summary,
            tier = tier,
            battlePowerBonus = battlePowerBonus
        };
    }
}

[Serializable]
public sealed class BattleBParticipantProfileSnapshot {
    public string professionId;
    public string professionDisplayName;
    public string heroTemplateId;
    public string heroTemplateDisplayName;
    public string templateSummary;
    public int swordIntent;
    public int swordIntentMax;
    public BattleCultivationModuleType preferredPrimaryModule;
    public BattleCultivationModuleType preferredSecondaryModule;
    public BattleFormationPositionType preferredPosition;
    public BattleBuildSlotSnapshot[] carriedSlots;

    public BattleBParticipantProfileSnapshot Clone() {
        BattleBParticipantProfileSnapshot profile = new BattleBParticipantProfileSnapshot();
        profile.professionId = professionId;
        profile.professionDisplayName = professionDisplayName;
        profile.heroTemplateId = heroTemplateId;
        profile.heroTemplateDisplayName = heroTemplateDisplayName;
        profile.templateSummary = templateSummary;
        profile.swordIntent = swordIntent;
        profile.swordIntentMax = swordIntentMax;
        profile.preferredPrimaryModule = preferredPrimaryModule;
        profile.preferredSecondaryModule = preferredSecondaryModule;
        profile.preferredPosition = preferredPosition;
        profile.carriedSlots = CloneSlotArray(carriedSlots);
        return profile;
    }

    public int GetModulePower(BattleCultivationModuleType moduleType) {
        if (carriedSlots == null || carriedSlots.Length == 0) {
            return 0;
        }

        int total = 0;
        for (int i = 0; i < carriedSlots.Length; i++) {
            BattleBuildSlotSnapshot slot = carriedSlots[i];
            if (slot != null && slot.moduleType == moduleType) {
                total += slot.battlePowerBonus;
            }
        }

        return total;
    }

    public string BuildSlotSummary(BattleCultivationModuleType moduleType, string fallback) {
        if (carriedSlots == null || carriedSlots.Length == 0) {
            return fallback;
        }

        System.Text.StringBuilder builder = new System.Text.StringBuilder(128);
        for (int i = 0; i < carriedSlots.Length; i++) {
            BattleBuildSlotSnapshot slot = carriedSlots[i];
            if (slot == null || slot.moduleType != moduleType) {
                continue;
            }

            if (builder.Length > 0) {
                builder.Append(" | ");
            }

            builder.Append(string.IsNullOrWhiteSpace(slot.slotDisplayName) ? slot.slotId : slot.slotDisplayName)
                .Append(':')
                .Append(string.IsNullOrWhiteSpace(slot.entryDisplayName) ? "空" : slot.entryDisplayName);
        }

        return builder.Length == 0 ? fallback : builder.ToString();
    }

    public void UpsertSlot(BattleBuildSlotSnapshot newSlot) {
        if (newSlot == null || string.IsNullOrWhiteSpace(newSlot.slotId)) {
            return;
        }

        if (carriedSlots == null || carriedSlots.Length == 0) {
            carriedSlots = new[] { newSlot.Clone() };
            return;
        }

        for (int i = 0; i < carriedSlots.Length; i++) {
            if (carriedSlots[i] != null && string.Equals(carriedSlots[i].slotId, newSlot.slotId, StringComparison.Ordinal)) {
                carriedSlots[i] = newSlot.Clone();
                return;
            }
        }

        BattleBuildSlotSnapshot[] expanded = new BattleBuildSlotSnapshot[carriedSlots.Length + 1];
        for (int i = 0; i < carriedSlots.Length; i++) {
            expanded[i] = carriedSlots[i] == null ? null : carriedSlots[i].Clone();
        }

        expanded[expanded.Length - 1] = newSlot.Clone();
        carriedSlots = expanded;
    }

    private static BattleBuildSlotSnapshot[] CloneSlotArray(BattleBuildSlotSnapshot[] source) {
        if (source == null || source.Length == 0) {
            return new BattleBuildSlotSnapshot[0];
        }

        BattleBuildSlotSnapshot[] clone = new BattleBuildSlotSnapshot[source.Length];
        for (int i = 0; i < source.Length; i++) {
            clone[i] = source[i] == null ? new BattleBuildSlotSnapshot() : source[i].Clone();
        }

        return clone;
    }
}

public sealed partial class BattleModeData : AbsModeData {
    private readonly BindableValue<BattleRoundFormationSnapshot> formationSnapshotValue =
        new BindableValue<BattleRoundFormationSnapshot>(null);

    public BindableValue<BattleRoundFormationSnapshot> FormationSnapshotValue { get { return formationSnapshotValue; } }

    public BattleRoundFormationSnapshot FormationSnapshot { get { return FormationSnapshotValue.Value; } }

    public void AssignFormationSnapshot(BattleRoundFormationSnapshot formationSnapshot, bool incrementVersion = true) {
        FormationSnapshotValue.SetValue(formationSnapshot == null ? null : formationSnapshot.Clone());
        if (incrementVersion) {
            StateVersionValue.SetValue(StateVersion + 1);
        }

        HasSnapshot = true;
    }

    public void PrepareFormationConfirmPhase() {
        BattleParticipantSnapshot[] participants = CreateParticipantsClone();
        for (int i = 0; i < participants.Length; i++) {
            BattleParticipantSnapshot participant = participants[i];
            if (participant == null || participant.isEliminated) {
                continue;
            }

            participant.hasConfirmedFormation = false;
            participant.hasCompletedCurrentPhase = false;
            participant.pendingFormationPosition = participant.currentFormationPosition;
            participant.formationSelectionSource = string.Empty;
        }

        ReplaceParticipants(participants);
    }

    public bool AreAllActiveParticipantsFormationConfirmed() {
        BattleParticipantSnapshot[] participants = Participants;
        if (participants == null || participants.Length == 0) {
            return false;
        }

        for (int i = 0; i < participants.Length; i++) {
            BattleParticipantSnapshot participant = participants[i];
            if (participant == null || participant.isEliminated) {
                continue;
            }

            if (!participant.hasConfirmedFormation) {
                return false;
            }
        }

        return true;
    }

    public int AutoConfirmPendingFormationChoices(string selectionSource, bool includeHumanParticipants) {
        if (MainState != BattleMainStateType.RoundLoop || RoundPhase != BattleRoundPhaseType.FormationConfirm) {
            return 0;
        }

        BattleParticipantSnapshot[] participants = CreateParticipantsClone();
        int changedCount = 0;
        for (int i = 0; i < participants.Length; i++) {
            BattleParticipantSnapshot participant = participants[i];
            if (participant == null || participant.isEliminated || participant.hasConfirmedFormation) {
                continue;
            }

            if (!includeHumanParticipants && participant.participantType != NetworkSyncRoomParticipantType.AI) {
                continue;
            }

            BattleFormationPositionType autoPosition =
                BattleBContentCatalog.ResolveAutoFormationPosition(participant, FormationSnapshot);
            if (ApplyFormationChoice(participant, autoPosition, selectionSource, true)) {
                changedCount++;
            }
        }

        if (changedCount > 0) {
            ReplaceParticipants(participants);
        }

        return changedCount;
    }

    public bool TrySubmitFormationChoice(string playerId, BattleFormationPositionType position, string selectionSource) {
        if (MainState != BattleMainStateType.RoundLoop ||
            RoundPhase != BattleRoundPhaseType.FormationConfirm ||
            string.IsNullOrWhiteSpace(playerId)) {
            return false;
        }

        BattleParticipantSnapshot[] participants = CreateParticipantsClone();
        bool changed = false;
        for (int i = 0; i < participants.Length; i++) {
            BattleParticipantSnapshot participant = participants[i];
            if (participant == null || participant.isEliminated || participant.hasConfirmedFormation) {
                continue;
            }

            if (!string.Equals(participant.playerId, playerId, StringComparison.Ordinal)) {
                continue;
            }

            changed = ApplyFormationChoice(participant, position, selectionSource, false);
            break;
        }

        if (!changed) {
            return false;
        }

        ReplaceParticipants(participants);
        return true;
    }

    private static bool ApplyFormationChoice(
        BattleParticipantSnapshot participant,
        BattleFormationPositionType position,
        string selectionSource,
        bool allowFallbackPosition) {
        if (participant == null || participant.isEliminated || participant.hasConfirmedFormation) {
            return false;
        }

        BattleFormationPositionType resolvedPosition = position;
        if (resolvedPosition == BattleFormationPositionType.None && allowFallbackPosition) {
            resolvedPosition = BattleBContentCatalog.ResolveAutoFormationPosition(participant, null);
        }

        participant.pendingFormationPosition = resolvedPosition;
        participant.currentFormationPosition = resolvedPosition;
        participant.hasConfirmedFormation = true;
        participant.hasCompletedCurrentPhase = true;
        participant.formationSelectionSource = string.IsNullOrWhiteSpace(selectionSource) ? string.Empty : selectionSource.Trim();
        return true;
    }
}

public sealed partial class BattleParticipantSnapshot {
    public BattleBParticipantProfileSnapshot buildProfile;
    public string professionName;
    public string heroProfessionId;
    public string heroTemplateId;
    public string heroTemplateName;
    public string heroTemplateSummary;
    public BattleFormationPositionType preferredPosition;
    public int martialBias;
    public int spellBias;
    public int wealthBias;
    public int companionBias;
    public int territoryBias;
    public BattleFormationPositionType currentFormationPosition;
    public BattleFormationPositionType pendingFormationPosition;
    public bool hasConfirmedFormation;
    public string formationSelectionSource;
    public int selectedSecondaryCultivationOptionIndex;
    public string selectedSecondaryCultivationOptionId;
    public string selectedSecondaryCultivationOptionName;
    public string selectedSecondaryCultivationOptionSummary;
    public string selectedCarrySlotLabel;
    public string selectedCarryTitle;
    public int selectedCultivationSwordIntentGain;
    public BattleCultivationModuleType selectedCultivationModuleType;
    public string selectedCultivationModuleName;
    public string selectedCultivationSlotId;
    public string selectedCultivationSlotDisplayName;
    public string selectedCultivationResolvedEntryId;
    public string selectedCultivationResolvedEntryName;
    public string selectedCultivationResolvedEntrySummary;
    public int selectedCultivationTier;
    public BattleFormationPositionType selectedRecommendedPosition;
    public int swordIntentValue;
    public int swordIntentCurrent;
}
