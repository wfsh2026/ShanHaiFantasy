using System;

public sealed class NetworkSyncBattleServerModule : INetworkSyncServerModule {
    private NetworkSyncServer server;
    private NetworkSyncBattleStateRpc currentState;
    private Func<int, string> playerIdResolver;

    public event Action<NetworkSyncServerContext, NetworkSyncBattleHeroSelectCmd> BattleHeroSelectRequested;
    public event Action<NetworkSyncServerContext, NetworkSyncBattleCultivationSelectCmd> BattleCultivationSelectRequested;
    public event Action<NetworkSyncServerContext, NetworkSyncBattleFormationConfirmCmd> BattleFormationConfirmRequested;
    public event Action<NetworkSyncServerContext, NetworkSyncBattleDebugActionCmd> BattleDebugActionRequested;

    public NetworkSyncBattleStateRpc CurrentState {
        get { return currentState; }
    }

    public void SetPlayerIdResolver(Func<int, string> resolver) {
        playerIdResolver = resolver;
    }

    public void Register(NetworkSyncServer networkServer) {
        server = networkServer ?? throw new ArgumentNullException(nameof(networkServer));
        server.RegisterCmd<NetworkSyncBattleSnapshotRequestCmd>(
            NetworkSyncBattleProtocols.BattleSnapshotRequestCmd,
            HandleBattleSnapshotRequestCmd,
            ValidateBattleSnapshotRequestCmd);
        server.RegisterCmd<NetworkSyncBattleHeroSelectCmd>(
            NetworkSyncBattleProtocols.BattleHeroSelectCmd,
            HandleBattleHeroSelectCmd,
            ValidateBattleHeroSelectCmd);
        server.RegisterCmd<NetworkSyncBattleCultivationSelectCmd>(
            NetworkSyncBattleProtocols.BattleCultivationSelectCmd,
            HandleBattleCultivationSelectCmd,
            ValidateBattleCultivationSelectCmd);
        server.RegisterCmd<NetworkSyncBattleFormationConfirmCmd>(
            NetworkSyncBattleProtocols.BattleFormationConfirmCmd,
            HandleBattleFormationConfirmCmd,
            ValidateBattleFormationConfirmCmd);
        server.RegisterCmd<NetworkSyncBattleDebugActionCmd>(
            NetworkSyncBattleProtocols.BattleDebugActionCmd,
            HandleBattleDebugActionCmd,
            ValidateBattleDebugActionCmd);
        server.Registry.Register(NetworkSyncBattleProtocols.BattleStateSnapshotRpc);
        server.Registry.Register(NetworkSyncBattleProtocols.BattleSnapshotRequestCmd);
        server.Registry.Register(NetworkSyncBattleProtocols.BattleHeroSelectCmd);
        server.Registry.Register(NetworkSyncBattleProtocols.BattleCultivationSelectCmd);
        server.Registry.Register(NetworkSyncBattleProtocols.BattleFormationConfirmCmd);
        server.Registry.Register(NetworkSyncBattleProtocols.BattleDebugActionCmd);
    }

    public void SetCurrentState(NetworkSyncBattleStateRpc state) {
        currentState = CloneState(state);
    }

    public void BroadcastCurrentState(int worldId, Predicate<NetworkSyncConnectionRef> filter = null) {
        if (server == null || currentState == null) {
            return;
        }

        server.Rpc(CloneState(currentState), worldId, filter);
    }

    public void ReplyCurrentState(NetworkSyncServerContext context) {
        if (context == null || currentState == null) {
            return;
        }

        NetworkSyncConnectionRef targetConnection = context.Connection;
        if (targetConnection == null) {
            return;
        }

        context.Rpc(
            CloneState(currentState),
            connection => connection != null && connection.ConnectionId == targetConnection.ConnectionId);
    }

    private NetworkSyncValidationResult ValidateBattleSnapshotRequestCmd(
        NetworkSyncServerContext context,
        NetworkSyncBattleSnapshotRequestCmd message) {
        if (message == null) {
            return NetworkSyncValidationResult.Fail("battle_snapshot_request_null", "Battle snapshot request is null.");
        }

        return NetworkSyncValidationResult.Ok();
    }

    private void HandleBattleSnapshotRequestCmd(NetworkSyncServerContext context, NetworkSyncBattleSnapshotRequestCmd message) {
        ReplyCurrentState(context);
    }

    private static NetworkSyncBattleStateRpc CloneState(NetworkSyncBattleStateRpc source) {
        if (source == null) {
            return null;
        }

        NetworkSyncBattleStateRpc state = new NetworkSyncBattleStateRpc();
        state.matchId = source.matchId;
        state.roomInviteCode = source.roomInviteCode;
        state.sceneId = source.sceneId;
        state.mainState = source.mainState;
        state.roundIndex = source.roundIndex;
        state.roundPhase = source.roundPhase;
        state.aliveCount = source.aliveCount;
        state.winnerPlayerId = source.winnerPlayerId;
        state.stateVersion = source.stateVersion;
        state.stageName = source.stageName;
        state.stageEnterCount = source.stageEnterCount;
        state.sectEnvironment = source.sectEnvironment == null ? null : source.sectEnvironment.Clone();
        state.formationSnapshot = source.formationSnapshot == null ? null : source.formationSnapshot.Clone();
        state.battleMatchList = CloneBattleMatchArray(source.battleMatchList);

        if (source.participants != null) {
            state.participants = new BattleParticipantSnapshot[source.participants.Length];
            for (int i = 0; i < source.participants.Length; i++) {
                state.participants[i] = source.participants[i] == null
                    ? new BattleParticipantSnapshot()
                    : source.participants[i].Clone();
            }
        } else {
            state.participants = new BattleParticipantSnapshot[0];
        }

        return state;
    }

    private NetworkSyncValidationResult ValidateBattleHeroSelectCmd(
        NetworkSyncServerContext context,
        NetworkSyncBattleHeroSelectCmd message) {
        if (message == null) {
            return NetworkSyncValidationResult.Fail("battle_hero_select_null", "Battle hero select cmd is null.");
        }

        if (currentState == null) {
            return NetworkSyncValidationResult.Fail("battle_state_missing", "Battle state snapshot is missing.");
        }

        if (!string.Equals(currentState.matchId ?? string.Empty, message.matchId ?? string.Empty, StringComparison.Ordinal)) {
            return NetworkSyncValidationResult.Fail("battle_match_mismatch", "Battle match id does not match current state.");
        }

        if (currentState.mainState != BattleMainStateType.HeroSelect) {
            return NetworkSyncValidationResult.Fail("battle_phase_invalid", "Battle hero select is only valid in HeroSelect.");
        }

        if (context == null || context.Connection == null) {
            return NetworkSyncValidationResult.Fail("battle_connection_missing", "Battle connection is missing.");
        }

        string authoritativePlayerId = ResolveAuthoritativePlayerId(context.Connection);
        if (!string.Equals(authoritativePlayerId ?? string.Empty, message.playerId ?? string.Empty, StringComparison.Ordinal)) {
            return NetworkSyncValidationResult.Fail("battle_player_mismatch", "Battle player id does not match connection metadata.");
        }

        if (currentState.participants == null || currentState.participants.Length == 0) {
            return NetworkSyncValidationResult.Fail("battle_participant_missing", "Battle participants are missing.");
        }

        for (int i = 0; i < currentState.participants.Length; i++) {
            BattleParticipantSnapshot participant = currentState.participants[i];
            if (participant == null || participant.isEliminated) {
                continue;
            }

            if (!string.Equals(participant.playerId ?? string.Empty, message.playerId ?? string.Empty, StringComparison.Ordinal)) {
                continue;
            }

            if (participant.hasLockedHero) {
                return NetworkSyncValidationResult.Fail("battle_hero_locked", "Battle participant already locked hero.");
            }

            if (participant.heroCandidates == null || participant.heroCandidates.Length == 0) {
                return NetworkSyncValidationResult.Fail("battle_candidate_missing", "Battle participant has no hero candidates.");
            }

            if (message.candidateIndex < 0 || message.candidateIndex >= participant.heroCandidates.Length) {
                return NetworkSyncValidationResult.Fail("battle_candidate_invalid", "Battle candidate index is invalid.");
            }

            return NetworkSyncValidationResult.Ok();
        }

        return NetworkSyncValidationResult.Fail("battle_participant_not_found", "Battle participant was not found.");
    }

    private void HandleBattleHeroSelectCmd(NetworkSyncServerContext context, NetworkSyncBattleHeroSelectCmd message) {
        BattleHeroSelectRequested?.Invoke(context, message);
    }

    private NetworkSyncValidationResult ValidateBattleCultivationSelectCmd(
        NetworkSyncServerContext context,
        NetworkSyncBattleCultivationSelectCmd message) {
        if (message == null) {
            return NetworkSyncValidationResult.Fail("battle_cultivation_select_null", "Battle cultivation select cmd is null.");
        }

        if (currentState == null) {
            return NetworkSyncValidationResult.Fail("battle_state_missing", "Battle state snapshot is missing.");
        }

        if (!string.Equals(currentState.matchId ?? string.Empty, message.matchId ?? string.Empty, StringComparison.Ordinal)) {
            return NetworkSyncValidationResult.Fail("battle_match_mismatch", "Battle match id does not match current state.");
        }

        if (currentState.mainState != BattleMainStateType.RoundLoop ||
            currentState.roundPhase != BattleRoundPhaseType.Cultivation) {
            return NetworkSyncValidationResult.Fail("battle_phase_invalid", "Battle cultivation select is only valid in Cultivation.");
        }

        if (context == null || context.Connection == null) {
            return NetworkSyncValidationResult.Fail("battle_connection_missing", "Battle connection is missing.");
        }

        string authoritativePlayerId = ResolveAuthoritativePlayerId(context.Connection);
        if (!string.Equals(authoritativePlayerId ?? string.Empty, message.playerId ?? string.Empty, StringComparison.Ordinal)) {
            return NetworkSyncValidationResult.Fail("battle_player_mismatch", "Battle player id does not match connection metadata.");
        }

        if (currentState.participants == null || currentState.participants.Length == 0) {
            return NetworkSyncValidationResult.Fail("battle_participant_missing", "Battle participants are missing.");
        }

        for (int i = 0; i < currentState.participants.Length; i++) {
            BattleParticipantSnapshot participant = currentState.participants[i];
            if (participant == null || participant.isEliminated) {
                continue;
            }

            if (!string.Equals(participant.playerId ?? string.Empty, message.playerId ?? string.Empty, StringComparison.Ordinal)) {
                continue;
            }

            if (participant.hasCompletedCurrentPhase) {
                return NetworkSyncValidationResult.Fail("battle_cultivation_locked", "Battle participant already selected cultivation.");
            }

            if (participant.cultivationOptions == null || participant.cultivationOptions.Length == 0) {
                return NetworkSyncValidationResult.Fail("battle_cultivation_option_missing", "Battle participant has no cultivation options.");
            }

            if (message.optionIndex < 0 || message.optionIndex >= participant.cultivationOptions.Length) {
                return NetworkSyncValidationResult.Fail("battle_cultivation_option_invalid", "Battle cultivation option index is invalid.");
            }

            return NetworkSyncValidationResult.Ok();
        }

        return NetworkSyncValidationResult.Fail("battle_participant_not_found", "Battle participant was not found.");
    }

    private void HandleBattleCultivationSelectCmd(
        NetworkSyncServerContext context,
        NetworkSyncBattleCultivationSelectCmd message) {
        BattleCultivationSelectRequested?.Invoke(context, message);
    }

    private NetworkSyncValidationResult ValidateBattleFormationConfirmCmd(
        NetworkSyncServerContext context,
        NetworkSyncBattleFormationConfirmCmd message) {
        if (message == null) {
            return NetworkSyncValidationResult.Fail("battle_formation_confirm_null", "Battle formation confirm cmd is null.");
        }

        if (currentState == null) {
            return NetworkSyncValidationResult.Fail("battle_state_missing", "Battle state snapshot is missing.");
        }

        if (currentState.mainState != BattleMainStateType.RoundLoop ||
            currentState.roundPhase != BattleRoundPhaseType.FormationConfirm) {
            return NetworkSyncValidationResult.Fail("battle_phase_invalid", "Battle formation confirm is only valid in FormationConfirm.");
        }

        string authoritativePlayerId = ResolveAuthoritativePlayerId(context == null ? null : context.Connection);
        if (!string.Equals(authoritativePlayerId ?? string.Empty, message.playerId ?? string.Empty, StringComparison.Ordinal)) {
            return NetworkSyncValidationResult.Fail("battle_player_mismatch", "Battle player id does not match connection metadata.");
        }

        return ValidateParticipantAvailability(message.playerId, participant => !participant.hasConfirmedFormation);
    }

    private void HandleBattleFormationConfirmCmd(
        NetworkSyncServerContext context,
        NetworkSyncBattleFormationConfirmCmd message) {
        BattleFormationConfirmRequested?.Invoke(context, message);
    }

    private NetworkSyncValidationResult ValidateBattleDebugActionCmd(
        NetworkSyncServerContext context,
        NetworkSyncBattleDebugActionCmd message) {
        if (message == null) {
            return NetworkSyncValidationResult.Fail("battle_debug_action_null", "Battle debug action cmd is null.");
        }

        if (currentState == null) {
            return NetworkSyncValidationResult.Fail("battle_state_missing", "Battle state snapshot is missing.");
        }

        string authoritativePlayerId = ResolveAuthoritativePlayerId(context == null ? null : context.Connection);
        if (!string.Equals(authoritativePlayerId ?? string.Empty, message.playerId ?? string.Empty, StringComparison.Ordinal)) {
            return NetworkSyncValidationResult.Fail("battle_player_mismatch", "Battle player id does not match connection metadata.");
        }

        if (currentState.participants == null || currentState.participants.Length == 0) {
            return NetworkSyncValidationResult.Fail("battle_participant_missing", "Battle participants are missing.");
        }

        for (int i = 0; i < currentState.participants.Length; i++) {
            BattleParticipantSnapshot participant = currentState.participants[i];
            if (participant != null &&
                string.Equals(participant.playerId ?? string.Empty, message.playerId ?? string.Empty, StringComparison.Ordinal) &&
                participant.isHost) {
                return NetworkSyncValidationResult.Ok();
            }
        }

        return NetworkSyncValidationResult.Fail("battle_debug_host_only", "Battle debug action requires host authority.");
    }

    private void HandleBattleDebugActionCmd(
        NetworkSyncServerContext context,
        NetworkSyncBattleDebugActionCmd message) {
        BattleDebugActionRequested?.Invoke(context, message);
    }

    private NetworkSyncValidationResult ValidateParticipantAvailability(
        string playerId,
        Func<BattleParticipantSnapshot, bool> predicate) {
        if (currentState.participants == null || currentState.participants.Length == 0) {
            return NetworkSyncValidationResult.Fail("battle_participant_missing", "Battle participants are missing.");
        }

        for (int i = 0; i < currentState.participants.Length; i++) {
            BattleParticipantSnapshot participant = currentState.participants[i];
            if (participant == null || participant.isEliminated) {
                continue;
            }

            if (!string.Equals(participant.playerId ?? string.Empty, playerId ?? string.Empty, StringComparison.Ordinal)) {
                continue;
            }

            if (!predicate(participant)) {
                return NetworkSyncValidationResult.Fail("battle_participant_locked", "Battle participant is not available for this command.");
            }

            return NetworkSyncValidationResult.Ok();
        }

        return NetworkSyncValidationResult.Fail("battle_participant_not_found", "Battle participant was not found.");
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

    private string ResolveAuthoritativePlayerId(NetworkSyncConnectionRef connection) {
        if (connection == null) {
            return string.Empty;
        }

        if (playerIdResolver != null) {
            string resolvedByRoom = playerIdResolver(connection.ConnectionId);
            if (!string.IsNullOrWhiteSpace(resolvedByRoom)) {
                return resolvedByRoom;
            }
        }

        if (connection.Metadata.TryGet("player_id", out string playerId)) {
            return playerId ?? string.Empty;
        }

        return string.Empty;
    }
}
