using System;

public static class NetworkSyncBattleMessageIds {
    public const int BATTLE_SNAPSHOT_REQUEST_CMD = 12001;
    public const int BATTLE_STATE_SNAPSHOT_RPC = 12002;
    public const int BATTLE_HERO_SELECT_CMD = 12003;
    public const int BATTLE_CULTIVATION_SELECT_CMD = 12004;
    public const int BATTLE_FORMATION_CONFIRM_CMD = 12005;
    public const int BATTLE_DEBUG_ACTION_CMD = 12006;
}

[Serializable]
public sealed class NetworkSyncBattleSnapshotRequestCmd : INetworkSyncCmd {
    public string matchId;
}

[Serializable]
public sealed class NetworkSyncBattleStateRpc : INetworkSyncRpc {
    public string matchId;
    public string roomInviteCode;
    public string sceneId;
    public BattleMainStateType mainState;
    public int roundIndex;
    public BattleRoundPhaseType roundPhase;
    public int aliveCount;
    public string winnerPlayerId;
    public int stateVersion;
    public string stageName;
    public int stageEnterCount;
    public BattleA2SectEnvironmentSnapshot sectEnvironment;
    public BattleRoundFormationSnapshot formationSnapshot;
    public BattleRoundMatchSnapshot[] battleMatchList;
    public BattleParticipantSnapshot[] participants;
}

[Serializable]
public sealed class NetworkSyncBattleHeroSelectCmd : INetworkSyncCmd {
    public string matchId;
    public string playerId;
    public int candidateIndex;
}

[Serializable]
public sealed class NetworkSyncBattleCultivationSelectCmd : INetworkSyncCmd {
    public string matchId;
    public string playerId;
    public int optionIndex;
}

[Serializable]
public sealed class NetworkSyncBattleFormationConfirmCmd : INetworkSyncCmd {
    public string matchId;
    public string playerId;
    public BattleFormationPositionType position;
}

[Serializable]
public sealed class NetworkSyncBattleDebugActionCmd : INetworkSyncCmd {
    public string matchId;
    public string playerId;
    public BattleDebugActionType actionType;
    public int intValue;
}

public static class NetworkSyncBattleProtocols {
    public static readonly NetworkSyncMessageDescriptor BattleSnapshotRequestCmd =
        NetworkSyncMessageDescriptor.Create<NetworkSyncBattleSnapshotRequestCmd>(
            NetworkSyncBattleMessageIds.BATTLE_SNAPSHOT_REQUEST_CMD,
            "Battle",
            NetworkSyncMessageKind.Command,
            NetworkSyncProtocolType.Cmd,
            NetworkSyncDirection.ClientToServer,
            NetworkSyncDelivery.Reliable,
            NetworkSyncTarget.None,
            NetworkSyncAuthority.AnyClient);

    public static readonly NetworkSyncMessageDescriptor BattleHeroSelectCmd =
        NetworkSyncMessageDescriptor.Create<NetworkSyncBattleHeroSelectCmd>(
            NetworkSyncBattleMessageIds.BATTLE_HERO_SELECT_CMD,
            "Battle",
            NetworkSyncMessageKind.Command,
            NetworkSyncProtocolType.Cmd,
            NetworkSyncDirection.ClientToServer,
            NetworkSyncDelivery.Reliable,
            NetworkSyncTarget.None,
            NetworkSyncAuthority.OwnerOnly);

    public static readonly NetworkSyncMessageDescriptor BattleCultivationSelectCmd =
        NetworkSyncMessageDescriptor.Create<NetworkSyncBattleCultivationSelectCmd>(
            NetworkSyncBattleMessageIds.BATTLE_CULTIVATION_SELECT_CMD,
            "Battle",
            NetworkSyncMessageKind.Command,
            NetworkSyncProtocolType.Cmd,
            NetworkSyncDirection.ClientToServer,
            NetworkSyncDelivery.Reliable,
            NetworkSyncTarget.None,
            NetworkSyncAuthority.OwnerOnly);

    public static readonly NetworkSyncMessageDescriptor BattleFormationConfirmCmd =
        NetworkSyncMessageDescriptor.Create<NetworkSyncBattleFormationConfirmCmd>(
            NetworkSyncBattleMessageIds.BATTLE_FORMATION_CONFIRM_CMD,
            "Battle",
            NetworkSyncMessageKind.Command,
            NetworkSyncProtocolType.Cmd,
            NetworkSyncDirection.ClientToServer,
            NetworkSyncDelivery.Reliable,
            NetworkSyncTarget.None,
            NetworkSyncAuthority.OwnerOnly);

    public static readonly NetworkSyncMessageDescriptor BattleDebugActionCmd =
        NetworkSyncMessageDescriptor.Create<NetworkSyncBattleDebugActionCmd>(
            NetworkSyncBattleMessageIds.BATTLE_DEBUG_ACTION_CMD,
            "Battle",
            NetworkSyncMessageKind.Command,
            NetworkSyncProtocolType.Cmd,
            NetworkSyncDirection.ClientToServer,
            NetworkSyncDelivery.Reliable,
            NetworkSyncTarget.None,
            NetworkSyncAuthority.OwnerOnly);

    public static readonly NetworkSyncMessageDescriptor BattleStateSnapshotRpc =
        NetworkSyncMessageDescriptor.Create<NetworkSyncBattleStateRpc>(
            NetworkSyncBattleMessageIds.BATTLE_STATE_SNAPSHOT_RPC,
            "Battle",
            NetworkSyncMessageKind.Event,
            NetworkSyncProtocolType.Rpc,
            NetworkSyncDirection.ServerToClient,
            NetworkSyncDelivery.Reliable,
            NetworkSyncTarget.Broadcast,
            NetworkSyncAuthority.ServerOnly);
}
