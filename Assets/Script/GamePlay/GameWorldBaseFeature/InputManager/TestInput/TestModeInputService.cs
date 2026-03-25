/// <summary>
/// 测试模式输入服务。
/// 把输入动作收口为模式逻辑调用，避免 Handler 直接处理业务细节。
/// </summary>
public sealed class TestModeInputService {
    private readonly GameWorld gameWorld;

    public TestModeInputService(GameWorld targetGameWorld) {
        gameWorld = targetGameWorld;
    }

    public void RequestAddHP() {
        ClientTestModeLogic logic = GetModeLogic();
        if (logic != null) {
            logic.AddHP(10);
        }
    }

    public void RequestReduceHP() {
        ClientTestModeLogic logic = GetModeLogic();
        if (logic != null) {
            logic.ReduceHP(10);
        }
    }

    public void RequestAddMP() {
        ClientTestModeLogic logic = GetModeLogic();
        if (logic != null) {
            logic.AddMP(5);
        }
    }

    public void RequestReduceMP() {
        ClientTestModeLogic logic = GetModeLogic();
        if (logic != null) {
            logic.ReduceMP(5);
        }
    }

    public void RequestNextStage() {
        ClientTestModeManager modeManager = GetModeManager();
        if (modeManager != null) {
            modeManager.NextStage();
        }
    }

    private ClientTestModeManager GetModeManager() {
        if (gameWorld == null) {
            return null;
        }

        return gameWorld.GetExtendFeature<ClientTestModeManager>();
    }

    private ClientTestModeLogic GetModeLogic() {
        ClientTestModeManager modeManager = GetModeManager();
        if (modeManager == null) {
            return null;
        }

        return modeManager.GetLogic<ClientTestModeLogic>();
    }
}
