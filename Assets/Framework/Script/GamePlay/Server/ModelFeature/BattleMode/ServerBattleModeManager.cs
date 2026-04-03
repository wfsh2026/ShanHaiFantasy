using UnityEngine;

public sealed class ServerBattleModeManager : ServerModeManager {
    private BattleModeData data;
    private ServerBattleModeLogic logic;

    protected override void OnInit() {
        base.OnInit();

        data = AddData<BattleModeData>();
        AddLogic<ServerBattleModeLogic>();
        AddStage<BattleMatchInitStage>();
        AddStage<BattleHeroSelectStage>();
        AddStage<BattleSectRevealStage>();
        AddStage<BattleRoundLoopStage>();
        AddStage<BattleGameOverStage>();
        ChangeStage<BattleMatchInitStage>();

        logic = GetLogic<ServerBattleModeLogic>();
        if (logic != null) {
            logic.BroadcastCurrentSnapshot();
        }
    }

    protected override void OnRemove() {
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
}
