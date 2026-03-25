/// <summary>
/// 属性调整弹窗的返回结果。
/// </summary>
public sealed class AdjustAttrPopupResult : UIResultBase {
    public bool IsConfirm;
    public RoleAttrType AttrType;
    public RoleAttrOperationType OperationType;
    public int Value;
}
