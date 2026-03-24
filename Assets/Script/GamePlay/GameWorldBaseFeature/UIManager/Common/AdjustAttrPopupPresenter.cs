using System.Globalization;

public sealed class AdjustAttrPopupPresenter : UIPresenterBase {
    private RoleAttrUIService uiService;

    protected override void OnOpen(UIOpenDataBase openData) {
        uiService = new RoleAttrUIService(gameWorld);
        RefreshState();
    }

    protected override void OnClose() {
        uiService = null;
    }

    protected override void OnDispose() {
        uiService = null;
    }

    public void OnClickConfirm(string inputText) {
        AdjustAttrPopupOpenData openData = GetOpenData<AdjustAttrPopupOpenData>();
        if (openData == null) {
            ClosePanel();
            return;
        }

        int value = openData.DefaultValue;
        if (!string.IsNullOrEmpty(inputText)) {
            int parseValue;
            if (int.TryParse(inputText, NumberStyles.Integer, CultureInfo.InvariantCulture, out parseValue) && parseValue > 0) {
                value = parseValue;
            }
        }

        AdjustAttrPopupResult result = new AdjustAttrPopupResult();
        result.IsConfirm = true;
        result.AttrType = openData.AttrType;
        result.OperationType = openData.OperationType;
        result.Value = value;
        ClosePanel(result);
    }

    public void OnClickCancel() {
        AdjustAttrPopupOpenData openData = GetOpenData<AdjustAttrPopupOpenData>();
        AdjustAttrPopupResult result = new AdjustAttrPopupResult();
        result.IsConfirm = false;
        if (openData != null) {
            result.AttrType = openData.AttrType;
            result.OperationType = openData.OperationType;
            result.Value = openData.DefaultValue;
        }
        ClosePanel(result);
    }

    private void RefreshState() {
        AdjustAttrPopupOpenData openData = GetOpenData<AdjustAttrPopupOpenData>();
        if (openData == null) {
            return;
        }

        AdjustAttrPopupUIState state = new AdjustAttrPopupUIState();
        state.Title = string.IsNullOrEmpty(openData.Title) ? "调整属性" : openData.Title;
        state.Description = uiService == null ? string.Empty : uiService.GetOperationText(openData.AttrType, openData.OperationType);
        state.DefaultValue = openData.DefaultValue.ToString(CultureInfo.InvariantCulture);
        Refresh(state);
    }
}
