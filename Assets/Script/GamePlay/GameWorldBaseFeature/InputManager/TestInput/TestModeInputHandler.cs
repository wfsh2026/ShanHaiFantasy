public sealed class TestModeInputHandler : IInputHandler {
    private readonly TestModeInputService inputService;

    public TestModeInputHandler(TestModeInputService targetInputService) {
        inputService = targetInputService;
    }

    public bool HandleInput(InputManager inputManager, InputState inputState) {
        if (inputManager == null || inputState == null || inputService == null) {
            return false;
        }

        bool isHandled = false;

        if (inputManager.GetButtonDown(InputActionId.AddHP)) {
            inputService.RequestAddHP();
            isHandled = true;
        }

        if (inputManager.GetButtonDown(InputActionId.ReduceHP)) {
            inputService.RequestReduceHP();
            isHandled = true;
        }

        if (inputManager.GetButtonDown(InputActionId.AddMP)) {
            inputService.RequestAddMP();
            isHandled = true;
        }

        if (inputManager.GetButtonDown(InputActionId.ReduceMP)) {
            inputService.RequestReduceMP();
            isHandled = true;
        }

        if (inputManager.GetButtonDown(InputActionId.NextStage)) {
            inputService.RequestNextStage();
            isHandled = true;
        }

        return isHandled;
    }
}
