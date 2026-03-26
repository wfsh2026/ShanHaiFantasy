public interface IModeStage {
    void OnInit(IModeManager manager);
    void OnClear();
    void OnEnter();
    void OnQuit();
    void OnUpdate(float delta);
}
