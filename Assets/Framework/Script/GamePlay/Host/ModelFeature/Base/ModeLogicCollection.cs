using UnityEngine;

public sealed class ModeLogicCollection {
    private readonly HashList<IModeLogic> logics = new HashList<IModeLogic>(8);

    public void AddLogic<T>(AbsModeManager manager) where T : IModeLogic, new() {
        T logic = new T();
        logic.OnInit(manager);
        logics.Add(logic);
    }

    public T GetLogic<T>() where T : IModeLogic {
        if (!logics.TryGetValue(typeof(T), out IModeLogic logic)) {
            Debug.LogError("ModeLogicCollection get logic failed.");
            return default(T);
        }

        return (T) logic;
    }

    public void RemoveLogic<T>() where T : IModeLogic {
        if (!logics.TryGetValue(typeof(T), out IModeLogic logic)) {
            return;
        }

        logic.OnClear();
        logics.Remove(typeof(T));
    }

    public void Clear() {
        for (int i = logics.Count - 1; i >= 0; --i) {
            logics[i].OnClear();
        }

        logics.Clear();
    }
}
