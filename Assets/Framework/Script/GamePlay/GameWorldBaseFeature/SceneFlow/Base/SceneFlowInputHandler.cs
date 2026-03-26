/// <summary>
/// 场景流转相关的输入处理器。
/// 当前只负责把重载场景按键转成 SceneFlow 请求。
/// </summary>
public sealed class SceneFlowInputHandler : IInputHandler {
    private readonly SceneFlowManager sceneFlowManager;

    public SceneFlowInputHandler(SceneFlowManager targetSceneFlowManager) {
        sceneFlowManager = targetSceneFlowManager;
    }

    public bool HandleInput(InputManager inputManager, InputState inputState) {
        if (sceneFlowManager == null || inputManager == null || inputState == null) {
            return false;
        }

        if (!inputManager.GetButtonDown(InputActionId.ReloadScene)) {
            return false;
        }

        return sceneFlowManager.ReloadCurrentScene("ReloadFromInput");
    }
}
