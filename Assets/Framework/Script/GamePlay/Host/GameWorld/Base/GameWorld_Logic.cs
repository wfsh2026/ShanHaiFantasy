public partial class GameWorld {
    private bool isDispose;
    private TimerRegister timerRegister;
    private UpdateRegister updateRegister;
    private UpdateRegister fixedUpdateRegister;
    private LateUpdateRegister lateUpdateRegister;
    private GameWorldFeatures baseFeatures;
    private GameWorldFeatures extendFeatures;
    private GameType gameType;
    private GameStateType gameState;

    private void OnInit() {
        isDispose = false;
        gameType = GameType.Standard;
        gameState = GameStateType.Loading;
        timerRegister = new TimerRegister();
        updateRegister = new UpdateRegister();
        fixedUpdateRegister = new UpdateRegister();
        lateUpdateRegister = new LateUpdateRegister();
        baseFeatures = new GameWorldFeatures(this);
        extendFeatures = new GameWorldFeatures(this);
    }

    private void Dispose() {
        if (isDispose) {
            return;
        }

        isDispose = true;
        if (timerRegister != null) {
            timerRegister.Dispose();
        }
        if (updateRegister != null) {
            updateRegister.Dispose();
        }
        if (fixedUpdateRegister != null) {
            fixedUpdateRegister.Dispose();
        }
        if (lateUpdateRegister != null) {
            lateUpdateRegister.Dispose();
        }
        if (extendFeatures != null) {
            extendFeatures.Dispose();
        }
        if (baseFeatures != null) {
            baseFeatures.Dispose();
        }

        timerRegister = null;
        updateRegister = null;
        fixedUpdateRegister = null;
        lateUpdateRegister = null;
        extendFeatures = null;
        baseFeatures = null;
        gameState = GameStateType.None;
    }

    public void OnUpdate(float delta) {
        if (isDispose) {
            return;
        }

        timerRegister.OnUpdate(delta);
        updateRegister.OnUpdate(delta);
    }

    public void OnFixedUpdate(float delta) {
        if (isDispose) {
            return;
        }

        fixedUpdateRegister.OnUpdate(delta);
    }

    public void OnLateUpdate() {
        if (isDispose) {
            return;
        }

        lateUpdateRegister.OnLateUpdate();
    }

    public void AddUpdate(UpdateDelegate update) {
        if (isDispose) {
            return;
        }

        updateRegister.Register(update, false, false);
    }

    public void AddUpdate(UpdateDelegate update, bool isCheckProfiler, bool isRunUnloading = false) {
        if (isDispose) {
            return;
        }

        updateRegister.Register(update, isCheckProfiler, isRunUnloading);
    }

    public void RemoveUpdate(UpdateDelegate update) {
        if (isDispose) {
            return;
        }

        updateRegister.Unregister(update);
    }

    public void AddFixedUpdate(UpdateDelegate fixedUpdate) {
        if (isDispose) {
            return;
        }

        fixedUpdateRegister.Register(fixedUpdate, false, false);
    }

    public void AddFixedUpdate(UpdateDelegate fixedUpdate, bool isCheckProfiler, bool isRunUnloading = false) {
        if (isDispose) {
            return;
        }

        fixedUpdateRegister.Register(fixedUpdate, isCheckProfiler, isRunUnloading);
    }

    public void RemoveFixedUpdate(UpdateDelegate fixedUpdate) {
        if (isDispose) {
            return;
        }

        fixedUpdateRegister.Unregister(fixedUpdate);
    }

    public void AddLateUpdate(LateUpdateDelegate lateUpdate) {
        if (isDispose) {
            return;
        }

        lateUpdateRegister.Register(lateUpdate, false);
    }

    public void AddLateUpdate(LateUpdateDelegate lateUpdate, bool isCheckProfiler) {
        if (isDispose) {
            return;
        }

        lateUpdateRegister.Register(lateUpdate, isCheckProfiler);
    }

    public void RemoveLateUpdate(LateUpdateDelegate lateUpdate) {
        if (isDispose) {
            return;
        }

        lateUpdateRegister.UnRegister(lateUpdate);
    }

    public void AddBaseFeature<T>() where T : AbsBaseGameWorldFeature, new() {
        if (isDispose) {
            return;
        }

        baseFeatures.AddFeature<T>();
    }
    
    public void RemoveBaseFeature<T>() where T : AbsBaseGameWorldFeature {
        if (isDispose) {
            return;
        }

        baseFeatures.RemoveFeature<T>();
    }

    public void RemoveBaseFeature(AbsBaseGameWorldFeature feature) {
        if (isDispose) {
            return;
        }

        baseFeatures.RemoveFeature(feature);
    }

    public T GetBaseFeature<T>() where T : class, IGameWorldFeature {
        if (isDispose) {
            return null;
        }

        return baseFeatures.GetFeature<T>();
    }

    public void AddExtendFeature<T>() where T : AbsExtendGameWorldFeature, new() {
        if (isDispose) {
            return;
        }

        extendFeatures.AddFeature<T>();
    }

    public void RemoveExtendFeature<T>() where T : AbsExtendGameWorldFeature {
        if (isDispose) {
            return;
        }

        extendFeatures.RemoveFeature<T>();
    }

    public void RemoveExtendFeature(AbsExtendGameWorldFeature feature) {
        if (isDispose) {
            return;
        }

        extendFeatures.RemoveFeature(feature);
    }

    public T GetExtendFeature<T>() where T : class, IGameWorldFeature {
        if (isDispose) {
            return null;
        }

        return extendFeatures.GetFeature<T>();
    }

    public bool IsDispose {
        get {
            return isDispose;
        }
    }

    public GameType GameType {
        get {
            return gameType;
        }
    }
}
