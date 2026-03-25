public sealed class ClientTestModeData : AbsModeData {
    private const int DEFAULT_MAX_HP = 100;
    private const int DEFAULT_MAX_MP = 60;
    private const string DEFAULT_ROLE_NAME = "Test Hero";
    private const string DEFAULT_STAGE_NAME = "ClientTestModeStage";

    public BindableValue<string> RoleNameValue {
        get;
        private set;
    } = new BindableValue<string>(DEFAULT_ROLE_NAME);

    public BindableValue<string> StageNameValue {
        get;
        private set;
    } = new BindableValue<string>(DEFAULT_STAGE_NAME);

    public BindableValue<int> StageEnterCountValue {
        get;
        private set;
    } = new BindableValue<int>(0);

    public BindableValue<float> RunningTimeValue {
        get;
        private set;
    } = new BindableValue<float>(0f);

    public BindableValue<RoleAttrValue> HPValue {
        get;
        private set;
    } = new BindableValue<RoleAttrValue>(new RoleAttrValue(DEFAULT_MAX_HP, DEFAULT_MAX_HP));

    public BindableValue<RoleAttrValue> MPValue {
        get;
        private set;
    } = new BindableValue<RoleAttrValue>(new RoleAttrValue(DEFAULT_MAX_MP, DEFAULT_MAX_MP));

    public string RoleName {
        get {
            return RoleNameValue.Value;
        }
    }

    public string StageName {
        get {
            return StageNameValue.Value;
        }
    }

    public int StageEnterCount {
        get {
            return StageEnterCountValue.Value;
        }
    }

    public float RunningTime {
        get {
            return RunningTimeValue.Value;
        }
    }

    public int HP {
        get {
            return HPValue.Value.Current;
        }
    }

    public int MaxHP {
        get {
            return HPValue.Value.Max;
        }
    }

    public int MP {
        get {
            return MPValue.Value.Current;
        }
    }

    public int MaxMP {
        get {
            return MPValue.Value.Max;
        }
    }

    public bool HasAdjustAttrRequest {
        get;
        private set;
    }

    public string AdjustAttrRequestTitle {
        get;
        private set;
    }

    public RoleAttrType AdjustAttrRequestAttrType {
        get;
        private set;
    }

    public RoleAttrOperationType AdjustAttrRequestOperationType {
        get;
        private set;
    }

    public int AdjustAttrRequestDefaultValue {
        get;
        private set;
    }

    public override void OnInit() {
        RoleNameValue.SetValue(DEFAULT_ROLE_NAME);
        StageNameValue.SetValue(DEFAULT_STAGE_NAME);
        StageEnterCountValue.SetValue(0);
        RunningTimeValue.SetValue(0f);
        HPValue.SetValue(new RoleAttrValue(DEFAULT_MAX_HP, DEFAULT_MAX_HP));
        MPValue.SetValue(new RoleAttrValue(DEFAULT_MAX_MP, DEFAULT_MAX_MP));
        ClearAdjustAttrRequest();
    }

    public override void OnClear() {
        RoleNameValue.ClearListeners();
        StageNameValue.ClearListeners();
        StageEnterCountValue.ClearListeners();
        RunningTimeValue.ClearListeners();
        HPValue.ClearListeners();
        MPValue.ClearListeners();
    }

    public void MarkStageEnter() {
        StageEnterCountValue.SetValue(StageEnterCount + 1);
    }

    public void SetStageName(string stageName) {
        if (string.IsNullOrEmpty(stageName)) {
            return;
        }

        StageNameValue.SetValue(stageName);
    }

    public void Tick(float delta) {
        RunningTimeValue.SetValue(RunningTime + delta);
    }

    public void ChangeHP(int deltaValue) {
        int newValue = ClampValue(HP + deltaValue, 0, MaxHP);
        HPValue.SetValue(new RoleAttrValue(newValue, MaxHP));
    }

    public void ChangeMP(int deltaValue) {
        int newValue = ClampValue(MP + deltaValue, 0, MaxMP);
        MPValue.SetValue(new RoleAttrValue(newValue, MaxMP));
    }

    public void SetAdjustAttrRequest(string title, RoleAttrType attrType, RoleAttrOperationType operationType, int defaultValue) {
        HasAdjustAttrRequest = true;
        AdjustAttrRequestTitle = title;
        AdjustAttrRequestAttrType = attrType;
        AdjustAttrRequestOperationType = operationType;
        AdjustAttrRequestDefaultValue = defaultValue;
    }

    public void ClearAdjustAttrRequest() {
        HasAdjustAttrRequest = false;
        AdjustAttrRequestTitle = string.Empty;
        AdjustAttrRequestAttrType = RoleAttrType.HP;
        AdjustAttrRequestOperationType = RoleAttrOperationType.Add;
        AdjustAttrRequestDefaultValue = 0;
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
}
