using System;

public sealed class ClientTestModeData : AbsModeData {
    private const int DEFAULT_MAX_HP = 100;
    private const int DEFAULT_MAX_MP = 60;
    private const string DEFAULT_ROLE_NAME = "Test Hero";
    private const string DEFAULT_STAGE_NAME = "ClientTestModeStage";

    public event Action DataChanged;

    public int StageEnterCount {
        get;
        private set;
    }

    public string RoleName {
        get;
        private set;
    }

    public string StageName {
        get;
        private set;
    }

    public float RunningTime {
        get;
        private set;
    }

    public int HP {
        get;
        private set;
    }

    public int MaxHP {
        get;
        private set;
    }

    public int MP {
        get;
        private set;
    }

    public int MaxMP {
        get;
        private set;
    }

    public override void OnInit() {
        RoleName = DEFAULT_ROLE_NAME;
        StageName = DEFAULT_STAGE_NAME;
        MaxHP = DEFAULT_MAX_HP;
        HP = MaxHP;
        MaxMP = DEFAULT_MAX_MP;
        MP = MaxMP;
        RunningTime = 0f;
        StageEnterCount = 0;
        NotifyDataChanged();
    }

    public override void OnClear() {
        DataChanged = null;
    }

    public void MarkStageEnter() {
        StageEnterCount += 1;
        NotifyDataChanged();
    }

    public void SetStageName(string stageName) {
        if (string.IsNullOrEmpty(stageName)) {
            return;
        }

        StageName = stageName;
        NotifyDataChanged();
    }

    public void Tick(float delta) {
        RunningTime += delta;
        NotifyDataChanged();
    }

    public void ChangeHP(int deltaValue) {
        int newValue = HP + deltaValue;
        HP = ClampValue(newValue, 0, MaxHP);
        NotifyDataChanged();
    }

    public void ChangeMP(int deltaValue) {
        int newValue = MP + deltaValue;
        MP = ClampValue(newValue, 0, MaxMP);
        NotifyDataChanged();
    }

    private static int ClampValue(int value, int minValue, int maxValue) {
        if (value < minValue) {
            return minValue;
        }

        if (value > maxValue) {
            return maxValue;
        }

        return value;
    }

    private void NotifyDataChanged() {
        if (DataChanged != null) {
            DataChanged.Invoke();
        }
    }
}
