using System.Globalization;

public sealed class AdjustAttrPopupController : UIControllerBase<AdjustAttrPopup> {
    private ClientTestModeData data;

    public AdjustAttrPopupController(AdjustAttrPopup targetPanel) : base(targetPanel) {
    }

    public override void Bind() {
        data = ResolveModeData();
        RefreshRequest();
    }

    public override void Unbind() {
        if (data != null) {
            data.ClearAdjustAttrRequest();
        }

        data = null;
    }

    public void Confirm(string inputText) {
        string title;
        RoleAttrType attrType;
        RoleAttrOperationType operationType;
        int defaultValue;
        bool hasRequest = TryGetRequest(out title, out attrType, out operationType, out defaultValue);

        int value = defaultValue;
        if (!string.IsNullOrEmpty(inputText)) {
            int parsedValue;
            if (int.TryParse(inputText, NumberStyles.Integer, CultureInfo.InvariantCulture, out parsedValue) && parsedValue > 0) {
                value = parsedValue;
            }
        }

        AdjustAttrPopupResult result = new AdjustAttrPopupResult();
        result.IsConfirm = hasRequest;
        result.AttrType = attrType;
        result.OperationType = operationType;
        result.Value = value;
        ClosePanel(result);
    }

    public void Cancel() {
        string title;
        RoleAttrType attrType;
        RoleAttrOperationType operationType;
        int defaultValue;
        bool hasRequest = TryGetRequest(out title, out attrType, out operationType, out defaultValue);

        AdjustAttrPopupResult result = new AdjustAttrPopupResult();
        result.IsConfirm = false;
        result.AttrType = attrType;
        result.OperationType = operationType;
        result.Value = hasRequest ? defaultValue : 0;
        ClosePanel(result);
    }

    private void RefreshRequest() {
        string title;
        RoleAttrType attrType;
        RoleAttrOperationType operationType;
        int defaultValue;
        bool hasRequest = TryGetRequest(out title, out attrType, out operationType, out defaultValue);
        if (!hasRequest) {
            panel.ShowDefault();
            return;
        }

        panel.RefreshRequest(title, GetOperationText(attrType, operationType), defaultValue);
    }

    private bool TryGetRequest(out string title, out RoleAttrType attrType, out RoleAttrOperationType operationType, out int defaultValue) {
        title = "Adjust Attribute";
        attrType = RoleAttrType.HP;
        operationType = RoleAttrOperationType.Add;
        defaultValue = 0;

        if (data == null || !data.HasAdjustAttrRequest) {
            return false;
        }

        title = string.IsNullOrEmpty(data.AdjustAttrRequestTitle) ? title : data.AdjustAttrRequestTitle;
        attrType = data.AdjustAttrRequestAttrType;
        operationType = data.AdjustAttrRequestOperationType;
        defaultValue = data.AdjustAttrRequestDefaultValue;
        return true;
    }

    private ClientTestModeData ResolveModeData() {
        if (gameWorld == null) {
            return null;
        }

        ClientTestModeManager modeManager = gameWorld.GetExtendFeature<ClientTestModeManager>();
        if (modeManager == null) {
            return null;
        }

        return modeManager.GetData<ClientTestModeData>();
    }

    private static string GetOperationText(RoleAttrType attrType, RoleAttrOperationType operationType) {
        string attrName = attrType == RoleAttrType.HP ? "HP" : "MP";
        string actionName = operationType == RoleAttrOperationType.Add ? "Add" : "Reduce";
        return actionName + " " + attrName;
    }
}
