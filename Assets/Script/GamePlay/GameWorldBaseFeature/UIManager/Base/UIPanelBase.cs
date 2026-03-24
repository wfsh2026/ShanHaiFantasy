using System;
using UnityEngine;

public abstract class UIPanelBase : MonoBehaviour {
    private ClientUIFeatureManager uiManager;
    private UIWindowConfig config;
    private UIPresenterBase presenter;
    private Action<UIResultBase> resultCallback;
    private bool isCreated;

    public string UIId {
        get {
            return config.UIId;
        }
    }

    public UIWindowConfig Config {
        get {
            return config;
        }
    }

    public RectTransform RectTransform {
        get {
            return transform as RectTransform;
        }
    }

    protected TPresenter GetPresenter<TPresenter>() where TPresenter : UIPresenterBase {
        return presenter as TPresenter;
    }

    internal void Setup(ClientUIFeatureManager targetUIManager, UIWindowConfig windowConfig, UIPresenterBase panelPresenter) {
        uiManager = targetUIManager;
        config = windowConfig;
        presenter = panelPresenter;
        presenter.Setup(this, targetUIManager, targetUIManager.GameWorld);
    }

    internal void SetResultCallback(Action<UIResultBase> callback) {
        resultCallback = callback;
    }

    internal void OpenPanel(UIOpenDataBase openData) {
        gameObject.SetActive(true);
        if (!isCreated) {
            isCreated = true;
            OnCreate();
        }

        presenter.Open(openData);
        OnOpen(openData);
        OnShow();
    }

    internal void ClosePanel() {
        presenter.Close();
        OnHide();
        OnClose();
    }

    internal void DestroyPanel() {
        presenter.DestroyPresenter();
        OnDestroyPanel();
    }

    internal void InvokeResult(UIResultBase result) {
        if (resultCallback != null) {
            resultCallback.Invoke(result);
            resultCallback = null;
        }
    }

    protected void CloseSelf() {
        if (uiManager != null) {
            uiManager.ClosePanel(this, null);
        }
    }

    protected void CloseSelf(UIResultBase result) {
        if (uiManager != null) {
            uiManager.ClosePanel(this, result);
        }
    }

    protected abstract void OnCreate();
    protected virtual void OnOpen(UIOpenDataBase openData) {
    }
    protected virtual void OnShow() {
    }
    protected virtual void OnHide() {
    }
    protected virtual void OnClose() {
    }
    protected virtual void OnDestroyPanel() {
    }
    public abstract void Refresh(UIStateBase state);
}
