using System;
using System.Text;

public static class BattleBContentCatalog {
    private static readonly BattleBuildSlotSnapshot[] DEFAULT_SLOT_TEMPLATES = {
        CreateSlot("martial_weapon", "兵器槽", BattleCultivationModuleType.Martial),
        CreateSlot("martial_artifact", "法宝槽", BattleCultivationModuleType.Martial),
        CreateSlot("martial_armor", "护具槽", BattleCultivationModuleType.Martial),
        CreateSlot("martial_body", "炼体位", BattleCultivationModuleType.Martial),
        CreateSlot("spell_main", "主功法", BattleCultivationModuleType.Spell),
        CreateSlot("spell_heart", "心法槽", BattleCultivationModuleType.Spell),
        CreateSlot("spell_art", "神通位", BattleCultivationModuleType.Spell),
        CreateSlot("wealth_estate", "洞府经营", BattleCultivationModuleType.Wealth),
        CreateSlot("wealth_vault", "宝阁流转", BattleCultivationModuleType.Wealth),
        CreateSlot("wealth_mission", "宗门委托", BattleCultivationModuleType.Wealth),
        CreateSlot("companion_support", "灵侣协同", BattleCultivationModuleType.Companion),
        CreateSlot("companion_role", "灵宠方向", BattleCultivationModuleType.Companion),
        CreateSlot("territory_array", "阵法", BattleCultivationModuleType.Territory),
        CreateSlot("territory_eye", "阵眼适配", BattleCultivationModuleType.Territory),
        CreateSlot("territory_position", "站位偏好", BattleCultivationModuleType.Territory)
    };

    public static BattleBParticipantProfileSnapshot BuildDefaultProfile(int slotIndex) {
        BattleA2HeroCandidateSnapshot[] candidates = BattleA2ContentCatalog.BuildHeroCandidates(slotIndex, 0, 1);
        BattleA2HeroCandidateSnapshot candidate = candidates != null && candidates.Length > 0 ? candidates[0] : null;
        return BuildProfileFromHero(candidate);
    }

    public static BattleBParticipantProfileSnapshot BuildProfileFromHero(BattleA2HeroCandidateSnapshot candidate) {
        BattleBParticipantProfileSnapshot profile = new BattleBParticipantProfileSnapshot();
        profile.professionId = candidate == null ? "profession_sword" : candidate.professionId ?? "profession_sword";
        profile.professionDisplayName = candidate == null ? "剑修" : candidate.professionName ?? "剑修";
        profile.heroTemplateId = candidate == null ? "default" : candidate.templateId ?? "default";
        profile.heroTemplateDisplayName = candidate == null ? "默认" : candidate.templateName ?? "默认";
        profile.templateSummary = candidate == null ? "默认模板" : candidate.templateSummary ?? "默认模板";
        profile.swordIntent = candidate == null ? 0 : candidate.swordIntentStart;
        profile.swordIntentMax = 12;
        profile.preferredPrimaryModule = ResolvePrimaryModule(candidate);
        profile.preferredSecondaryModule = ResolveSecondaryModule(candidate);
        profile.preferredPosition = candidate == null ? BattleFormationPositionType.Middle : candidate.preferredPosition;
        profile.carriedSlots = CloneDefaultSlots();
        return profile;
    }

    public static BattleRoundFormationSnapshot BuildFormationSnapshot(int roundIndex, BattleA2SectEnvironmentSnapshot environment) {
        BattleFormationPositionType eyePosition = environment == null
            ? BattleFormationPositionType.Middle
            : environment.arrayEyePosition;

        return new BattleRoundFormationSnapshot {
            roundIndex = roundIndex,
            eyePosition = eyePosition,
            formationName = "本回合阵眼",
            description = "阵眼位于" + ResolveFormationLabel(eyePosition) + "，命中可获得额外收益。",
            hitBattlePowerBonus = 4 + Math.Max(0, roundIndex - 1),
            hitLuckBonus = 1
        };
    }

    public static BattleFormationPositionType ResolveAutoFormationPosition(
        BattleParticipantSnapshot participant,
        BattleRoundFormationSnapshot formationSnapshot) {
        if (participant == null) {
            return BattleFormationPositionType.Middle;
        }

        if (participant.selectedRecommendedPosition != BattleFormationPositionType.None) {
            return participant.selectedRecommendedPosition;
        }

        if (participant.buildProfile != null) {
            if (participant.buildProfile.preferredPrimaryModule == BattleCultivationModuleType.Territory &&
                formationSnapshot != null) {
                return formationSnapshot.eyePosition;
            }

            if (participant.buildProfile.preferredPrimaryModule == BattleCultivationModuleType.Martial) {
                return participant.buildProfile.preferredPosition == BattleFormationPositionType.None
                    ? BattleFormationPositionType.Front
                    : participant.buildProfile.preferredPosition;
            }

            if (participant.buildProfile.preferredPrimaryModule == BattleCultivationModuleType.Spell) {
                return participant.buildProfile.preferredPosition == BattleFormationPositionType.None
                    ? BattleFormationPositionType.Rear
                    : participant.buildProfile.preferredPosition;
            }

            return participant.buildProfile.preferredPosition == BattleFormationPositionType.None
                ? (formationSnapshot == null ? BattleFormationPositionType.Middle : formationSnapshot.eyePosition)
                : participant.buildProfile.preferredPosition;
        }

        return formationSnapshot == null ? BattleFormationPositionType.Middle : formationSnapshot.eyePosition;
    }

    public static string ResolveFormationLabel(BattleFormationPositionType position) {
        switch (position) {
            case BattleFormationPositionType.Front:
                return "前位";
            case BattleFormationPositionType.Middle:
                return "中位";
            case BattleFormationPositionType.Rear:
                return "后位";
            default:
                return "未定";
        }
    }

    public static string ResolveModuleLabel(BattleCultivationModuleType moduleType) {
        switch (moduleType) {
            case BattleCultivationModuleType.Martial:
                return "武";
            case BattleCultivationModuleType.Spell:
                return "法";
            case BattleCultivationModuleType.Wealth:
                return "财";
            case BattleCultivationModuleType.Companion:
                return "侣";
            case BattleCultivationModuleType.Territory:
                return "地";
            default:
                return "-";
        }
    }

    public static string ResolveStyleLabel(BattleParticipantSnapshot participant) {
        if (participant == null) {
            return "未定风格";
        }

        string profession = participant.professionName;
        BattleCultivationModuleType primary = participant.buildProfile == null
            ? BattleCultivationModuleType.None
            : participant.buildProfile.preferredPrimaryModule;
        string moduleLabel = ResolveModuleLabel(primary);
        string positionLabel = ResolveFormationLabel(participant.preferredPosition);
        return Normalize(profession, "未定职业") + " / " + moduleLabel + "线 / " + positionLabel;
    }

    public static string BuildCarrySummary(BattleParticipantSnapshot participant) {
        if (participant == null || participant.buildProfile == null || participant.buildProfile.carriedSlots == null) {
            return "承载未成型";
        }

        BattleBuildSlotSnapshot bestSlot = null;
        for (int i = 0; i < participant.buildProfile.carriedSlots.Length; i++) {
            BattleBuildSlotSnapshot slot = participant.buildProfile.carriedSlots[i];
            if (slot == null || string.IsNullOrWhiteSpace(slot.entryDisplayName) || slot.entryDisplayName == "空") {
                continue;
            }

            if (bestSlot == null || slot.battlePowerBonus > bestSlot.battlePowerBonus) {
                bestSlot = slot;
            }
        }

        if (bestSlot == null) {
            return "承载未成型";
        }

        return ResolveModuleLabel(bestSlot.moduleType) + "线承载：" +
            Normalize(bestSlot.slotDisplayName, bestSlot.slotId) + " -> " +
            Normalize(bestSlot.entryDisplayName, "空");
    }

    public static string BuildCarryTransitionPreview(BattleParticipantSnapshot participant) {
        if (participant == null || string.IsNullOrWhiteSpace(participant.selectedCultivationSlotId)) {
            return "本轮未锁定承载变化";
        }

        BattleBuildSlotSnapshot currentSlot = FindSlot(participant, participant.selectedCultivationSlotId);
        string currentEntry = currentSlot == null ? "空位" : Normalize(currentSlot.entryDisplayName, "空位");
        string nextEntry = Normalize(
            string.IsNullOrWhiteSpace(participant.selectedCarryTitle)
                ? participant.selectedCultivationResolvedEntryName
                : participant.selectedCarryTitle,
            "未定");
        string slotLabel = Normalize(
            string.IsNullOrWhiteSpace(participant.selectedCarrySlotLabel)
                ? participant.selectedCultivationSlotDisplayName
                : participant.selectedCarrySlotLabel,
            participant.selectedCultivationSlotId);
        string action = currentSlot == null || string.IsNullOrWhiteSpace(currentSlot.entryId)
            ? "新承载"
            : "替换承载";

        return action + "：" + slotLabel + " 由 " + currentEntry + " -> " + nextEntry;
    }

    public static int ResolveCarryBattlePower(BattleParticipantSnapshot participant) {
        if (participant == null || participant.buildProfile == null || participant.buildProfile.carriedSlots == null) {
            return 0;
        }

        int total = 0;
        for (int i = 0; i < participant.buildProfile.carriedSlots.Length; i++) {
            BattleBuildSlotSnapshot slot = participant.buildProfile.carriedSlots[i];
            if (slot == null || string.IsNullOrWhiteSpace(slot.entryId)) {
                continue;
            }

            total += Math.Max(0, slot.battlePowerBonus);
            if (slot.moduleType == participant.buildProfile.preferredPrimaryModule) {
                total += 2;
            }
        }

        return total;
    }

    public static int ResolveProfessionBattleBonus(BattleParticipantSnapshot participant) {
        if (participant == null) {
            return 0;
        }

        string professionId = Normalize(participant.heroProfessionId, participant.buildProfile == null ? string.Empty : participant.buildProfile.professionId);
        switch (professionId) {
            case "profession_sword":
                return participant.swordIntentValue * 2;
            case "profession_spell":
                return participant.spellGrowthTotal * 2 + participant.spellValue;
            case "profession_body":
                return participant.physicalGrowthTotal * 2 + participant.heroCombatBonus;
            case "profession_array":
                return participant.territoryGrowthTotal * 2;
            case "profession_support":
                return participant.companionGrowthTotal * 2 + participant.wealthGrowthTotal;
            default:
                return participant.swordIntentValue;
        }
    }

    public static int ResolveSlotFocusScore(BattleParticipantSnapshot participant, BattleCultivationModuleType moduleType) {
        if (participant == null || participant.buildProfile == null) {
            return 0;
        }

        return participant.buildProfile.GetModulePower(moduleType);
    }

    public static string BuildResultReasonSummary(
        BattleParticipantSnapshot participant,
        BattleParticipantSnapshot opponent,
        BattleRoundFormationSnapshot formationSnapshot,
        BattleA2SectEnvironmentSnapshot environment,
        BattleBattleOutcomeType outcome) {
        if (participant == null) {
            return string.Empty;
        }

        StringBuilder builder = new StringBuilder(96);
        bool hitEye = formationSnapshot != null && participant.currentFormationPosition == formationSnapshot.eyePosition;
        if (hitEye) {
            builder.Append("命中阵眼");
        } else if (participant.currentFormationPosition == participant.preferredPosition) {
            builder.Append("站位契合");
        } else {
            builder.Append("站位平稳");
        }

        string carry = BuildCarrySummary(participant);
        if (!string.IsNullOrWhiteSpace(carry) && carry != "承载未成型") {
            builder.Append(" / ").Append(carry);
        }

        if (participant.lastGrowthBattlePowerGain > 0 || participant.lastGrowthLuckGain > 0) {
            builder.Append(" / 本轮养成已生效");
        }

        if (outcome == BattleBattleOutcomeType.Win) {
            builder.Append(" / 压制对手");
        } else if (outcome == BattleBattleOutcomeType.Lose && opponent != null) {
            builder.Append(" / 被 ").Append(Normalize(opponent.displayName, opponent.playerId)).Append(" 反制");
        } else if (outcome == BattleBattleOutcomeType.Bye) {
            builder.Append(" / 轮空保留节奏");
        }

        if (environment != null && !string.IsNullOrWhiteSpace(environment.battleModifierTag)) {
            builder.Append(" / 环境：").Append(environment.battleModifierTag);
        }

        return builder.ToString();
    }

    public static string Normalize(string value, string fallback) {
        return string.IsNullOrWhiteSpace(value) ? fallback : value;
    }

    private static BattleBuildSlotSnapshot FindSlot(BattleParticipantSnapshot participant, string slotId) {
        if (participant == null || participant.buildProfile == null || participant.buildProfile.carriedSlots == null) {
            return null;
        }

        for (int i = 0; i < participant.buildProfile.carriedSlots.Length; i++) {
            BattleBuildSlotSnapshot slot = participant.buildProfile.carriedSlots[i];
            if (slot != null && string.Equals(slot.slotId, slotId, StringComparison.Ordinal)) {
                return slot;
            }
        }

        return null;
    }

    private static BattleBuildSlotSnapshot CreateSlot(string slotId, string slotDisplayName, BattleCultivationModuleType moduleType) {
        return new BattleBuildSlotSnapshot {
            slotId = slotId,
            slotDisplayName = slotDisplayName,
            moduleType = moduleType,
            entryId = string.Empty,
            entryDisplayName = "空",
            summary = "尚未承载",
            tier = 0,
            battlePowerBonus = 0
        };
    }

    private static BattleBuildSlotSnapshot[] CloneDefaultSlots() {
        BattleBuildSlotSnapshot[] slots = new BattleBuildSlotSnapshot[DEFAULT_SLOT_TEMPLATES.Length];
        for (int i = 0; i < DEFAULT_SLOT_TEMPLATES.Length; i++) {
            slots[i] = DEFAULT_SLOT_TEMPLATES[i].Clone();
        }

        return slots;
    }

    private static BattleCultivationModuleType ResolvePrimaryModule(BattleA2HeroCandidateSnapshot candidate) {
        if (candidate == null) {
            return BattleCultivationModuleType.Martial;
        }

        int martial = candidate.martialBias;
        int spell = candidate.spellBias;
        int wealth = candidate.wealthBias;
        int companion = candidate.companionBias;
        int territory = candidate.territoryBias;
        int best = martial;
        BattleCultivationModuleType moduleType = BattleCultivationModuleType.Martial;
        if (spell > best) {
            best = spell;
            moduleType = BattleCultivationModuleType.Spell;
        }

        if (wealth > best) {
            best = wealth;
            moduleType = BattleCultivationModuleType.Wealth;
        }

        if (companion > best) {
            best = companion;
            moduleType = BattleCultivationModuleType.Companion;
        }

        if (territory > best) {
            moduleType = BattleCultivationModuleType.Territory;
        }

        return moduleType;
    }

    private static BattleCultivationModuleType ResolveSecondaryModule(BattleA2HeroCandidateSnapshot candidate) {
        if (candidate == null) {
            return BattleCultivationModuleType.Spell;
        }

        int[] values = {
            candidate.martialBias,
            candidate.spellBias,
            candidate.wealthBias,
            candidate.companionBias,
            candidate.territoryBias
        };
        BattleCultivationModuleType[] types = {
            BattleCultivationModuleType.Martial,
            BattleCultivationModuleType.Spell,
            BattleCultivationModuleType.Wealth,
            BattleCultivationModuleType.Companion,
            BattleCultivationModuleType.Territory
        };
        int bestIndex = 0;
        int secondIndex = 1;
        if (values[1] > values[0]) {
            bestIndex = 1;
            secondIndex = 0;
        }

        for (int i = 2; i < values.Length; i++) {
            if (values[i] > values[bestIndex]) {
                secondIndex = bestIndex;
                bestIndex = i;
            } else if (values[i] > values[secondIndex]) {
                secondIndex = i;
            }
        }

        return types[secondIndex];
    }
}
