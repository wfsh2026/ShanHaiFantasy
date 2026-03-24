using System;

public interface IUIManager {
    void Open<TPanel>() where TPanel : UIPanelBase;
    void Open<TPanel, TOpenData>(TOpenData openData) where TPanel : UIPanelBase where TOpenData : UIOpenDataBase;
    void OpenForResult<TPanel, TOpenData, TResult>(TOpenData openData, Action<TResult> callback)
        where TPanel : UIPanelBase
        where TOpenData : UIOpenDataBase
        where TResult : UIResultBase;
    void Close<TPanel>() where TPanel : UIPanelBase;
    void CloseTop();
    bool IsOpen<TPanel>() where TPanel : UIPanelBase;
    TPanel GetPanel<TPanel>() where TPanel : UIPanelBase;
}
