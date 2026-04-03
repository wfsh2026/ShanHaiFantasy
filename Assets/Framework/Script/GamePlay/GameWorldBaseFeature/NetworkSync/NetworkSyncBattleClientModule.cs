using System;

public sealed class NetworkSyncBattleClientModule : INetworkSyncClientModule {
    private NetworkSyncClient client;

    public event Action<NetworkSyncBattleStateRpc> BattleStateReceived;

    public NetworkSyncBattleStateRpc LastState { get; private set; }

    public void Register(NetworkSyncClient networkClient) {
        client = networkClient ?? throw new ArgumentNullException(nameof(networkClient));
        client.RegisterRpc<NetworkSyncBattleStateRpc>(
            NetworkSyncBattleProtocols.BattleStateSnapshotRpc,
            OnBattleStateReceivedInternal);
        client.Registry.Register(NetworkSyncBattleProtocols.BattleSnapshotRequestCmd);
        client.Registry.Register(NetworkSyncBattleProtocols.BattleHeroSelectCmd);
        client.Registry.Register(NetworkSyncBattleProtocols.BattleCultivationSelectCmd);
        client.Registry.Register(NetworkSyncBattleProtocols.BattleFormationConfirmCmd);
        client.Registry.Register(NetworkSyncBattleProtocols.BattleDebugActionCmd);
    }

    public void ResetRuntimeState() {
        LastState = null;
    }

    public void RequestSnapshot(string matchId, int worldId = 0) {
        if (client == null) {
            throw new InvalidOperationException("NetworkSyncBattleClientModule is not registered.");
        }

        client.SendCmd(new NetworkSyncBattleSnapshotRequestCmd {
            matchId = matchId ?? string.Empty
        }, worldId);
    }

    public void SendHeroSelect(string matchId, string playerId, int candidateIndex, int worldId = 0) {
        if (client == null) {
            throw new InvalidOperationException("NetworkSyncBattleClientModule is not registered.");
        }

        client.SendCmd(new NetworkSyncBattleHeroSelectCmd {
            matchId = matchId ?? string.Empty,
            playerId = playerId ?? string.Empty,
            candidateIndex = candidateIndex
        }, worldId);
    }

    public void SendCultivationSelect(string matchId, string playerId, int optionIndex, int worldId = 0) {
        if (client == null) {
            throw new InvalidOperationException("NetworkSyncBattleClientModule is not registered.");
        }

        client.SendCmd(new NetworkSyncBattleCultivationSelectCmd {
            matchId = matchId ?? string.Empty,
            playerId = playerId ?? string.Empty,
            optionIndex = optionIndex
        }, worldId);
    }

    public void SendFormationConfirm(string matchId, string playerId, BattleFormationPositionType position, int worldId = 0) {
        if (client == null) {
            throw new InvalidOperationException("NetworkSyncBattleClientModule is not registered.");
        }

        client.SendCmd(new NetworkSyncBattleFormationConfirmCmd {
            matchId = matchId ?? string.Empty,
            playerId = playerId ?? string.Empty,
            position = position
        }, worldId);
    }

    public void SendDebugAction(string matchId, string playerId, BattleDebugActionType actionType, int intValue = 0, int worldId = 0) {
        if (client == null) {
            throw new InvalidOperationException("NetworkSyncBattleClientModule is not registered.");
        }

        client.SendCmd(new NetworkSyncBattleDebugActionCmd {
            matchId = matchId ?? string.Empty,
            playerId = playerId ?? string.Empty,
            actionType = actionType,
            intValue = intValue
        }, worldId);
    }

    private void OnBattleStateReceivedInternal(NetworkSyncClientContext context, NetworkSyncBattleStateRpc message) {
        LastState = message;
        BattleStateReceived?.Invoke(message);
    }
}
