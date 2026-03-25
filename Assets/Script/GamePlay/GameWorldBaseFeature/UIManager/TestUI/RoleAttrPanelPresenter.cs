public sealed class RoleAttrPanelPresenter : UIPresenterBase {
    private const int QUICK_HP_VALUE = 10;
    private const int QUICK_MP_VALUE = 5;

    private RoleAttrUIService uiService;

    protected override void OnOpen(UIOpenDataBase openData) {
        uiService = new RoleAttrUIService(gameWorld);
        uiService.AddDataListener(OnDataChanged);
        RefreshState();
    }

    protected override void OnClose() {
        Cleanup();
    }

    protected override void OnDispose() {
        Cleanup();
    }

    public void OnClickClose() {
        PlayUIButtonSound();
        ClosePanel();
    }

    public void OnClickAddHP() {
        if (uiService != null) {
            uiService.PlayUIButtonSound();
            uiService.RequestAttrChange(RoleAttrType.HP, RoleAttrOperationType.Add, QUICK_HP_VALUE);
        }
    }

    public void OnClickReduceHP() {
        if (uiService != null) {
            uiService.PlayUIButtonSound();
            uiService.RequestAttrChange(RoleAttrType.HP, RoleAttrOperationType.Reduce, QUICK_HP_VALUE);
        }
    }

    public void OnClickAddMP() {
        if (uiService != null) {
            uiService.PlayUIButtonSound();
            uiService.RequestAttrChange(RoleAttrType.MP, RoleAttrOperationType.Add, QUICK_MP_VALUE);
        }
    }

    public void OnClickReduceMP() {
        if (uiService != null) {
            uiService.PlayUIButtonSound();
            uiService.RequestAttrChange(RoleAttrType.MP, RoleAttrOperationType.Reduce, QUICK_MP_VALUE);
        }
    }

    public void OnClickPopupChange(RoleAttrType attrType, RoleAttrOperationType operationType) {
        PlayUIButtonSound();
        AdjustAttrPopupOpenData openData = new AdjustAttrPopupOpenData();
        openData.Title = uiService == null ? "Adjust Attribute" : uiService.GetOperationText(attrType, operationType);
        openData.AttrType = attrType;
        openData.OperationType = operationType;
        openData.DefaultValue = attrType == RoleAttrType.HP ? QUICK_HP_VALUE : QUICK_MP_VALUE;

        OpenPanelForResult<AdjustAttrPopup, AdjustAttrPopupOpenData, AdjustAttrPopupResult>(openData, OnPopupResult);
    }

    public void OnClickNextStage() {
        if (uiService != null) {
            uiService.PlayUIButtonSound();
            uiService.RequestNextStage();
        }
    }

    public void OnClickReloadScene() {
        if (uiService != null) {
            uiService.RequestReloadUITestScene();
        }
    }

    public void OnClickToggleEmitter() {
        if (uiService != null) {
            uiService.PlayUIButtonSound();
            uiService.ToggleDemoEmitterObject();
        }
    }

    private void OnPopupResult(AdjustAttrPopupResult result) {
        if (result == null || !result.IsConfirm || uiService == null) {
            return;
        }

        uiService.RequestAttrChange(result.AttrType, result.OperationType, result.Value);
    }

    private void OnDataChanged() {
        RefreshState();
    }

    private void RefreshState() {
        if (uiService == null) {
            return;
        }

        ClientTestModeData data = uiService.GetModeData();
        if (data == null) {
            return;
        }

        RoleAttrPanelOpenData openData = GetOpenData<RoleAttrPanelOpenData>();
        RoleAttrPanelUIState state = new RoleAttrPanelUIState();
        state.Title = data.RoleName + " Attribute Test";
        state.OpenSourceText = "OpenSource: " + (openData == null || string.IsNullOrEmpty(openData.OpenSource) ? "ModeAutoOpen" : openData.OpenSource);
        state.StageName = "Stage: " + data.StageName + "  |  EnterCount: " + data.StageEnterCount;
        state.RunningTimeText = "RunningTime: " + data.RunningTime.ToString("F1") + "s";
        state.HPText = "HP: " + data.HP + " / " + data.MaxHP;
        state.MPText = "MP: " + data.MP + " / " + data.MaxMP;
        state.CanChangeHP = true;
        state.CanChangeMP = true;
        Refresh(state);
    }

    private void Cleanup() {
        if (uiService != null) {
            uiService.RemoveDataListener(OnDataChanged);
            uiService = null;
        }
    }

    private void PlayUIButtonSound() {
        if (uiService != null) {
            uiService.PlayUIButtonSound();
        }
    }
}
