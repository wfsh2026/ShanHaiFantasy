public sealed class BattleQuickSetupPanelController : UIControllerBase<BattleQuickSetupPanel> {
    private ClientBattleModeLogic logic;
    private BattleModeData data;

    public BattleQuickSetupPanelController(BattleQuickSetupPanel targetPanel) : base(targetPanel) { }

    public override void Bind() {
        ClientBattleModeManager manager = gameWorld == null ? null : gameWorld.GetExtendFeature<ClientBattleModeManager>();
        logic = manager == null ? null : manager.GetLogic<ClientBattleModeLogic>();
        data = manager == null ? null : manager.GetData<BattleModeData>();
        RefreshStatus();
    }

    public override void Unbind() { logic = null; data = null; }

    public void Trigger(int index) {
        if (logic == null) {
            return;
        }

        switch (index) {
            case 0: logic.SendDebugAction(BattleDebugActionType.SkipPhase); break;
            case 1: logic.SendDebugAction(BattleDebugActionType.CycleEnvironment, 1); break;
            case 2: logic.SendDebugAction(BattleDebugActionType.CycleFormationEye, 1); break;
            case 3: logic.SendDebugAction(BattleDebugActionType.AddLuck, 1); break;
            case 4: logic.SendDebugAction(BattleDebugActionType.AddSwordIntent, 1); break;
            case 5: logic.SendDebugAction(BattleDebugActionType.ForceFront); break;
            case 6: logic.SendDebugAction(BattleDebugActionType.ForceMiddle); break;
            case 7: logic.SendDebugAction(BattleDebugActionType.ForceRear); break;
            case 8: logic.SendDebugAction(BattleDebugActionType.RefreshCultivation); break;
        }

        RefreshStatus();
    }

    private void RefreshStatus() {
        if (data == null) {
            panel.RefreshStatus("QuickSetup\n等待 BattleMode");
            return;
        }

        panel.RefreshStatus("QuickSetup\n" +
            "状态：" + data.MainState + "\n" +
            "回合：" + data.RoundIndex + " / " + data.RoundPhase + "\n" +
            "阵眼：" + (data.FormationSnapshot == null ? "待生成" : BattleBContentCatalog.ResolveFormationLabel(data.FormationSnapshot.eyePosition)));
    }
}
