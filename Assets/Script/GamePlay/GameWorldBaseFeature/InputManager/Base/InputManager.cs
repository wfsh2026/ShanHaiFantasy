public sealed class InputManager {
    private readonly InputDeviceAdapter inputDeviceAdapter;
    private readonly InputState inputState;
    private readonly InputMap inputMap;
    private readonly InputContextManager contextManager;
    private readonly InputRouter inputRouter;
    private bool isInputEnable;

    public InputManager() {
        inputDeviceAdapter = new InputDeviceAdapter();
        inputState = new InputState();
        inputMap = new InputMap();
        contextManager = new InputContextManager();
        inputRouter = new InputRouter();
        isInputEnable = true;
    }

    public InputState InputState {
        get {
            return inputState;
        }
    }

    public void Tick(float delta) {
        if (!isInputEnable) {
            inputState.BeginFrame();
            return;
        }

        inputDeviceAdapter.UpdateState(inputMap, inputState);
        inputRouter.Route(this, inputState, contextManager);
    }

    public void SetInputEnable(bool isEnable) {
        isInputEnable = isEnable;
    }

    public void SetContextActive(InputContextType contextType, bool isActive) {
        contextManager.SetContextActive(contextType, isActive);
    }

    public bool HasContext(InputContextType contextType) {
        return contextManager.HasContext(contextType);
    }

    public void RegisterHandler(InputContextType contextType, IInputHandler handler, int priority) {
        inputRouter.RegisterHandler(contextType, handler, priority);
    }

    public void UnregisterHandler(InputContextType contextType, IInputHandler handler) {
        inputRouter.UnregisterHandler(contextType, handler);
    }

    public bool GetButtonDown(InputActionId actionId) {
        return inputState.GetButtonDown(actionId);
    }

    public bool GetButton(InputActionId actionId) {
        return inputState.GetButton(actionId);
    }

    public bool GetButtonUp(InputActionId actionId) {
        return inputState.GetButtonUp(actionId);
    }

    public float GetAxis(InputActionId actionId) {
        return inputState.GetAxis(actionId);
    }

    public void Clear() {
        inputRouter.Clear();
        inputState.BeginFrame();
    }
}
