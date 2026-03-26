using UnityEngine;

public sealed class ServerTestModeStage : AbsModeStage {
    public override void OnEnter() {
        ServerTestModeData data = manager.GetData<ServerTestModeData>();
        if (data != null) {
            data.MarkStageEnter();
        }

        Debug.Log("ServerTestModeStage Enter");
    }

    public override void OnUpdate(float delta) {
    }

    public override void OnQuit() {
        Debug.Log("ServerTestModeStage Quit");
    }
}
