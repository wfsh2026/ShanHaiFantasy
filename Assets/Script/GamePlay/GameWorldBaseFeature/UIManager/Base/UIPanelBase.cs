using System;
using UnityEngine;

public abstract class UIPanelBase : MonoBehaviour {
    private UIManager uiManager;
    private UIWindowConfig config;
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

    public UIManager UIManager {
        get {
            return uiManager;
        }
    }

    public GameWorld GameWorld {
        get {
            return uiManager != null ? uiManager.GameWorld : null;
        }
    }

    internal void Setup(UIManager targetUIManager, UIWindowConfig windowConfig) {
        uiManager = targetUIManager;
        config = windowConfig;
    }

    internal void SetResultCallback(Action<UIResultBase> callback) {
        resultCallback = callback;
    }

    internal void OpenPanel() {
        gameObject.SetActive(true);
        if (!isCreated) {
            isCreated = true;
            OnCreate();
        }

        OnOpen();
        OnShow();
    }

    internal void ClosePanel() {
        OnHide();
        OnClose();
    }

    internal void DestroyPanel() {
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
    protected virtual void OnOpen() {
    }
    protected virtual void OnShow() {
    }
    protected virtual void OnHide() {
    }
    protected virtual void OnClose() {
    }
    protected virtual void OnDestroyPanel() {
    }
    public virtual void Refresh(UIStateBase state) {
    }
}
