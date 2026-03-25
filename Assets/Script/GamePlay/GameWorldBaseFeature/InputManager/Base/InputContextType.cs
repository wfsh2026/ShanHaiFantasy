/// <summary>
/// 输入上下文定义。
/// 用来区分全局输入、UI 输入、弹窗输入和模式输入等场景。
/// </summary>
public enum InputContextType {
    Global = 0,
    UI = 1,
    Popup = 2,
    Mode = 3,
    Block = 4,
}
