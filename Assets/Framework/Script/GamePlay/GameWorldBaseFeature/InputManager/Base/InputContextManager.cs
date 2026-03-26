using System.Collections.Generic;

/// <summary>
/// 输入上下文管理器。
/// 统一维护当前激活的上下文集合，并按预设优先级输出。
/// </summary>
public sealed class InputContextManager {
    private static readonly InputContextType[] PRIORITY_ORDER = new[] {
        InputContextType.Block,
        InputContextType.Popup,
        InputContextType.UI,
        InputContextType.Global,
        InputContextType.Mode,
    };

    private readonly HashSet<InputContextType> activeContexts = new HashSet<InputContextType>();

    public void SetContextActive(InputContextType contextType, bool isActive) {
        if (isActive) {
            activeContexts.Add(contextType);
        } else {
            activeContexts.Remove(contextType);
        }
    }

    public bool HasContext(InputContextType contextType) {
        return activeContexts.Contains(contextType);
    }

    public void GetActiveContexts(List<InputContextType> outputList) {
        outputList.Clear();
        for (int i = 0; i < PRIORITY_ORDER.Length; ++i) {
            InputContextType contextType = PRIORITY_ORDER[i];
            if (activeContexts.Contains(contextType)) {
                outputList.Add(contextType);
            }
        }
    }
}
