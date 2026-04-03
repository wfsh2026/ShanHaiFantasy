using UnityEngine;

public sealed class QuickSetupPanel : MonoBehaviour {
    private const string PANEL_NAME = "QuickSetupPanel";

    public static QuickSetupPanel EnsureInstance() {
        QuickSetupPanel instance = FindObjectOfType<QuickSetupPanel>();
        if (instance != null) {
            return instance;
        }

        GameObject panelObject = new GameObject(PANEL_NAME);
        return panelObject.AddComponent<QuickSetupPanel>();
    }

    private void OnGUI() {
        ClientBattleModeLogic logic = ResolveLogic();
        BattleModeData data = ResolveData();
        if (logic == null || data == null) {
            return;
        }

        GUI.Box(new Rect(Screen.width - 270f, 16f, 250f, 246f), "QuickSetupPanel");
        float x = Screen.width - 256f;
        float y = 44f;
        float width = 106f;
        float height = 28f;
        float gap = 8f;
        if (GUI.Button(new Rect(x, y, width, height), "Skip Phase")) {
            logic.SendDebugAction(BattleDebugActionType.SkipPhase);
        }

        if (GUI.Button(new Rect(x + width + gap, y, width, height), "Next Env")) {
            logic.SendDebugAction(BattleDebugActionType.CycleEnvironment, 1);
        }

        y += height + gap;
        if (GUI.Button(new Rect(x, y, width, height), "Next Eye")) {
            logic.SendDebugAction(BattleDebugActionType.CycleFormationEye, 1);
        }

        if (GUI.Button(new Rect(x + width + gap, y, width, height), "+Luck")) {
            logic.SendDebugAction(BattleDebugActionType.AddLuck, 1);
        }

        y += height + gap;
        if (GUI.Button(new Rect(x, y, width, height), "+Sword")) {
            logic.SendDebugAction(BattleDebugActionType.AddSwordIntent, 1);
        }

        if (GUI.Button(new Rect(x + width + gap, y, width, height), "Refresh Opt")) {
            logic.SendDebugAction(BattleDebugActionType.RefreshCultivation);
        }

        y += height + gap;
        if (GUI.Button(new Rect(x, y, width, height), "Front")) {
            logic.SendDebugAction(BattleDebugActionType.ForceFront);
        }

        if (GUI.Button(new Rect(x + width + gap, y, width, height), "Middle")) {
            logic.SendDebugAction(BattleDebugActionType.ForceMiddle);
        }

        y += height + gap;
        if (GUI.Button(new Rect(x, y, width, height), "Rear")) {
            logic.SendDebugAction(BattleDebugActionType.ForceRear);
        }

        GUI.Label(
            new Rect(x, y + height + gap, 220f, 52f),
            "Round " + data.RoundIndex +
            " | " + data.RoundPhase +
            "\nEye: " + (data.FormationSnapshot == null ? "-" : BattleBContentCatalog.ResolveFormationLabel(data.FormationSnapshot.eyePosition)));
    }

    private static BattleModeData ResolveData() {
        ClientBattleModeManager manager = ResolveModeManager();
        return manager == null ? null : manager.GetData<BattleModeData>();
    }

    private static ClientBattleModeLogic ResolveLogic() {
        ClientBattleModeManager manager = ResolveModeManager();
        return manager == null ? null : manager.GetLogic<ClientBattleModeLogic>();
    }

    private static ClientBattleModeManager ResolveModeManager() {
        if (UIManager.Instance == null || UIManager.Instance.GameWorld == null) {
            return null;
        }

        return UIManager.Instance.GameWorld.GetExtendFeature<ClientBattleModeManager>();
    }
}
