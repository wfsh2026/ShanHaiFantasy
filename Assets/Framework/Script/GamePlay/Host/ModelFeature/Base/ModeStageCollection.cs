using UnityEngine;

public sealed class ModeStageCollection {
    private readonly HashList<IModeStage> stages = new HashList<IModeStage>(8);

    public void AddStage<T>(AbsModeManager manager) where T : IModeStage, new() {
        T stage = new T();
        stage.OnInit(manager);
        stages.Add(stage);
    }

    public T GetStage<T>() where T : IModeStage {
        if (!stages.TryGetValue(typeof(T), out IModeStage stage)) {
            Debug.LogError("ModeStageCollection get stage failed.");
            return default(T);
        }

        return (T) stage;
    }

    public IModeStage GetStage(string name) {
        if (!stages.TryGetValue(name, out IModeStage stage)) {
            Debug.LogError("ModeStageCollection get stage by name failed.");
            return null;
        }

        return stage;
    }

    public IModeStage GetNextStage(string currentStageName) {
        int index = stages.Find(currentStageName);
        if (index < 0) {
            index = -1;
        }

        ++index;
        if (index >= stages.Count) {
            Debug.LogError("ModeStageCollection next stage failed.");
            return null;
        }

        return stages[index];
    }

    public void RemoveStage<T>() where T : IModeStage {
        if (!stages.TryGetValue(typeof(T), out IModeStage stage)) {
            return;
        }

        stage.OnClear();
        stages.Remove(typeof(T));
    }

    public void Clear() {
        for (int i = stages.Count - 1; i >= 0; --i) {
            stages[i].OnClear();
        }

        stages.Clear();
    }
}
