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
