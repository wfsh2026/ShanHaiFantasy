using UnityEngine;

public abstract class ServerModeManager : AbsModeManager {
    protected override void OnInit() {
        base.OnInit();
    }

    protected override void OnRemove() {
        base.OnRemove();
    }

    protected override void OnChangeStage(string stageName) {
        Debug.Log("Server mode stage change: " + stageName);
    }
}
