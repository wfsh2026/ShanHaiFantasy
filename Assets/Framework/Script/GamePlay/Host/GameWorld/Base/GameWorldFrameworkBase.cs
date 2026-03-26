using System;
using System.Collections.Generic;

public delegate void UpdateDelegate(float delta);
public delegate void LateUpdateDelegate();

public sealed class TimerRegister : IDisposable {
    private readonly List<TimerEntry> timerEntries = new List<TimerEntry>();

    public void Dispose() {
        timerEntries.Clear();
    }

    public void AddTimer(float delay, Action callback) {
        if (callback == null) {
            return;
        }

        timerEntries.Add(new TimerEntry(delay, callback));
    }

    public void RemoveTimer(Action callback) {
        for (int i = timerEntries.Count - 1; i >= 0; --i) {
            if (timerEntries[i].IsEquals(callback)) {
                timerEntries.RemoveAt(i);
            }
        }
    }

    public void OnUpdate(float delta) {
        for (int i = timerEntries.Count - 1; i >= 0; --i) {
            TimerEntry entry = timerEntries[i];
            entry.Elapsed += delta;
            if (entry.Elapsed >= entry.Delay) {
                timerEntries.RemoveAt(i);
                if (entry.Callback != null) {
                    entry.Callback.Invoke();
                }
                continue;
            }

            timerEntries[i] = entry;
        }
    }

    private struct TimerEntry {
        public TimerEntry(float delay, Action callback) {
            Delay = delay;
            Callback = callback;
            Elapsed = 0f;
        }

        public float Delay;
        public float Elapsed;
        public Action Callback;

        public bool IsEquals(Action callback) {
            return Callback == callback;
        }
    }
}

public class UpdateRegister : IDisposable {
    private List<WrapperUpdate> updates;
    private List<WrapperUpdate> removeUpdates;

    public UpdateRegister() {
        updates = new List<WrapperUpdate>(32);
        removeUpdates = new List<WrapperUpdate>(8);
    }

    public void Dispose() {
        updates.Clear();
        removeUpdates.Clear();
    }

    public void OnUpdate(float delta) {
        TickRemoveUpdates();
        TickUpdates(delta);
    }

    public void Register(UpdateDelegate handler, bool isCheckProfiler, bool isRunUnloading) {
        if (SearchUpdateIndex(handler) != -1) {
            return;
        }

        WrapperUpdate update = new WrapperUpdate(handler);
        update.IsCheckProfiler = isCheckProfiler;
        update.IsRunUnloading = isRunUnloading;
        updates.Add(update);
    }

    public void Unregister(UpdateDelegate handler) {
        int index = SearchUpdateIndex(handler);
        if (index < 0) {
            return;
        }

        WrapperUpdate update = updates[index];
        update.IsRemove = true;
        updates[index] = update;
        removeUpdates.Add(update);
    }

    private void TickUpdates(float delta) {
        for (int i = 0; i < updates.Count; ++i) {
            if (i >= updates.Count) {
                return;
            }

            WrapperUpdate update = updates[i];
            update.Invoke(delta);
        }
    }

    private void TickRemoveUpdates() {
        if (removeUpdates.Count == 0) {
            return;
        }

        for (int i = 0; i < removeUpdates.Count; ++i) {
            WrapperUpdate update = removeUpdates[i];
            int index = SearchUpdateIndex(update.Handler);
            if (index >= 0) {
                updates.RemoveAt(index);
            }
        }

        removeUpdates.Clear();
    }

    private int SearchUpdateIndex(UpdateDelegate handler) {
        for (int i = 0; i < updates.Count; ++i) {
            if (updates[i].IsEquals(handler)) {
                return i;
            }
        }

        return -1;
    }

    private struct WrapperUpdate {
        public WrapperUpdate(UpdateDelegate handler) {
            Handler = handler;
            IsRemove = false;
            IsCheckProfiler = false;
            IsRunUnloading = false;
        }

        public bool IsRemove;
        public bool IsCheckProfiler;
        public bool IsRunUnloading;
        public UpdateDelegate Handler;

        public void Invoke(float delta) {
            if (!IsRemove) {
                if (Handler != null) {
                    Handler.Invoke(delta);
                }
            }
        }

        public bool IsEquals(UpdateDelegate handler) {
            return Handler == handler;
        }
    }
}

public sealed class LateUpdateRegister : IDisposable {
    private readonly List<LateUpdateEntry> lateUpdates = new List<LateUpdateEntry>();

    public void Dispose() {
        lateUpdates.Clear();
    }

    public void Register(LateUpdateDelegate handler, bool isCheckProfiler) {
        if (SearchIndex(handler) != -1) {
            return;
        }

        lateUpdates.Add(new LateUpdateEntry(handler, isCheckProfiler));
    }

    public void UnRegister(LateUpdateDelegate handler) {
        int index = SearchIndex(handler);
        if (index >= 0) {
            lateUpdates.RemoveAt(index);
        }
    }

    public void OnLateUpdate() {
        for (int i = 0; i < lateUpdates.Count; ++i) {
            lateUpdates[i].Invoke();
        }
    }

    private int SearchIndex(LateUpdateDelegate handler) {
        for (int i = 0; i < lateUpdates.Count; ++i) {
            if (lateUpdates[i].Handler == handler) {
                return i;
            }
        }

        return -1;
    }

    private struct LateUpdateEntry {
        public LateUpdateDelegate Handler;
        public bool IsCheckProfiler;

        public LateUpdateEntry(LateUpdateDelegate handler, bool isCheckProfiler) {
            Handler = handler;
            IsCheckProfiler = isCheckProfiler;
        }

        public void Invoke() {
            if (Handler != null) {
                Handler.Invoke();
            }
        }
    }
}

public interface IGameWorldFeature {
    void Init(GameWorld gameWorld);
    void Clear();
}

public abstract class AbsGameWorldFeature : IGameWorldFeature {
    protected GameWorld gameWorld;

    public void Init(GameWorld world) {
        gameWorld = world;
        OnInit();
    }

    public void Clear() {
        OnRemove();
        gameWorld = null;
    }

    protected virtual void OnInit() {
    }

    protected virtual void OnRemove() {
    }
}

public abstract class AbsBaseGameWorldFeature : AbsGameWorldFeature {
}

public abstract class AbsExtendGameWorldFeature : AbsGameWorldFeature {
}

public sealed class GameWorldFeatures : IDisposable {
    private GameWorld gameWorld;
    private List<IGameWorldFeature> features;
    private Dictionary<Type, IGameWorldFeature> featureDict;

    public GameWorldFeatures(GameWorld world) {
        gameWorld = world;
        features = new List<IGameWorldFeature>(16);
        featureDict = new Dictionary<Type, IGameWorldFeature>(16);
    }

    public void Dispose() {
        for (int i = 0; i < features.Count; ++i) {
            features[i].Clear();
        }

        features.Clear();
        featureDict.Clear();
        gameWorld = null;
    }

    public void AddFeature(IGameWorldFeature feature) {
        if (feature == null) {
            return;
        }

        if (featureDict.ContainsKey(feature.GetType())) {
            return;
        }

        features.Add(feature);
        featureDict.Add(feature.GetType(), feature);
        feature.Init(gameWorld);
    }

    public void AddFeature<T>() where T : IGameWorldFeature, new() {
        if (featureDict.ContainsKey(typeof(T))) {
            return;
        }

        IGameWorldFeature feature = new T();
        features.Add(feature);
        featureDict.Add(typeof(T), feature);
        feature.Init(gameWorld);
    }

    public void RemoveFeature<T>() where T : IGameWorldFeature {
        if (!featureDict.TryGetValue(typeof(T), out IGameWorldFeature feature)) {
            return;
        }

        features.Remove(feature);
        featureDict.Remove(typeof(T));
        feature.Clear();
    }

    public void RemoveFeature(IGameWorldFeature feature) {
        if (feature == null) {
            return;
        }

        if (features.Remove(feature)) {
            featureDict.Remove(feature.GetType());
            feature.Clear();
        }
    }

    public T GetFeature<T>() where T : class, IGameWorldFeature {
        if (featureDict.TryGetValue(typeof(T), out IGameWorldFeature feature)) {
            return feature as T;
        }

        return null;
    }
}
