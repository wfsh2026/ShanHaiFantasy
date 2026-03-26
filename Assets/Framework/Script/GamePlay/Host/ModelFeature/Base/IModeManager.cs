public interface IModeManager {
    GameWorld GameWorld { get; }
    IModeStage RunningStage { get; }

    bool RunningStageIs<T>() where T : IModeStage;
    T GetData<T>() where T : IModeData;
    T GetDataCollection<T>() where T : IModeData;
    T GetCommonData<T>() where T : IModeData;
    T GetStage<T>() where T : IModeStage;
    T GetLogic<T>() where T : IModeLogic;
    void ChangeStage<T>() where T : IModeStage;
    void NextStage();
    void GameOver();
}
