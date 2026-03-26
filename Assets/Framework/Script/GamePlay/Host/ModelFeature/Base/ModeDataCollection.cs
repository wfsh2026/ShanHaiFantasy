using UnityEngine;

public sealed class ModeDataCollection {
    private readonly HashList<IModeData> modeDatas = new HashList<IModeData>(8);

    public void AddData<T>(AbsModeManager manager) where T : IModeData, new() {
        T data = new T();
        data.OnInit(manager);
        modeDatas.Add(data);
    }

    public T GetData<T>() where T : IModeData {
        if (!modeDatas.TryGetValue(typeof(T), out IModeData data)) {
            Debug.LogError("ModeDataCollection get data failed.");
            return default(T);
        }

        return (T) data;
    }

    public void RemoveModeData<T>() where T : IModeData {
        if (!modeDatas.TryGetValue(typeof(T), out IModeData data)) {
            return;
        }

        data.OnClear();
        modeDatas.Remove(typeof(T));
    }

    public void Clear() {
        for (int i = modeDatas.Count - 1; i >= 0; --i) {
            modeDatas[i].OnClear();
        }

        modeDatas.Clear();
    }
}
