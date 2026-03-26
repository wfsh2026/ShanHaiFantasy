public abstract class AbsModeLogic : IModeLogic {
    protected IModeManager manager;
    protected GameWorld gameWorld;

    void IModeLogic.OnInit(IModeManager modeManager) {
        manager = modeManager;
        gameWorld = modeManager.GameWorld;
        OnInit();
    }

    void IModeLogic.OnClear() {
        OnClear();
        manager = null;
        gameWorld = null;
    }

    public virtual void OnInit() {
    }

    public virtual void OnClear() {
    }
}
