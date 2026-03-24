public sealed class InputHandlerEntry {
    public InputContextType ContextType {
        get;
        private set;
    }

    public IInputHandler Handler {
        get;
        private set;
    }

    public int Priority {
        get;
        private set;
    }

    public InputHandlerEntry(InputContextType contextType, IInputHandler handler, int priority) {
        ContextType = contextType;
        Handler = handler;
        Priority = priority;
    }
}
