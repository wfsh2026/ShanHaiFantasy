using UnityEngine;

public sealed class ClientBattleModeManager : ClientModeManager {
    private BattleModeData data;
    private ClientBattleModeLogic logic;

    protected override void OnInit() {
        base.OnInit();

        data = AddData<BattleModeData>();
        AddStage<BattleMatchInitStage>();
        AddStage<BattleHeroSelectStage>();
        AddStage<BattleSectRevealStage>();
        AddStage<BattleRoundLoopStage>();
        AddStage<BattleGameOverStage>();
        ChangeStage<BattleMatchInitStage>();

        AddLogic<ClientBattleModeLogic>();
        logic = GetLogic<ClientBattleModeLogic>();
        OpenModeUI();
    }

    protected override void OnRemove() {
        CloseModeUI();
        logic = null;
        data = null;
        base.OnRemove();
    }

    protected override void OnUpdate(float delta) {
        base.OnUpdate(delta);
        if (logic != null) {
            logic.Tick(delta);
        }
    }

    protected override void OnChangeStage(string stageName) {
        base.OnChangeStage(stageName);
        if (data != null) {
            data.StageNameValue.SetValue(stageName);
        }
    }

    private void OpenModeUI() {
        UIManager.Instance.Open<BattleA2Panel>();
    }

    private void CloseModeUI() {
        UIManager.Instance.Close<BattleA2Panel>();
    }
}
