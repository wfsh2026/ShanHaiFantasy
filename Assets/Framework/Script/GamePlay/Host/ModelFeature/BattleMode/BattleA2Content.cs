using System;
using UnityEngine;

public static class BattleA2ContentCatalog {
    private const int DefaultHeroCount = 3;
    private const int DefaultOptionCount = 4;

    private static readonly BattleA2HeroCandidateSnapshot[] HeroPool = {
        Hero("hero_combo", "剑修·连斩", "云崖剑宗", 86, "连斩", "稳定压制与前位对拼。", "profession_sword", "剑修", 4, BattleFormationPositionType.Front, 4, 1, 0, 0, 1),
        Hero("hero_flight", "剑修·御剑", "天穹剑台", 82, "御剑", "中后位机动与法脉推进。", "profession_sword", "剑修", 5, BattleFormationPositionType.Middle, 1, 4, 0, 0, 2),
        Hero("hero_execute", "剑修·绝杀", "绝峰剑窟", 90, "绝杀", "前位爆发和斩杀窗口。", "profession_sword", "剑修", 6, BattleFormationPositionType.Front, 5, 0, 0, 0, 1),
        Hero("hero_break", "阵修·破阵", "阵锋天宫", 84, "破阵", "依赖阵眼与地线收益。", "profession_array", "阵修", 3, BattleFormationPositionType.Middle, 1, 1, 0, 0, 5),
        Hero("hero_guard", "体修·归岚", "守一山门", 83, "归岚", "前中位硬抗与持续压迫。", "profession_body", "体修", 2, BattleFormationPositionType.Front, 4, 0, 1, 0, 2),
        Hero("hero_spirit", "灵修·灵契", "灵侍谷", 80, "灵契", "依靠灵侣协同和后位续航。", "profession_support", "灵修", 3, BattleFormationPositionType.Rear, 1, 2, 0, 4, 1),
        Hero("hero_merchant", "商修·藏锋", "流金宝阁", 79, "藏锋", "资源转战力，慢热滚雪球。", "profession_support", "商修", 2, BattleFormationPositionType.Rear, 0, 1, 4, 1, 1),
        Hero("hero_spell", "法修·惊雷", "玄霄法坛", 85, "惊雷", "法脉爆发与后位成型。", "profession_spell", "法修", 4, BattleFormationPositionType.Rear, 0, 5, 0, 0, 1),
        Hero("hero_mist", "法修·雾心", "雾岚峰", 81, "雾心", "中位控场与持续法压。", "profession_spell", "法修", 5, BattleFormationPositionType.Middle, 0, 4, 1, 0, 2),
        Hero("hero_partner", "灵修·双生", "青羽灵门", 82, "双生", "重视灵侣与阵位联动。", "profession_support", "灵修", 4, BattleFormationPositionType.Middle, 1, 1, 0, 5, 1),
        Hero("hero_array", "阵修·观星", "星罗阵府", 84, "观星", "阵眼命中时能迅速拉开差距。", "profession_array", "阵修", 3, BattleFormationPositionType.Middle, 0, 2, 1, 0, 5),
        Hero("hero_body", "体修·镇岳", "山海重门", 88, "镇岳", "前位吃伤反压，越打越稳。", "profession_body", "体修", 1, BattleFormationPositionType.Front, 5, 0, 1, 0, 1)
    };

    private static readonly BattleA2SectEnvironmentSnapshot[] EnvPool = {
        Env("sect_mist_peak", "雾岚峰", "稳守型环境，命中中位阵眼更容易滚起优势。", "稳守"),
        Env("sect_sunless_valley", "无光谷", "低视野环境，前位抢攻更容易拉开差距。", "抢攻"),
        Env("sect_bamboo_yard", "青竹院", "平衡环境，适合养成后再发力。", "平衡"),
        Env("sect_thunder_ring", "惊雷环", "高压节奏环境，后位机动和御剑收益更高。", "机动")
    };

    private static readonly BattleCultivationOptionSnapshot[] BaseOptions = {
        Opt("opt_weapon", "青锋古剑", "强化兵器槽与正面对拼。", "基础", BattleCultivationModuleType.Martial, "martial_weapon", "兵器槽", BattleFormationPositionType.Front, 2, 0, 0, 0, 0, 0, 3, 1,
            Sub("sub_armor", "玄鳞护衣", "补足护具与气运缓冲。", "护具槽", "玄鳞护衣", BattleFormationPositionType.Front, 1, 0, 0, 0, 0, 1, 1, 0)),
        Opt("opt_artifact", "回响剑匣", "法宝与持续输出推进。", "标准", BattleCultivationModuleType.Martial, "martial_artifact", "法宝槽", BattleFormationPositionType.Middle, 1, 1, 0, 0, 0, 0, 2, 1,
            Sub("sub_body", "雷火炼体", "补炼体位，提高前线抗压。", "炼体位", "雷火炼体", BattleFormationPositionType.Front, 2, 0, 0, 0, 0, 0, 1, 0)),
        Opt("opt_main_manual", "青莲剑诀", "主功法成型，偏法脉推进。", "职业适配", BattleCultivationModuleType.Spell, "spell_main", "主功法", BattleFormationPositionType.Rear, 0, 3, 0, 0, 0, 0, 2, 1,
            Sub("sub_heart", "太虚心法", "补心法，稳住气机。", "心法槽", "太虚心法", BattleFormationPositionType.Middle, 0, 1, 0, 0, 0, 1, 1, 0)),
        Opt("opt_divine", "破阵神通", "围绕阵眼和站位收益发力。", "职业适配", BattleCultivationModuleType.Spell, "spell_art", "神通位", BattleFormationPositionType.Middle, 0, 2, 0, 0, 1, 0, 2, 0,
            Sub("sub_eye", "阵眼推演", "补阵眼理解和地脉收益。", "阵眼槽", "阵眼推演", BattleFormationPositionType.Middle, 0, 0, 0, 0, 2, 1, 1, 0)),
        Opt("opt_cave", "灵泉洞府", "稳定气运与后续成长空间。", "标准", BattleCultivationModuleType.Wealth, "wealth_estate", "洞府槽", BattleFormationPositionType.Middle, 0, 0, 2, 0, 0, 1, 1, 0,
            Sub("sub_store", "流光宝阁", "补宝阁流转效率。", "宝阁槽", "流光宝阁", BattleFormationPositionType.Rear, 0, 0, 1, 0, 0, 0, 1, 0)),
        Opt("opt_task", "宗门委托", "把财脉转成战力准备度。", "标准", BattleCultivationModuleType.Wealth, "wealth_mission", "委托槽", BattleFormationPositionType.Middle, 0, 0, 2, 0, 0, 1, 2, 0,
            Sub("sub_companion", "青鸾灵宠", "补轻量资源辅助。", "侣槽", "青鸾灵宠", BattleFormationPositionType.Rear, 0, 0, 0, 1, 0, 1, 0, 0)),
        Opt("opt_spirit", "剑灵契约", "强化剑意恢复与战斗辅助。", "本命", BattleCultivationModuleType.Companion, "companion_support", "灵侣槽", BattleFormationPositionType.Middle, 1, 0, 0, 3, 0, 0, 2, 2,
            Sub("sub_back", "流云后位", "补后位站位理解。", "站位槽", "流云后位", BattleFormationPositionType.Rear, 0, 1, 0, 0, 1, 0, 1, 0)),
        Opt("opt_array", "裂锋站位", "围绕前位阵眼打出爆发。", "本命", BattleCultivationModuleType.Territory, "territory_array", "阵法槽", BattleFormationPositionType.Front, 1, 0, 0, 0, 3, 0, 2, 1,
            Sub("sub_partner", "护阵道侣", "补稳定辅助。", "侣槽", "护阵道侣", BattleFormationPositionType.Middle, 0, 0, 0, 2, 1, 1, 0, 0)),
        Opt("opt_thunder", "惊雷法相", "把法脉收益转成瞬时爆发。", "强化", BattleCultivationModuleType.Spell, "spell_art", "神通位", BattleFormationPositionType.Rear, 0, 4, 0, 0, 0, 0, 3, 1,
            Sub("sub_thunder_heart", "雷纹心法", "补心法并提升命中阵眼后的收益。", "心法槽", "雷纹心法", BattleFormationPositionType.Middle, 0, 2, 0, 0, 1, 0, 1, 0)),
        Opt("opt_guard", "镇岳体魄", "炼体成型，稳住前线交换。", "强化", BattleCultivationModuleType.Martial, "martial_body", "炼体位", BattleFormationPositionType.Front, 3, 0, 0, 0, 0, 1, 2, 0,
            Sub("sub_guard_armor", "沉岳护具", "补足护具槽，提升抗压。", "护具槽", "沉岳护具", BattleFormationPositionType.Front, 1, 0, 0, 0, 0, 1, 1, 0)),
        Opt("opt_familiar", "双生灵契", "把灵侣线转成站位与续战优势。", "强化", BattleCultivationModuleType.Companion, "companion_role", "灵宠位", BattleFormationPositionType.Rear, 0, 1, 0, 4, 0, 1, 1, 1,
            Sub("sub_familiar_array", "灵契阵位", "让灵侣线更容易吃到阵眼收益。", "阵位槽", "灵契阵位", BattleFormationPositionType.Middle, 0, 0, 0, 1, 2, 0, 1, 0)),
        Opt("opt_star", "观星阵图", "强化地线与阵眼联动。", "强化", BattleCultivationModuleType.Territory, "territory_eye", "阵眼槽", BattleFormationPositionType.Middle, 0, 1, 0, 0, 4, 0, 2, 0,
            Sub("sub_star_pos", "星罗站位", "把站位偏好锁向阵眼。", "站位槽", "星罗站位", BattleFormationPositionType.Middle, 0, 0, 0, 0, 1, 1, 1, 0)),
        Opt("opt_trade", "行商宝录", "用财线滚动战力与气运。", "强化", BattleCultivationModuleType.Wealth, "wealth_vault", "宝阁槽", BattleFormationPositionType.Rear, 0, 0, 3, 0, 0, 2, 2, 0,
            Sub("sub_trade_partner", "外援客卿", "把财脉收益分给灵侣线。", "客卿槽", "外援客卿", BattleFormationPositionType.Middle, 0, 0, 1, 2, 0, 0, 1, 0))
    };

    public static BattleA2HeroCandidateSnapshot[] BuildHeroCandidates(int slotIndex, int roundIndex, int candidateCount = DefaultHeroCount) {
        if (candidateCount <= 0) {
            candidateCount = DefaultHeroCount;
        }

        BattleA2HeroCandidateSnapshot[] result = new BattleA2HeroCandidateSnapshot[candidateCount];
        int baseIndex = Mathf.Abs(roundIndex * 17 + slotIndex * 7) % HeroPool.Length;
        for (int i = 0; i < candidateCount; i++) {
            result[i] = HeroPool[(baseIndex + i * 3) % HeroPool.Length].Clone();
        }

        return result;
    }

    public static int ResolveAutoSelectionIndex(int slotIndex, int roundIndex, int candidateCount = DefaultHeroCount) {
        return candidateCount <= 0 ? 0 : Mathf.Abs(roundIndex * 13 + slotIndex * 5) % candidateCount;
    }

    public static BattleA2SectEnvironmentSnapshot BuildSectEnvironment(int roundIndex, int aliveCount) {
        BattleA2SectEnvironmentSnapshot env = EnvPool[Mathf.Abs(roundIndex + aliveCount) % EnvPool.Length].Clone();
        env.roundIndex = roundIndex;
        env.aliveCount = aliveCount;
        env.arrayEyePosition = ResolveArrayEyePosition(roundIndex, aliveCount);
        env.arrayEyeLabel = GetPositionLabel(env.arrayEyePosition);
        env.arrayEyeSummary = "本轮阵眼位于" + env.arrayEyeLabel + "，命中可获得额外收益。";
        return env;
    }

    public static int ResolveInitialLuck(int slotIndex) {
        return 6 + (Mathf.Abs(slotIndex) % 2);
    }

    public static BattleCultivationOptionSnapshot[] BuildCultivationOptions(BattleParticipantSnapshot participant, int roundIndex) {
        return BuildCultivationOptions(participant, roundIndex, null);
    }

    public static BattleCultivationOptionSnapshot[] BuildCultivationOptions(
        BattleParticipantSnapshot participant,
        int roundIndex,
        BattleA2SectEnvironmentSnapshot environment) {
        WeightedOption[] weighted = new WeightedOption[BaseOptions.Length];
        for (int i = 0; i < BaseOptions.Length; i++) {
            BattleCultivationOptionSnapshot option = BaseOptions[i].Clone();
            option.optionId = option.optionId + "_" + roundIndex + "_" + i;
            option.recommendedPosition = ResolveRecommendedPosition(participant, environment, option);
            option.weightTag = "权重 " + ScoreCultivationOption(participant, roundIndex, option);
            weighted[i] = new WeightedOption(option, ScoreCultivationOption(participant, roundIndex, option));
        }

        Array.Sort(weighted, (a, b) => b.score.CompareTo(a.score));
        BattleCultivationOptionSnapshot[] picks = new BattleCultivationOptionSnapshot[DefaultOptionCount];
        int next = TryPick(weighted, picks, 0, option =>
            option.moduleType == BattleCultivationModuleType.Martial ||
            option.moduleType == BattleCultivationModuleType.Spell,
            default(BattleCultivationModuleType));
        next = TryPick(weighted, picks, next, option =>
            option.moduleType == BattleCultivationModuleType.Wealth ||
            option.moduleType == BattleCultivationModuleType.Companion ||
            option.moduleType == BattleCultivationModuleType.Territory,
            default(BattleCultivationModuleType));
        BattleCultivationModuleType preferred = ResolvePreferredModule(participant);
        next = TryPick(weighted, picks, next, option => option.moduleType == preferred, preferred);
        TryPick(weighted, picks, next, null, default(BattleCultivationModuleType));
        return picks;
    }

    public static int ResolveAutoCultivationSelectionIndex(
        BattleParticipantSnapshot participant,
        int roundIndex,
        BattleCultivationOptionSnapshot[] options) {
        if (options == null || options.Length == 0) {
            return 0;
        }

        int bestIndex = 0;
        int bestScore = int.MinValue;
        for (int i = 0; i < options.Length; i++) {
            BattleCultivationOptionSnapshot option = options[i];
            if (option == null) {
                continue;
            }

            int score = ScoreCultivationOption(participant, roundIndex, option) +
                ResolveSecondaryScore(participant, roundIndex, option);
            if (score > bestScore) {
                bestScore = score;
                bestIndex = i;
            }
        }

        return bestIndex;
    }

    public static int ResolveAutoSecondarySelectionIndex(
        BattleParticipantSnapshot participant,
        int roundIndex,
        BattleCultivationOptionSnapshot option) {
        if (option == null || option.secondaryOptions == null || option.secondaryOptions.Length == 0) {
            return -1;
        }

        int bestIndex = 0;
        int bestScore = int.MinValue;
        for (int i = 0; i < option.secondaryOptions.Length; i++) {
            BattleCultivationSubOptionSnapshot subOption = option.secondaryOptions[i];
            if (subOption == null) {
                continue;
            }

            int score = ScoreSubOption(participant, roundIndex, option, subOption);
            if (score > bestScore) {
                bestScore = score;
                bestIndex = i;
            }
        }

        return bestIndex;
    }

    public static BattleFormationPositionType ResolveAutoFormationPosition(
        BattleParticipantSnapshot participant,
        int roundIndex,
        BattleA2SectEnvironmentSnapshot environment) {
        if (participant == null) {
            return BattleFormationPositionType.Middle;
        }

        if (participant.selectedRecommendedPosition != BattleFormationPositionType.None) {
            return participant.selectedRecommendedPosition;
        }

        if (environment != null &&
            participant.territoryGrowthTotal >= participant.physicalGrowthTotal &&
            participant.territoryGrowthTotal >= participant.spellGrowthTotal) {
            return environment.arrayEyePosition;
        }

        return participant.preferredPosition != BattleFormationPositionType.None
            ? participant.preferredPosition
            : ResolveArrayEyePosition(roundIndex, participant.slotIndex);
    }

    public static int ResolveHeroPower(BattleParticipantSnapshot participant) {
        if (participant == null) {
            return 0;
        }

        if (participant.heroCandidates == null || participant.heroCandidates.Length == 0) {
            return 60;
        }

        if (participant.selectedCandidateIndex >= 0 &&
            participant.selectedCandidateIndex < participant.heroCandidates.Length &&
            participant.heroCandidates[participant.selectedCandidateIndex] != null) {
            return participant.heroCandidates[participant.selectedCandidateIndex].powerScore;
        }

        for (int i = 0; i < participant.heroCandidates.Length; i++) {
            BattleA2HeroCandidateSnapshot candidate = participant.heroCandidates[i];
            if (candidate != null &&
                string.Equals(candidate.heroId ?? string.Empty, participant.selectedHeroId ?? string.Empty, StringComparison.Ordinal)) {
                return candidate.powerScore;
            }
        }

        return 60;
    }

    public static BattleRoundMatchSnapshot[] BuildBattleMatches(
        BattleParticipantSnapshot[] participants,
        int roundIndex,
        BattleA2SectEnvironmentSnapshot environment,
        BattleRoundFormationSnapshot formationSnapshot = null) {
        if (participants == null || participants.Length == 0) {
            return new BattleRoundMatchSnapshot[0];
        }

        int aliveCount = CountActive(participants);
        if (aliveCount <= 0) {
            return new BattleRoundMatchSnapshot[0];
        }

        BattleParticipantSnapshot[] active = new BattleParticipantSnapshot[aliveCount];
        int cursor = 0;
        for (int i = 0; i < participants.Length; i++) {
            if (participants[i] != null && !participants[i].isEliminated) {
                active[cursor++] = participants[i];
            }
        }

        BattleRoundMatchSnapshot[] matches = new BattleRoundMatchSnapshot[(aliveCount + 1) / 2];
        for (int i = 0; i < active.Length; i += 2) {
            matches[i / 2] = BuildMatch(
                active[i],
                i + 1 < active.Length ? active[i + 1] : null,
                roundIndex,
                i / 2,
                environment,
                formationSnapshot);
        }

        return matches;
    }

    public static int ResolveBattleScore(
        BattleParticipantSnapshot participant,
        int roundIndex,
        BattleA2SectEnvironmentSnapshot environment) {
        if (participant == null) {
            return 0;
        }

        int luck = participant.luckValue * 3;
        int hero = ResolveHeroPower(participant);
        int growth =
            participant.physicalGrowthTotal * (ResolveBias(participant, BattleCultivationModuleType.Martial) + 2) +
            participant.spellGrowthTotal * (ResolveBias(participant, BattleCultivationModuleType.Spell) + 2) +
            participant.wealthGrowthTotal * (ResolveBias(participant, BattleCultivationModuleType.Wealth) + 1) +
            participant.companionGrowthTotal * (ResolveBias(participant, BattleCultivationModuleType.Companion) + 1) +
            participant.territoryGrowthTotal * (ResolveBias(participant, BattleCultivationModuleType.Territory) + 2) +
            participant.battlePowerTotal * 5;
        int carry = BattleBContentCatalog.ResolveCarryBattlePower(participant) * 2;
        int profession = BattleBContentCatalog.ResolveProfessionBattleBonus(participant);
        int environmentScore = ResolveEnvironmentBattleScore(participant, roundIndex, environment);
        int positionScore = participant.currentFormationPosition == participant.preferredPosition ? 6 : 2;
        if (environment != null && participant.currentFormationPosition == environment.arrayEyePosition) {
            positionScore += 6;
        }

        return hero + growth + luck + carry + profession + environmentScore + positionScore + roundIndex * 5;
    }

    public static string ResolveBattleSummary(BattleRoundMatchSnapshot match) {
        if (match == null) {
            return string.Empty;
        }

        if (match.isBye) {
            return Normalize(match.leftDisplayName, match.leftPlayerId) + " 轮空";
        }

        switch (match.leftOutcome) {
            case BattleBattleOutcomeType.Win:
                return Normalize(match.leftDisplayName, match.leftPlayerId) + " 胜 " +
                    Normalize(match.rightDisplayName, match.rightPlayerId);
            case BattleBattleOutcomeType.Lose:
                return Normalize(match.leftDisplayName, match.leftPlayerId) + " 负 " +
                    Normalize(match.rightDisplayName, match.rightPlayerId);
            case BattleBattleOutcomeType.Draw:
                return Normalize(match.leftDisplayName, match.leftPlayerId) + " 平 " +
                    Normalize(match.rightDisplayName, match.rightPlayerId);
            default:
                return "未决";
        }
    }

    private static BattleA2HeroCandidateSnapshot Hero(
        string id,
        string name,
        string sect,
        int power,
        string templateName,
        string summary,
        string professionId,
        string professionName,
        int swordIntentStart,
        BattleFormationPositionType preferred,
        int martial,
        int spell,
        int wealth,
        int companion,
        int territory) {
        return new BattleA2HeroCandidateSnapshot {
            heroId = id,
            displayName = name,
            sectTag = sect,
            powerScore = power,
            description = summary,
            professionId = professionId,
            professionName = professionName,
            templateId = templateName.ToLowerInvariant(),
            templateName = templateName,
            templateSummary = summary,
            swordIntentStart = swordIntentStart,
            preferredPosition = preferred,
            martialBias = martial,
            spellBias = spell,
            wealthBias = wealth,
            companionBias = companion,
            territoryBias = territory
        };
    }

    private static BattleA2SectEnvironmentSnapshot Env(string id, string name, string description, string tag) {
        return new BattleA2SectEnvironmentSnapshot {
            environmentId = id,
            displayName = name,
            description = description,
            battleModifierTag = tag,
            arrayEyePosition = BattleFormationPositionType.Middle,
            arrayEyeLabel = "中位",
            arrayEyeSummary = "等待本轮阵眼"
        };
    }

    private static BattleCultivationOptionSnapshot Opt(
        string id,
        string name,
        string description,
        string rarity,
        BattleCultivationModuleType module,
        string slotType,
        string carrySlot,
        BattleFormationPositionType recommended,
        int physical,
        int spell,
        int wealth,
        int companion,
        int territory,
        int luck,
        int power,
        int swordIntent,
        params BattleCultivationSubOptionSnapshot[] secondary) {
        return new BattleCultivationOptionSnapshot {
            optionId = id,
            displayName = name,
            description = description,
            rarityLabel = rarity,
            moduleType = module,
            slotType = slotType,
            carrySlotLabel = carrySlot,
            weightTag = string.Empty,
            recommendedPosition = recommended,
            physicalGain = physical,
            spellGain = spell,
            wealthGain = wealth,
            companionGain = companion,
            territoryGain = territory,
            luckGain = luck,
            battlePowerBonus = power,
            swordIntentGain = swordIntent,
            secondaryOptions = secondary ?? new BattleCultivationSubOptionSnapshot[0]
        };
    }

    private static BattleCultivationSubOptionSnapshot Sub(
        string id,
        string name,
        string description,
        string slotLabel,
        string carryTitle,
        BattleFormationPositionType recommended,
        int physical,
        int spell,
        int wealth,
        int companion,
        int territory,
        int luck,
        int power,
        int swordIntent) {
        return new BattleCultivationSubOptionSnapshot {
            subOptionId = id,
            displayName = name,
            description = description,
            recommendedPosition = recommended,
            carrySlotLabel = slotLabel,
            carryTitle = carryTitle,
            physicalGain = physical,
            spellGain = spell,
            wealthGain = wealth,
            companionGain = companion,
            territoryGain = territory,
            luckGain = luck,
            battlePowerBonus = power,
            swordIntentGain = swordIntent
        };
    }

    private static BattleFormationPositionType ResolveArrayEyePosition(int roundIndex, int seed) {
        int value = Mathf.Abs(roundIndex * 7 + seed * 3) % 3;
        return value == 0
            ? BattleFormationPositionType.Front
            : (value == 1 ? BattleFormationPositionType.Middle : BattleFormationPositionType.Rear);
    }

    private static string GetPositionLabel(BattleFormationPositionType position) {
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

    private static BattleCultivationModuleType ResolvePreferredModule(BattleParticipantSnapshot participant) {
        if (participant == null) {
            return BattleCultivationModuleType.Martial;
        }

        int martial = ResolveBias(participant, BattleCultivationModuleType.Martial) + participant.physicalGrowthTotal;
        int spell = ResolveBias(participant, BattleCultivationModuleType.Spell) + participant.spellGrowthTotal;
        int wealth = ResolveBias(participant, BattleCultivationModuleType.Wealth) + participant.wealthGrowthTotal;
        int companion = ResolveBias(participant, BattleCultivationModuleType.Companion) + participant.companionGrowthTotal;
        int territory = ResolveBias(participant, BattleCultivationModuleType.Territory) + participant.territoryGrowthTotal;
        if (spell >= martial && spell >= wealth && spell >= companion && spell >= territory) {
            return BattleCultivationModuleType.Spell;
        }

        if (territory >= wealth && territory >= companion) {
            return BattleCultivationModuleType.Territory;
        }

        if (companion >= wealth) {
            return BattleCultivationModuleType.Companion;
        }

        if (wealth >= martial) {
            return BattleCultivationModuleType.Wealth;
        }

        return BattleCultivationModuleType.Martial;
    }

    private static BattleFormationPositionType ResolveRecommendedPosition(
        BattleParticipantSnapshot participant,
        BattleA2SectEnvironmentSnapshot environment,
        BattleCultivationOptionSnapshot option) {
        if (option == null) {
            return BattleFormationPositionType.Middle;
        }

        if (option.recommendedPosition != BattleFormationPositionType.None) {
            return option.recommendedPosition;
        }

        if (environment != null && option.moduleType == BattleCultivationModuleType.Territory) {
            return environment.arrayEyePosition;
        }

        return participant == null ? BattleFormationPositionType.Middle : participant.preferredPosition;
    }

    private static int ScoreCultivationOption(
        BattleParticipantSnapshot participant,
        int roundIndex,
        BattleCultivationOptionSnapshot option) {
        if (option == null) {
            return int.MinValue;
        }

        int score = option.battlePowerBonus * 5 +
            option.luckGain * 4 +
            option.swordIntentGain * 3 +
            roundIndex;
        if (participant != null) {
            score += option.physicalGain * (ResolveBias(participant, BattleCultivationModuleType.Martial) + 1);
            score += option.spellGain * (ResolveBias(participant, BattleCultivationModuleType.Spell) + 1);
            score += option.wealthGain * (ResolveBias(participant, BattleCultivationModuleType.Wealth) + 1);
            score += option.companionGain * (ResolveBias(participant, BattleCultivationModuleType.Companion) + 1);
            score += option.territoryGain * (ResolveBias(participant, BattleCultivationModuleType.Territory) + 1);
            if (option.recommendedPosition == participant.preferredPosition) {
                score += 4;
            }

            score += ResolveProfessionOptionBonus(participant, option);
            if (participant.buildProfile != null &&
                option.moduleType == participant.buildProfile.preferredPrimaryModule) {
                score += 5;
            }
        }

        if (option.moduleType == BattleCultivationModuleType.Martial ||
            option.moduleType == BattleCultivationModuleType.Spell) {
            score += 2;
        }

        return score;
    }

    private static int ResolveSecondaryScore(
        BattleParticipantSnapshot participant,
        int roundIndex,
        BattleCultivationOptionSnapshot option) {
        int pick = ResolveAutoSecondarySelectionIndex(participant, roundIndex, option);
        if (pick < 0 || option == null || option.secondaryOptions == null || pick >= option.secondaryOptions.Length || option.secondaryOptions[pick] == null) {
            return 0;
        }

        return ScoreSubOption(participant, roundIndex, option, option.secondaryOptions[pick]);
    }

    private static int ScoreSubOption(
        BattleParticipantSnapshot participant,
        int roundIndex,
        BattleCultivationOptionSnapshot option,
        BattleCultivationSubOptionSnapshot subOption) {
        if (subOption == null) {
            return int.MinValue;
        }

        int score = subOption.battlePowerBonus * 4 +
            subOption.luckGain * 3 +
            subOption.swordIntentGain * 2 +
            roundIndex;
        if (participant != null) {
            score += subOption.physicalGain * (ResolveBias(participant, BattleCultivationModuleType.Martial) + 1);
            score += subOption.spellGain * (ResolveBias(participant, BattleCultivationModuleType.Spell) + 1);
            score += subOption.wealthGain * (ResolveBias(participant, BattleCultivationModuleType.Wealth) + 1);
            score += subOption.companionGain * (ResolveBias(participant, BattleCultivationModuleType.Companion) + 1);
            score += subOption.territoryGain * (ResolveBias(participant, BattleCultivationModuleType.Territory) + 1);
            if (subOption.recommendedPosition == participant.preferredPosition) {
                score += 3;
            }
        }

        if (option != null && subOption.recommendedPosition == option.recommendedPosition) {
            score += 2;
        }

        return score;
    }

    private static int TryPick(
        WeightedOption[] weighted,
        BattleCultivationOptionSnapshot[] picks,
        int startIndex,
        Predicate<BattleCultivationOptionSnapshot> predicate,
        BattleCultivationModuleType target) {
        int index = startIndex;
        for (int i = 0; i < weighted.Length && index < picks.Length; i++) {
            BattleCultivationOptionSnapshot option = weighted[i].option;
            if (option == null || ContainsOption(picks, index, option.optionId)) {
                continue;
            }

            if (predicate != null && !predicate(option)) {
                continue;
            }

            if (target != default(BattleCultivationModuleType) &&
                CountModule(picks, index, target) >= 2 &&
                option.moduleType == target) {
                continue;
            }

            picks[index++] = option.Clone();
        }

        for (int i = 0; i < weighted.Length && index < picks.Length; i++) {
            BattleCultivationOptionSnapshot option = weighted[i].option;
            if (option == null || ContainsOption(picks, index, option.optionId)) {
                continue;
            }

            if (CountModule(picks, index, option.moduleType) >= 2) {
                continue;
            }

            picks[index++] = option.Clone();
        }

        return index;
    }

    private static bool ContainsOption(BattleCultivationOptionSnapshot[] picks, int length, string optionId) {
        for (int i = 0; i < length; i++) {
            if (picks[i] != null && string.Equals(picks[i].optionId, optionId, StringComparison.Ordinal)) {
                return true;
            }
        }

        return false;
    }

    private static int CountModule(BattleCultivationOptionSnapshot[] picks, int length, BattleCultivationModuleType module) {
        int count = 0;
        for (int i = 0; i < length; i++) {
            if (picks[i] != null && picks[i].moduleType == module) {
                count++;
            }
        }

        return count;
    }

    private static BattleRoundMatchSnapshot BuildMatch(
        BattleParticipantSnapshot left,
        BattleParticipantSnapshot right,
        int roundIndex,
        int matchIndex,
        BattleA2SectEnvironmentSnapshot environment,
        BattleRoundFormationSnapshot formationSnapshot) {
        BattleFormationPositionType eye = formationSnapshot == null
            ? (environment == null ? BattleFormationPositionType.None : environment.arrayEyePosition)
            : formationSnapshot.eyePosition;
        string eyeLabel = formationSnapshot == null
            ? (environment == null ? "无阵眼" : environment.arrayEyeLabel)
            : BattleBContentCatalog.ResolveFormationLabel(eye);
        BattleRoundMatchSnapshot match = new BattleRoundMatchSnapshot {
            roundIndex = roundIndex,
            matchIndex = matchIndex,
            leftPlayerId = left == null ? string.Empty : left.playerId ?? string.Empty,
            leftDisplayName = left == null ? string.Empty : left.displayName ?? string.Empty,
            rightPlayerId = right == null ? string.Empty : right.playerId ?? string.Empty,
            rightDisplayName = right == null ? string.Empty : right.displayName ?? string.Empty,
            isBye = right == null,
            leftPosition = left == null ? BattleFormationPositionType.None : left.currentFormationPosition,
            rightPosition = right == null ? BattleFormationPositionType.None : right.currentFormationPosition,
            arrayEyePosition = eye,
            leftPositionLabel = GetPositionLabel(left == null ? BattleFormationPositionType.None : left.currentFormationPosition),
            rightPositionLabel = GetPositionLabel(right == null ? BattleFormationPositionType.None : right.currentFormationPosition),
            arrayEyeLabel = eyeLabel,
            leftOutcome = BattleBattleOutcomeType.Pending,
            rightOutcome = BattleBattleOutcomeType.Pending,
            leftScore = ResolveBattleScore(left, roundIndex, environment),
            rightScore = right == null ? 0 : ResolveBattleScore(right, roundIndex, environment),
            leftStyleSummary = BattleBContentCatalog.ResolveStyleLabel(left),
            rightStyleSummary = BattleBContentCatalog.ResolveStyleLabel(right),
            leftCarrySummary = BattleBContentCatalog.BuildCarrySummary(left),
            rightCarrySummary = BattleBContentCatalog.BuildCarrySummary(right),
            eyeSummary = eye == BattleFormationPositionType.None ? "本轮无阵眼" : "本轮阵眼位于" + eyeLabel
        };

        if (right == null) {
            match.leftOutcome = BattleBattleOutcomeType.Bye;
            match.leftReasonSummary = BattleBContentCatalog.BuildResultReasonSummary(left, null, formationSnapshot, environment, match.leftOutcome);
            match.summary = ResolveBattleSummary(match);
            return match;
        }

        if (match.leftScore == match.rightScore) {
            match.leftOutcome = BattleBattleOutcomeType.Draw;
            match.rightOutcome = BattleBattleOutcomeType.Draw;
            match.leftLuckDelta = -1;
            match.rightLuckDelta = -1;
        } else if (match.leftScore > match.rightScore) {
            match.leftOutcome = BattleBattleOutcomeType.Win;
            match.rightOutcome = BattleBattleOutcomeType.Lose;
            match.leftLuckDelta = left != null && left.currentFormationPosition == eye ? 0 : -1;
            match.rightLuckDelta = -2;
        } else {
            match.leftOutcome = BattleBattleOutcomeType.Lose;
            match.rightOutcome = BattleBattleOutcomeType.Win;
            match.leftLuckDelta = -2;
            match.rightLuckDelta = right != null && right.currentFormationPosition == eye ? 0 : -1;
        }

        match.leftReasonSummary = BattleBContentCatalog.BuildResultReasonSummary(left, right, formationSnapshot, environment, match.leftOutcome);
        match.rightReasonSummary = BattleBContentCatalog.BuildResultReasonSummary(right, left, formationSnapshot, environment, match.rightOutcome);
        match.summary = ResolveBattleSummary(match);
        return match;
    }

    private static int ResolveEnvironmentBattleScore(
        BattleParticipantSnapshot participant,
        int roundIndex,
        BattleA2SectEnvironmentSnapshot environment) {
        if (environment == null || participant == null) {
            return 0;
        }

        int score = (Mathf.Abs(environment.roundIndex * 17 + environment.aliveCount * 13 + roundIndex * 7 + participant.slotIndex * 11) % 7) - 3;
        if (environment.environmentId == "sect_thunder_ring" && participant.currentFormationPosition == BattleFormationPositionType.Rear) {
            score += 4;
        }

        if (environment.environmentId == "sect_sunless_valley" && participant.currentFormationPosition == BattleFormationPositionType.Front) {
            score += 4;
        }

        if (environment.environmentId == "sect_mist_peak" && participant.currentFormationPosition == BattleFormationPositionType.Middle) {
            score += 3;
        }

        return score;
    }

    private static int ResolveBias(BattleParticipantSnapshot participant, BattleCultivationModuleType module) {
        if (participant == null || participant.buildProfile == null) {
            return module == BattleCultivationModuleType.Martial ? 1 : 0;
        }

        if (participant.buildProfile.preferredPrimaryModule == module) {
            return 4;
        }

        if (participant.buildProfile.preferredSecondaryModule == module) {
            return 2;
        }

        return 1;
    }

    private static int ResolveProfessionOptionBonus(BattleParticipantSnapshot participant, BattleCultivationOptionSnapshot option) {
        string professionId = ResolveProfessionId(participant);
        if (professionId == "profession_sword" && option.moduleType == BattleCultivationModuleType.Martial) {
            return 3;
        }

        if (professionId == "profession_spell" && option.moduleType == BattleCultivationModuleType.Spell) {
            return 4;
        }

        if (professionId == "profession_body" && option.moduleType == BattleCultivationModuleType.Martial) {
            return 4;
        }

        if (professionId == "profession_array" && option.moduleType == BattleCultivationModuleType.Territory) {
            return 5;
        }

        if (professionId == "profession_support" &&
            (option.moduleType == BattleCultivationModuleType.Companion || option.moduleType == BattleCultivationModuleType.Wealth)) {
            return 4;
        }

        return 0;
    }

    private static string ResolveProfessionId(BattleParticipantSnapshot participant) {
        return participant == null || participant.buildProfile == null
            ? string.Empty
            : participant.buildProfile.professionId ?? string.Empty;
    }

    private static int CountActive(BattleParticipantSnapshot[] participants) {
        int count = 0;
        for (int i = 0; i < participants.Length; i++) {
            if (participants[i] != null && !participants[i].isEliminated) {
                count++;
            }
        }

        return count;
    }

    private static string Normalize(string value, string fallback) {
        return string.IsNullOrWhiteSpace(value) ? fallback : value;
    }

    private readonly struct WeightedOption {
        public readonly BattleCultivationOptionSnapshot option;
        public readonly int score;

        public WeightedOption(BattleCultivationOptionSnapshot option, int score) {
            this.option = option;
            this.score = score;
        }
    }
}

[Serializable]
public sealed class BattleA2HeroCandidateSnapshot {
    public string heroId;
    public string displayName;
    public string sectTag;
    public int powerScore;
    public string description;
    public string professionId;
    public string professionName;
    public string templateId;
    public string templateName;
    public string templateSummary;
    public int swordIntentStart;
    public BattleFormationPositionType preferredPosition;
    public int martialBias;
    public int spellBias;
    public int wealthBias;
    public int companionBias;
    public int territoryBias;

    public BattleA2HeroCandidateSnapshot Clone() {
        return (BattleA2HeroCandidateSnapshot)MemberwiseClone();
    }
}

[Serializable]
public sealed class BattleA2SectEnvironmentSnapshot {
    public string environmentId;
    public string displayName;
    public string description;
    public int roundIndex;
    public int aliveCount;
    public string battleModifierTag;
    public BattleFormationPositionType arrayEyePosition;
    public string arrayEyeLabel;
    public string arrayEyeSummary;

    public BattleA2SectEnvironmentSnapshot Clone() {
        return (BattleA2SectEnvironmentSnapshot)MemberwiseClone();
    }
}

[Serializable]
public sealed class BattleCultivationSubOptionSnapshot {
    public string subOptionId;
    public string displayName;
    public string description;
    public BattleFormationPositionType recommendedPosition;
    public string carrySlotLabel;
    public string carryTitle;
    public int physicalGain;
    public int spellGain;
    public int wealthGain;
    public int companionGain;
    public int territoryGain;
    public int luckGain;
    public int battlePowerBonus;
    public int swordIntentGain;

    public BattleCultivationSubOptionSnapshot Clone() {
        return (BattleCultivationSubOptionSnapshot)MemberwiseClone();
    }
}

[Serializable]
public sealed class BattleCultivationOptionSnapshot {
    public string optionId;
    public string displayName;
    public string description;
    public string rarityLabel;
    public BattleCultivationModuleType moduleType;
    public string slotType;
    public string carrySlotLabel;
    public string weightTag;
    public BattleFormationPositionType recommendedPosition;
    public int physicalGain;
    public int spellGain;
    public int wealthGain;
    public int companionGain;
    public int territoryGain;
    public int luckGain;
    public int battlePowerBonus;
    public int swordIntentGain;
    public BattleCultivationSubOptionSnapshot[] secondaryOptions;

    public BattleCultivationOptionSnapshot Clone() {
        BattleCultivationOptionSnapshot clone = (BattleCultivationOptionSnapshot)MemberwiseClone();
        if (secondaryOptions == null || secondaryOptions.Length == 0) {
            clone.secondaryOptions = new BattleCultivationSubOptionSnapshot[0];
            return clone;
        }

        clone.secondaryOptions = new BattleCultivationSubOptionSnapshot[secondaryOptions.Length];
        for (int i = 0; i < secondaryOptions.Length; i++) {
            clone.secondaryOptions[i] = secondaryOptions[i] == null
                ? new BattleCultivationSubOptionSnapshot()
                : secondaryOptions[i].Clone();
        }

        return clone;
    }
}

[Serializable]
public sealed class BattleRoundMatchSnapshot {
    public int roundIndex;
    public int matchIndex;
    public string leftPlayerId;
    public string leftDisplayName;
    public string rightPlayerId;
    public string rightDisplayName;
    public bool isBye;
    public BattleFormationPositionType leftPosition;
    public BattleFormationPositionType rightPosition;
    public BattleFormationPositionType arrayEyePosition;
    public string leftPositionLabel;
    public string rightPositionLabel;
    public string arrayEyeLabel;
    public BattleBattleOutcomeType leftOutcome;
    public BattleBattleOutcomeType rightOutcome;
    public int leftScore;
    public int rightScore;
    public int leftLuckDelta;
    public int rightLuckDelta;
    public string summary;
    public string leftReasonSummary;
    public string rightReasonSummary;
    public string leftCarrySummary;
    public string rightCarrySummary;
    public string leftStyleSummary;
    public string rightStyleSummary;
    public string eyeSummary;

    public BattleRoundMatchSnapshot Clone() {
        return (BattleRoundMatchSnapshot)MemberwiseClone();
    }
}

public enum BattleBattleOutcomeType {
    Pending = 0,
    Win = 1,
    Lose = 2,
    Draw = 3,
    Bye = 4
}

public enum BattleCultivationModuleType {
    None = 0,
    Martial = 1,
    Spell = 2,
    Wealth = 3,
    Companion = 4,
    Territory = 5
}

public enum BattleFormationPositionType {
    None = 0,
    Front = 1,
    Middle = 2,
    Rear = 3
}
