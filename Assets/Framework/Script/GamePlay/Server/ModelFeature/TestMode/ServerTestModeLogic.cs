using UnityEngine;

public sealed class ServerTestModeLogic : AbsModeLogic {
    private ServerTestModeData data;
    private float logTimer;

    public override void OnInit() {
        data = manager.GetData<ServerTestModeData>();
        logTimer = 0f;
    }

    public override void OnClear() {
        data = null;
        logTimer = 0f;
    }

    public void Tick(float delta) {
        if (data == null) {
            return;
        }

        data.Tick(delta);
        logTimer += delta;
        if (logTimer < 1f) {
            return;
        }

        logTimer = 0f;
        Debug.Log("ServerTestModeLogic Tick, RunningTime = " + data.RunningTime);
    }
}
