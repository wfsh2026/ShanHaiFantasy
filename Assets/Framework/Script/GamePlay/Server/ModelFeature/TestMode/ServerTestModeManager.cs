public sealed class ServerTestModeManager : ServerModeManager {
    private ServerTestModeLogic logic;

    protected override void OnInit() {
        base.OnInit();

        AddData<ServerTestModeData>();
        AddLogic<ServerTestModeLogic>();
        AddStage<ServerTestModeStage>();
        ChangeStage<ServerTestModeStage>();

        logic = GetLogic<ServerTestModeLogic>();
    }

    protected override void OnRemove() {
        logic = null;
        base.OnRemove();
    }

    protected override void OnUpdate(float delta) {
        base.OnUpdate(delta);
        if (logic != null) {
            logic.Tick(delta);
        }
    }
}
