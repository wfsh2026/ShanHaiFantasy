using System;

public abstract class UIPresenterBase {
    protected UIPanelBase panel;
    protected ClientUIFeatureManager uiManager;
    protected GameWorld gameWorld;
    private UIOpenDataBase currentOpenData;

    internal void Setup(UIPanelBase targetPanel, ClientUIFeatureManager targetUIManager, GameWorld targetGameWorld) {
        panel = targetPanel;
        uiManager = targetUIManager;
        gameWorld = targetGameWorld;
    }

    internal void Open(UIOpenDataBase openData) {
        currentOpenData = openData;
        OnOpen(openData);
    }

    internal void Close() {
        OnClose();
        currentOpenData = null;
    }

    internal void DestroyPresenter() {
        OnDispose();
        currentOpenData = null;
        panel = null;
        uiManager = null;
        gameWorld = null;
    }

    protected T GetOpenData<T>() where T : UIOpenDataBase {
        return currentOpenData as T;
    }

    protected void Refresh(UIStateBase state) {
        if (panel != null) {
            panel.Refresh(state);
        }
    }

    protected void ClosePanel() {
        if (uiManager != null && panel != null) {
            uiManager.ClosePanel(panel, null);
        }
    }

    protected void ClosePanel(UIResultBase result) {
        if (uiManager != null && panel != null) {
            uiManager.ClosePanel(panel, result);
        }
    }

    protected void OpenPanel<TPanel, TOpenData>(TOpenData openData)
        where TPanel : UIPanelBase
        where TOpenData : UIOpenDataBase {
        if (uiManager != null) {
            uiManager.Open<TPanel, TOpenData>(openData);
        }
    }

    protected void OpenPanelForResult<TPanel, TOpenData, TResult>(TOpenData openData, Action<TResult> callback)
        where TPanel : UIPanelBase
        where TOpenData : UIOpenDataBase
        where TResult : UIResultBase {
        if (uiManager != null) {
            uiManager.OpenForResult<TPanel, TOpenData, TResult>(openData, callback);
        }
    }

    protected abstract void OnOpen(UIOpenDataBase openData);
    protected virtual void OnClose() {
    }
    protected virtual void OnDispose() {
    }
}
