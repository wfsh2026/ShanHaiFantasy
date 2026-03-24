using System.Collections.Generic;

public sealed class InputRouter {
    private readonly Dictionary<InputContextType, List<InputHandlerEntry>> handlerMap = new Dictionary<InputContextType, List<InputHandlerEntry>>();
    private readonly List<InputContextType> activeContextsBuffer = new List<InputContextType>(8);

    public void RegisterHandler(InputContextType contextType, IInputHandler handler, int priority) {
        if (handler == null) {
            return;
        }

        List<InputHandlerEntry> handlerList;
        if (!handlerMap.TryGetValue(contextType, out handlerList)) {
            handlerList = new List<InputHandlerEntry>(4);
            handlerMap.Add(contextType, handlerList);
        }

        for (int i = 0; i < handlerList.Count; ++i) {
            if (handlerList[i].Handler == handler) {
                return;
            }
        }

        handlerList.Add(new InputHandlerEntry(contextType, handler, priority));
        handlerList.Sort(CompareEntryPriority);
    }

    public void UnregisterHandler(InputContextType contextType, IInputHandler handler) {
        if (handler == null) {
            return;
        }

        List<InputHandlerEntry> handlerList;
        if (!handlerMap.TryGetValue(contextType, out handlerList)) {
            return;
        }

        for (int i = handlerList.Count - 1; i >= 0; --i) {
            if (handlerList[i].Handler == handler) {
                handlerList.RemoveAt(i);
            }
        }
    }

    public void Route(InputManager inputManager, InputState inputState, InputContextManager contextManager) {
        if (inputManager == null || inputState == null || contextManager == null) {
            return;
        }

        contextManager.GetActiveContexts(activeContextsBuffer);
        for (int i = 0; i < activeContextsBuffer.Count; ++i) {
            InputContextType contextType = activeContextsBuffer[i];
            if (!handlerMap.TryGetValue(contextType, out List<InputHandlerEntry> handlerList)) {
                continue;
            }

            for (int j = 0; j < handlerList.Count; ++j) {
                IInputHandler handler = handlerList[j].Handler;
                if (handler != null && handler.HandleInput(inputManager, inputState)) {
                    return;
                }
            }
        }
    }

    public void Clear() {
        handlerMap.Clear();
        activeContextsBuffer.Clear();
    }

    private static int CompareEntryPriority(InputHandlerEntry leftEntry, InputHandlerEntry rightEntry) {
        return rightEntry.Priority.CompareTo(leftEntry.Priority);
    }
}
