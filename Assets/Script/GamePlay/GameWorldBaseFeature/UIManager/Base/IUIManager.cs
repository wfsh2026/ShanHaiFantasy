using System;

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
