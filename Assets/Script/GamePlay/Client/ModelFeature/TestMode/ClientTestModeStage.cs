using UnityEngine;

public sealed class ClientTestModeStage : AbsModeStage {
    public override void OnEnter() {
        ClientTestModeData data = manager.GetData<ClientTestModeData>();
        if (data != null) {
            data.MarkStageEnter();
            data.SetStageName(GetType().Name);
        }

        Debug.Log("ClientTestModeStage Enter");
    }

    public override void OnUpdate(float delta) {
    }

    public override void OnQuit() {
        Debug.Log("ClientTestModeStage Quit");
    }
}
