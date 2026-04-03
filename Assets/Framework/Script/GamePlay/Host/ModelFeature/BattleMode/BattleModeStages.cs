using UnityEngine;

public abstract class BattleModeStageBase : AbsModeStage {
public override void OnEnter() {
        BattleModeData data = manager.GetData<BattleModeData>();
        if (data != null && data.IsAuthoritative) {
            data.MarkStageEnter(GetType().Name);
        }

        Debug.Log(GetType().Name + " Enter");
    }

    public override void OnUpdate(float delta) {
    }

    public override void OnQuit() {
        Debug.Log(GetType().Name + " Quit");
    }
}

public sealed class BattleMatchInitStage : BattleModeStageBase {
}

public sealed class BattleHeroSelectStage : BattleModeStageBase {
}

public sealed class BattleSectRevealStage : BattleModeStageBase {
}

public sealed class BattleRoundLoopStage : BattleModeStageBase {
}

public sealed class BattleGameOverStage : BattleModeStageBase {
}
