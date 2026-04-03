using UnityEngine;

/// <summary>
/// Battle 链路本地回环测试入口。
/// 复用房间创建/开始流程，随后验证 Battle snapshot 的广播与按 matchId 请求回包。
/// </summary>
public sealed class NetworkSyncBattleLocalDemo : MonoBehaviour {
    private const string DEMO_OBJECT_NAME = "NetworkSyncBattleLocalDemo";
    private const int WORLD_ID = 1;
    private const int HOST_CONNECTION_ID = 11;
    private const int GUEST_CONNECTION_ID = 12;
    private const string TEST_SCENE_ID = "BattleTestScene";

    private NetworkSyncLocalLoopbackHub hub;
    private NetworkSyncServer server;
    private NetworkSyncRoomServerModule roomServerModule;
    private NetworkSyncBattleServerModule battleServerModule;
    private ServerRoomNetHandler roomHandler;
    private NetworkSyncClient hostClient;
    private NetworkSyncRoomClientModule hostRoomModule;
    private NetworkSyncBattleClientModule hostBattleModule;
    private ClientRoomNetProxy hostRoomProxy;
    private NetworkSyncClient guestClient;
    private NetworkSyncRoomClientModule guestRoomModule;
    private NetworkSyncBattleClientModule guestBattleModule;
    private ClientRoomNetProxy guestRoomProxy;
    private bool isInitialized;
    private string lastDemoSummary = "Battle network demo not executed.";

    public static NetworkSyncBattleLocalDemo EnsureInstance() {
        NetworkSyncBattleLocalDemo instance = FindObjectOfType<NetworkSyncBattleLocalDemo>();
        if (instance != null) {
            return instance;
        }

        GameObject demoObject = new GameObject(DEMO_OBJECT_NAME);
        return demoObject.AddComponent<NetworkSyncBattleLocalDemo>();
    }

    public string LastDemoSummary {
        get {
            return lastDemoSummary;
        }
    }

    [ContextMenu("Setup Battle Demo")]
    public void SetupBattleDemo() {
        DisposeRuntime();

        hub = new NetworkSyncLocalLoopbackHub();

        NetworkSyncLocalLoopbackServerTransport serverTransport = hub.CreateServerTransport();
        server = new NetworkSyncServer(serverTransport);
        roomServerModule = new NetworkSyncRoomServerModule(TEST_SCENE_ID, allowCreateInviteCodeFallback: true);
        battleServerModule = new NetworkSyncBattleServerModule();
        server.RegisterModule(roomServerModule);
        server.RegisterModule(battleServerModule);
        roomHandler = new ServerRoomNetHandler(roomServerModule);

        hostClient = CreateClient(
            HOST_CONNECTION_ID,
            out hostRoomModule,
            out hostBattleModule,
            out hostRoomProxy);
        guestClient = CreateClient(
            GUEST_CONNECTION_ID,
            out guestRoomModule,
            out guestBattleModule,
            out guestRoomProxy);

        isInitialized = true;
        lastDemoSummary = "Battle network demo setup completed.";
        Debug.Log(lastDemoSummary);
    }

    [ContextMenu("Run Battle Snapshot Demo Once")]
    public string RunBattleSnapshotDemoOnce() {
        EnsureInitialized();
        ResetRuntimeStates();

        hostRoomProxy.CreateRoom("38888", "BattleHost", "avatar_player_host");
        string inviteCode = roomHandler.CurrentInviteCode;
        if (string.IsNullOrWhiteSpace(inviteCode)) {
            lastDemoSummary = "Battle demo failed: invite code is empty.";
            Debug.LogError(lastDemoSummary);
            return lastDemoSummary;
        }

        guestRoomProxy.JoinRoom(inviteCode, "BattleGuest", "avatar_player_guest");
        hostRoomProxy.StartRoom();

        if (hostRoomModule.LastRoomStart == null || guestRoomModule.LastRoomStart == null) {
            lastDemoSummary = "Battle demo failed: room start rpc missing.";
            Debug.LogError(lastDemoSummary);
            return lastDemoSummary;
        }

        NetworkSyncBattleStateRpc snapshot = BuildBattleSnapshot();
        if (snapshot == null) {
            lastDemoSummary = "Battle demo failed: battle snapshot build failed.";
            Debug.LogError(lastDemoSummary);
            return lastDemoSummary;
        }

        battleServerModule.SetCurrentState(snapshot);
        battleServerModule.BroadcastCurrentState(WORLD_ID);

        hostBattleModule.RequestSnapshot(snapshot.matchId, WORLD_ID);
        guestBattleModule.RequestSnapshot(snapshot.matchId, WORLD_ID);

        bool hostReceived = hostBattleModule.LastState != null &&
            hostBattleModule.LastState.matchId == snapshot.matchId;
        bool guestReceived = guestBattleModule.LastState != null &&
            guestBattleModule.LastState.matchId == snapshot.matchId;

        hostRoomProxy.LeaveRoom();

        lastDemoSummary =
            "Battle Demo OK | MatchId: " + snapshot.matchId +
            " | HostReceived: " + hostReceived +
            " | GuestReceived: " + guestReceived +
            " | Alive: " + snapshot.aliveCount +
            " | State: " + snapshot.mainState +
            "/" + snapshot.roundPhase;
        Debug.Log(lastDemoSummary);
        return lastDemoSummary;
    }

    private void EnsureInitialized() {
        if (!isInitialized) {
            SetupBattleDemo();
        }
    }

    private NetworkSyncClient CreateClient(
        int connectionId,
        out NetworkSyncRoomClientModule roomModule,
        out NetworkSyncBattleClientModule battleModule,
        out ClientRoomNetProxy roomProxy) {
        NetworkSyncLocalLoopbackClientTransport transport = hub.CreateClientTransport(connectionId);
        NetworkSyncClient client = new NetworkSyncClient(transport);
        roomModule = new NetworkSyncRoomClientModule();
        battleModule = new NetworkSyncBattleClientModule();
        client.RegisterModule(roomModule);
        client.RegisterModule(battleModule);
        roomProxy = new ClientRoomNetProxy(roomModule, WORLD_ID);
        return client;
    }

    private void ResetRuntimeStates() {
        if (hostRoomModule != null) {
            hostRoomModule.ResetRuntimeState();
        }

        if (guestRoomModule != null) {
            guestRoomModule.ResetRuntimeState();
        }

        if (hostBattleModule != null) {
            hostBattleModule.ResetRuntimeState();
        }

        if (guestBattleModule != null) {
            guestBattleModule.ResetRuntimeState();
        }
    }

    private NetworkSyncBattleStateRpc BuildBattleSnapshot() {
        NetworkSyncRoomStateRpc roomState = roomHandler == null ? null : roomHandler.GetRoomStateSnapshot();
        if (roomState == null || roomState.slots == null || roomState.slots.Length == 0) {
            return null;
        }

        NetworkSyncBattleStateRpc snapshot = new NetworkSyncBattleStateRpc();
        snapshot.matchId = roomHandler.CurrentMatchId;
        snapshot.roomInviteCode = roomHandler.CurrentInviteCode;
        snapshot.sceneId = TEST_SCENE_ID;
        snapshot.mainState = BattleMainStateType.MatchInit;
        snapshot.roundIndex = 0;
        snapshot.roundPhase = BattleRoundPhaseType.Battle;
        snapshot.aliveCount = roomState.slots.Length;
        snapshot.winnerPlayerId = string.Empty;
        snapshot.stateVersion = 1;
        snapshot.stageName = "BattleMatchInitStage";
        snapshot.stageEnterCount = 1;
        snapshot.sectEnvironment = BattleA2ContentCatalog.BuildSectEnvironment(0, snapshot.aliveCount);
        snapshot.battleMatchList = new BattleRoundMatchSnapshot[0];
        snapshot.participants = BuildParticipants(roomState.slots);
        return snapshot;
    }

    private static BattleParticipantSnapshot[] BuildParticipants(NetworkSyncRoomSlotData[] slots) {
        BattleParticipantSnapshot[] participants = new BattleParticipantSnapshot[slots.Length];
        for (int i = 0; i < slots.Length; i++) {
            NetworkSyncRoomSlotData slot = slots[i];
            participants[i] = new BattleParticipantSnapshot {
                slotIndex = slot == null ? i : slot.slotIndex,
                playerId = slot == null ? string.Empty : slot.playerId ?? string.Empty,
                displayName = slot == null ? string.Empty : slot.displayName ?? string.Empty,
                avatarId = slot == null ? string.Empty : slot.avatarId ?? string.Empty,
                participantType = slot == null ? NetworkSyncRoomParticipantType.AI : slot.participantType,
                isHost = slot != null && slot.isHost,
                isEliminated = false,
                luckValue = BattleA2ContentCatalog.ResolveInitialLuck(slot == null ? i : slot.slotIndex),
                battlePowerTotal = 0,
                physicalGrowthTotal = 0,
                spellGrowthTotal = 0,
                wealthGrowthTotal = 0,
                companionGrowthTotal = 0,
                territoryGrowthTotal = 0,
                recentBattleRoundIndex = -1,
                recentBattleOutcome = BattleBattleOutcomeType.Pending,
                recentBattleIsBye = false,
                recentBattleOpponentPlayerId = string.Empty,
                recentBattleOpponentDisplayName = string.Empty,
                recentBattleScore = 0,
                recentBattleLuckDelta = 0,
                recentBattleSummary = string.Empty,
                cultivationOptions = new BattleCultivationOptionSnapshot[0],
                selectedCultivationOptionIndex = -1,
                selectedCultivationOptionId = string.Empty,
                selectedCultivationOptionName = string.Empty,
                selectedCultivationOptionSummary = string.Empty,
                selectedCultivationOptionSource = string.Empty,
                selectedCultivationBattlePowerGain = 0,
                selectedCultivationPhysicalGain = 0,
                selectedCultivationSpellGain = 0,
                selectedCultivationWealthGain = 0,
                selectedCultivationCompanionGain = 0,
                selectedCultivationTerritoryGain = 0,
                selectedCultivationLuckGain = 0,
                hasSelectedCultivationOption = false,
                lastGrowthRoundIndex = -1,
                lastGrowthOptionId = string.Empty,
                lastGrowthOptionName = string.Empty,
                lastGrowthOptionSummary = string.Empty,
                lastGrowthBattlePowerGain = 0,
                lastGrowthPhysicalGain = 0,
                lastGrowthSpellGain = 0,
                lastGrowthWealthGain = 0,
                lastGrowthCompanionGain = 0,
                lastGrowthTerritoryGain = 0,
                lastGrowthLuckGain = 0,
                heroCandidates = BattleA2ContentCatalog.BuildHeroCandidates(slot == null ? i : slot.slotIndex, 0),
                selectedCandidateIndex = -1,
                selectedHeroId = string.Empty,
                selectedHeroName = string.Empty,
                selectionSource = string.Empty,
                hasLockedHero = false,
                hasCompletedCurrentPhase = false
            };
        }

        return participants;
    }

    private void OnDestroy() {
        DisposeRuntime();
    }

    private void DisposeRuntime() {
        DisposeClient(hostClient);
        DisposeClient(guestClient);

        if (server != null) {
            server.Dispose();
        }

        hub = null;
        server = null;
        roomServerModule = null;
        battleServerModule = null;
        roomHandler = null;
        hostClient = null;
        hostRoomModule = null;
        hostBattleModule = null;
        hostRoomProxy = null;
        guestClient = null;
        guestRoomModule = null;
        guestBattleModule = null;
        guestRoomProxy = null;
        isInitialized = false;
    }

    private static void DisposeClient(NetworkSyncClient client) {
        if (client != null) {
            client.Dispose();
        }
    }
}
