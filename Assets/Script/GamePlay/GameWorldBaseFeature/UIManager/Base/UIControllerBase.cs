using System;

public abstract class UIControllerBase<TPanel> where TPanel : UIPanelBase {
    protected readonly TPanel panel;
    protected readonly UIManager uiManager;
    protected readonly GameWorld gameWorld;

    protected UIControllerBase(TPanel targetPanel) {
        panel = targetPanel;
        uiManager = targetPanel != null ? targetPanel.UIManager : null;
        gameWorld = targetPanel != null ? targetPanel.GameWorld : null;
    }

    public virtual void Bind() {
    }

    public virtual void Unbind() {
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

    protected void OpenPanel<TTargetPanel>()
        where TTargetPanel : UIPanelBase {
        if (uiManager != null) {
            uiManager.Open<TTargetPanel>();
        }
    }

    protected void OpenPanelForResult<TTargetPanel, TResult>(Action<TResult> callback)
        where TTargetPanel : UIPanelBase
        where TResult : UIResultBase {
        if (uiManager != null) {
            uiManager.OpenForResult<TTargetPanel, TResult>(callback);
        }
    }
}
