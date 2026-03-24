public sealed class ServerTestModeData : AbsModeData {
    public int StageEnterCount {
        get;
        private set;
    }

    public float RunningTime {
        get;
        private set;
    }

    public void MarkStageEnter() {
        StageEnterCount += 1;
    }

    public void Tick(float delta) {
        RunningTime += delta;
    }
}
