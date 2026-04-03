using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class BattleA2SceneVerifier : MonoBehaviour {
    [SerializeField] private bool autoOpenBattlePanel = true;
    [SerializeField] private bool autoOpenQuickSetupPanel = true;
    [SerializeField] private bool autoDriveCultivationScene = true;

    private float nextRetryTime;
    private float nextDriveTime;

    private void Start() { TryOpenPanels(); }

    private void Update() {
        if (Time.unscaledTime >= nextRetryTime) {
            nextRetryTime = Time.unscaledTime + 0.5f;
            TryOpenPanels();
        }

        if (Time.unscaledTime >= nextDriveTime) {
            nextDriveTime = Time.unscaledTime + 0.75f;
            TryDriveCultivationScene();
        }
    }

    private void OnGUI() {
        if (!IsVerifierScene()) {
            return;
        }

        BattleModeData data = ResolveData();
        GUI.Box(new Rect(16f, 16f, 420f, 280f), "Battle Scene Verifier");
        GUI.Label(new Rect(28f, 44f, 392f, 246f), BuildVerifierSummary(data));
    }

    private void TryOpenPanels() {
        if (!IsVerifierScene() || UIManager.Instance == null || UIManager.Instance.GameWorld == null) {
            return;
        }

        if (autoOpenBattlePanel && !UIManager.Instance.IsOpen<BattleA2Panel>()) {
            UIManager.Instance.Open<BattleA2Panel>();
        }

        if (autoOpenQuickSetupPanel &&
            SceneManager.GetActiveScene().name == "CultivationTestScene" &&
            !UIManager.Instance.IsOpen<BattleQuickSetupPanel>()) {
            UIManager.Instance.Open<BattleQuickSetupPanel>();
        }
    }

    private void TryDriveCultivationScene() {
        if (!autoDriveCultivationScene || SceneManager.GetActiveScene().name != "CultivationTestScene") {
            return;
        }

        ClientBattleModeLogic logic = ResolveLogic();
        BattleModeData data = ResolveData();
        if (logic == null || data == null) {
            return;
        }

        if (data.MainState == BattleMainStateType.RoundLoop && data.RoundPhase == BattleRoundPhaseType.Cultivation) {
            return;
        }

        if (data.MainState == BattleMainStateType.GameOver) {
            return;
        }

        logic.SendDebugAction(BattleDebugActionType.SkipPhase);
    }

    private BattleModeData ResolveData() {
        if (UIManager.Instance == null || UIManager.Instance.GameWorld == null) return null;
        ClientBattleModeManager manager = UIManager.Instance.GameWorld.GetExtendFeature<ClientBattleModeManager>();
        return manager == null ? null : manager.GetData<BattleModeData>();
    }

    private ClientBattleModeLogic ResolveLogic() {
        if (UIManager.Instance == null || UIManager.Instance.GameWorld == null) return null;
        ClientBattleModeManager manager = UIManager.Instance.GameWorld.GetExtendFeature<ClientBattleModeManager>();
        return manager == null ? null : manager.GetLogic<ClientBattleModeLogic>();
    }

    private static string BuildVerifierSummary(BattleModeData data) {
        if (data == null || !data.HasSnapshot) return "Waiting for BattleMode snapshot...";
        BattleParticipantSnapshot local = ResolveLocal(data);
        return "Scene: " + SceneManager.GetActiveScene().name +
            "\nState: " + data.MainState +
            "\nRound: " + data.RoundIndex + " | Phase: " + data.RoundPhase +
            "\nParticipants: " + (data.Participants == null ? 0 : data.Participants.Length) + " | Alive: " + data.AliveCount +
            "\nOptionsReady: " + (local != null && local.cultivationOptions != null && local.cultivationOptions.Length >= 4) +
            "\nFormationReady: " + (data.FormationSnapshot != null) +
            "\nBuildReady: " + (local != null && local.buildProfile != null) +
            "\nSecondaryReady: " + (local != null && !string.IsNullOrWhiteSpace(local.selectedSecondaryCultivationOptionName)) +
            "\nAIDecisionReady: " + HasAiDecision(data) +
            "\nLatestBattle: " + BuildLatestBattle(data.BattleMatchList) +
            "\nLatestGrowth: " + BuildLatestGrowth(local);
    }

    private static BattleParticipantSnapshot ResolveLocal(BattleModeData data) {
        if (data == null || data.Participants == null) return null;
        for (int i = 0; i < data.Participants.Length; i++) {
            if (data.Participants[i] != null && data.Participants[i].isHost) return data.Participants[i];
        }
        return data.Participants.Length > 0 ? data.Participants[0] : null;
    }

    private static bool HasAiDecision(BattleModeData data) {
        if (data == null || data.Participants == null) return false;
        for (int i = 0; i < data.Participants.Length; i++) {
            BattleParticipantSnapshot p = data.Participants[i];
            if (p == null || p.participantType != NetworkSyncRoomParticipantType.AI) continue;
            if (p.hasLockedHero || p.hasSelectedCultivationOption || p.hasConfirmedFormation) return true;
        }
        return false;
    }

    private static string BuildLatestBattle(BattleRoundMatchSnapshot[] matches) {
        if (matches == null || matches.Length == 0) return "-";
        for (int i = matches.Length - 1; i >= 0; i--) if (matches[i] != null) return matches[i].summary;
        return "-";
    }

    private static string BuildLatestGrowth(BattleParticipantSnapshot local) {
        if (local == null || local.lastGrowthRoundIndex < 0) return "-";
        return local.lastGrowthOptionName + " / " + local.selectedSecondaryCultivationOptionName;
    }

    private static bool IsVerifierScene() {
        string sceneName = SceneManager.GetActiveScene().name;
        return sceneName == "BattleTestScene" || sceneName == "CultivationTestScene";
    }
}
