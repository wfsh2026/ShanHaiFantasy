using System;

/// <summary>
/// UI 管理器接口。
/// 对外只暴露统一的界面开关和查询能力。
/// </summary>
public interface IUIManager {
    void Open<TPanel>() where TPanel : UIPanelBase;
    void OpenForResult<TPanel, TResult>(Action<TResult> callback)
        where TPanel : UIPanelBase
        where TResult : UIResultBase;
    void Close<TPanel>() where TPanel : UIPanelBase;
    void CloseTop();
    bool IsOpen<TPanel>() where TPanel : UIPanelBase;
    TPanel GetPanel<TPanel>() where TPanel : UIPanelBase;
}
