public abstract class AbsModeStage : IModeStage {
    protected IModeManager manager;
    protected GameWorld gameWorld;

    void IModeStage.OnInit(IModeManager modeManager) {
        manager = modeManager;
        gameWorld = modeManager.GameWorld;
        OnInit();
    }

    void IModeStage.OnClear() {
        OnClear();
        manager = null;
        gameWorld = null;
    }

    public virtual void OnInit() {
    }

    public virtual void OnClear() {
    }

    public virtual void OnEnter() {
    }

    public virtual void OnQuit() {
    }

    public virtual void OnUpdate(float delta) {
    }
}
