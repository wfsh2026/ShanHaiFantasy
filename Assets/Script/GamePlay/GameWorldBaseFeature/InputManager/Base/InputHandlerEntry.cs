/// <summary>
/// 输入 Handler 注册信息。
/// 记录上下文、处理器实例以及优先级。
/// </summary>
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
