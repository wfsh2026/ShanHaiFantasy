using UnityEngine;

public abstract class AbsModeManager : AbsExtendGameWorldFeature, IModeManager {
    protected ModeStageCollection stageCollection = new ModeStageCollection();
    private readonly ModeLogicCollection logicCollection = new ModeLogicCollection();
    private readonly ModeDataCollection dataCollection = new ModeDataCollection();
    protected IModeStage runningStage;
    private IModeData modeData;

    public GameWorld GameWorld {
        get {
            return gameWorld;
        }
    }

    public IModeStage RunningStage {
        get {
            return runningStage;
        }
    }

    protected override void OnInit() {
        base.OnInit();
        gameWorld.AddUpdate(OnUpdate);
    }

    protected override void OnRemove() {
        if (gameWorld != null) {
            gameWorld.RemoveUpdate(OnUpdate);
        }

        stageCollection.Clear();
        logicCollection.Clear();
        dataCollection.Clear();

        if (modeData != null) {
            modeData.OnClear();
            modeData = null;
        }

        runningStage = null;
        base.OnRemove();
    }

    protected virtual void OnUpdate(float delta) {
        if (runningStage != null) {
            runningStage.OnUpdate(delta);
        }
    }

    public bool RunningStageIs<T>() where T : IModeStage {
        return runningStage is T;
    }

    public T GetData<T>() where T : IModeData {
        if (!(modeData is T)) {
            Debug.LogError("Mode manager data type mismatch.");
            return default(T);
        }

        return (T) modeData;
    }

    public T GetDataCollection<T>() where T : IModeData {
        return dataCollection.GetData<T>();
    }

    public T GetCommonData<T>() where T : IModeData {
        if (!(modeData is T)) {
            return default(T);
        }

        return (T) modeData;
    }

    public T GetStage<T>() where T : IModeStage {
        return stageCollection.GetStage<T>();
    }

    public T GetLogic<T>() where T : IModeLogic {
        return logicCollection.GetLogic<T>();
    }

    public virtual void ChangeStage<T>() where T : IModeStage {
        if (runningStage != null) {
            runningStage.OnQuit();
        }

        runningStage = stageCollection.GetStage<T>();
        if (runningStage == null) {
            return;
        }

        runningStage.OnEnter();
        OnChangeStage(runningStage.GetType().Name);
    }

    public void NextStage() {
        string currentStageName = "None";
        if (runningStage != null) {
            currentStageName = runningStage.GetType().Name;
            runningStage.OnQuit();
        }

        runningStage = stageCollection.GetNextStage(currentStageName);
        if (runningStage == null) {
            return;
        }

        runningStage.OnEnter();
        OnChangeStage(runningStage.GetType().Name);
    }

    public void GameOver() {
        gameWorld.RemoveExtendFeature(this);
    }

    protected T AddData<T>() where T : IModeData, new() {
        modeData = new T();
        modeData.OnInit(this);
        return (T) modeData;
    }

    protected T AddDataCollection<T>() where T : IModeData, new() {
        dataCollection.AddData<T>(this);
        return dataCollection.GetData<T>();
    }

    protected void AddStage<T>() where T : IModeStage, new() {
        stageCollection.AddStage<T>(this);
    }

    protected void AddLogic<T>() where T : IModeLogic, new() {
        logicCollection.AddLogic<T>(this);
    }

    protected void RemoveStage<T>() where T : IModeStage {
        stageCollection.RemoveStage<T>();
    }

    protected void RemoveLogic<T>() where T : IModeLogic {
        logicCollection.RemoveLogic<T>();
    }

    protected void ChangeStage(string stageName) {
        if (runningStage != null) {
            runningStage.OnQuit();
        }

        runningStage = stageCollection.GetStage(stageName);
        if (runningStage == null) {
            return;
        }

        runningStage.OnEnter();
        OnChangeStage(runningStage.GetType().Name);
    }

    protected virtual void OnChangeStage(string stageName) {
    }
}
