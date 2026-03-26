using UnityEngine;

public abstract class AbsModeMonoLogic : MonoBehaviour {
    protected IModeManager manager;
    protected GameWorld gameWorld;

    public Transform MyTransform {
        get;
        private set;
    }

    public void Init(IModeManager modeManager) {
        manager = modeManager;
        gameWorld = modeManager.GameWorld;
        MyTransform = transform;
        OnInit();
    }

    public void Clear() {
        OnClear();
        manager = null;
        gameWorld = null;
        MyTransform = null;
    }

    public virtual void OnInit() {
    }

    public virtual void OnClear() {
    }
}
