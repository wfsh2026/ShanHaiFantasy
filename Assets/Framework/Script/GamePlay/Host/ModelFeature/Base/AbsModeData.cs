public abstract class AbsModeData : IModeData {
    protected IModeManager manager;
    protected GameWorld gameWorld;

    void IModeData.OnInit(IModeManager modeManager) {
        manager = modeManager;
        gameWorld = modeManager.GameWorld;
        OnInit();
    }

    void IModeData.OnClear() {
        OnClear();
        manager = null;
        gameWorld = null;
    }

    public virtual void OnInit() {
    }

    public virtual void OnClear() {
    }
}
