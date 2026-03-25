using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class ClientUIFeatureManager : AbsExtendGameWorldFeature, IUIManager {
    private readonly Dictionary<string, List<UIPanelBase>> activePanels = new Dictionary<string, List<UIPanelBase>>();
    private readonly Dictionary<string, Stack<UIPanelBase>> cachedPanels = new Dictionary<string, Stack<UIPanelBase>>();
    private readonly List<UIPanelBase> backStack = new List<UIPanelBase>();

    private UILayerRoot layerRoot;
    private UIRegistry registry;
    private UIAssetLoaderAdapter loader;

    public GameWorld GameWorld {
        get {
            return gameWorld;
        }
    }

    protected override void OnInit() {
        layerRoot = new UILayerRoot();
        registry = new UIRegistry();
        loader = new UIAssetLoaderAdapter();
        RegisterWindows();
    }

    protected override void OnRemove() {
        CloseAllPanels();

        activePanels.Clear();
        cachedPanels.Clear();
        backStack.Clear();
        registry = null;
        loader = null;

        if (layerRoot != null) {
            layerRoot.Dispose();
            layerRoot = null;
        }
    }

    public void Open<TPanel>() where TPanel : UIPanelBase {
        Open<TPanel, UIOpenDataBase>(null);
    }

    public void Open<TPanel, TOpenData>(TOpenData openData)
        where TPanel : UIPanelBase
        where TOpenData : UIOpenDataBase {
        UIWindowConfig config = registry.GetConfig<TPanel>();
        if (config == null) {
            Debug.LogError("UI config is not registered.");
            return;
        }

        if (config.OpenMode == UIOpenMode.Single) {
            UIPanelBase reusePanel = GetReusablePanel(config);
            if (reusePanel != null) {
                ActivatePanel(reusePanel, config, openData, null);
                return;
            }
        }

        CreatePanelInstance<TPanel>(config, openData, null);
    }

    public void OpenForResult<TPanel, TOpenData, TResult>(TOpenData openData, Action<TResult> callback)
        where TPanel : UIPanelBase
        where TOpenData : UIOpenDataBase
        where TResult : UIResultBase {
        UIWindowConfig config = registry.GetConfig<TPanel>();
        if (config == null) {
            Debug.LogError("UI config is not registered.");
            return;
        }

        Action<UIResultBase> wrappedCallback = null;
        if (callback != null) {
            wrappedCallback = (result) => {
                callback.Invoke(result as TResult);
            };
        }

        if (config.OpenMode == UIOpenMode.Single) {
            UIPanelBase reusePanel = GetReusablePanel(config);
            if (reusePanel != null) {
                ActivatePanel(reusePanel, config, openData, wrappedCallback);
                return;
            }
        }

        CreatePanelInstance<TPanel>(config, openData, wrappedCallback);
    }

    public void Close<TPanel>() where TPanel : UIPanelBase {
        UIWindowConfig config = registry.GetConfig<TPanel>();
        if (config == null) {
            return;
        }

        if (!activePanels.TryGetValue(config.UIId, out List<UIPanelBase> list) || list.Count == 0) {
            return;
        }

        ClosePanel(list[list.Count - 1], null);
    }

    public void CloseTop() {
        for (int i = backStack.Count - 1; i >= 0; --i) {
            UIPanelBase panel = backStack[i];
            if (panel != null && panel.gameObject != null && panel.gameObject.activeSelf) {
                ClosePanel(panel, null);
                return;
            }
        }
    }

    public bool IsOpen<TPanel>() where TPanel : UIPanelBase {
        UIWindowConfig config = registry.GetConfig<TPanel>();
        if (config == null) {
            return false;
        }

        return activePanels.TryGetValue(config.UIId, out List<UIPanelBase> list) && list.Count > 0;
    }

    public TPanel GetPanel<TPanel>() where TPanel : UIPanelBase {
        UIWindowConfig config = registry.GetConfig<TPanel>();
        if (config == null) {
            return null;
        }

        if (!activePanels.TryGetValue(config.UIId, out List<UIPanelBase> list) || list.Count == 0) {
            return null;
        }

        return list[list.Count - 1] as TPanel;
    }

    internal void ClosePanel(UIPanelBase panel, UIResultBase result) {
        if (panel == null || panel.Config == null) {
            return;
        }

        RemoveActivePanel(panel);
        RemoveBackStackPanel(panel);

        panel.ClosePanel();
        panel.InvokeResult(result);

        if (panel.Config.CacheMode == UICacheMode.DestroyOnClose) {
            panel.DestroyPanel();
            UnityEngine.Object.Destroy(panel.gameObject);
            return;
        }

        panel.gameObject.SetActive(false);
        CachePanel(panel);
    }

    private void RegisterWindows() {
        registry.Register<RoleAttrHUDPanel, RoleAttrHUDPresenter>(
            "RoleAttrHUD",
            UILayer.HUD,
            UICacheMode.Permanent,
            UIOpenMode.Single,
            string.Empty,
            false,
            false,
            false);
        registry.Register<RoleAttrPanel, RoleAttrPanelPresenter>(
            "RoleAttrPanel",
            UILayer.Normal,
            UICacheMode.HideOnClose,
            UIOpenMode.Single,
            string.Empty,
            true,
            true,
            true);
        registry.Register<AdjustAttrPopup, AdjustAttrPopupPresenter>(
            "AdjustAttrPopup",
            UILayer.Popup,
            UICacheMode.DestroyOnClose,
            UIOpenMode.Multi,
            string.Empty,
            false,
            true,
            true);
        registry.Register<LoadingPanel, LoadingPanelPresenter>(
            "LoadingPanel",
            UILayer.System,
            UICacheMode.DestroyOnClose,
            UIOpenMode.Single,
            string.Empty,
            false,
            false,
            true);
    }

    public void CloseByLayer(UILayer layer) {
        List<UIPanelBase> closeList = new List<UIPanelBase>();
        foreach (KeyValuePair<string, List<UIPanelBase>> pair in activePanels) {
            List<UIPanelBase> panelList = pair.Value;
            for (int i = 0; i < panelList.Count; ++i) {
                UIPanelBase panel = panelList[i];
                if (panel != null && panel.Config != null && panel.Config.Layer == layer) {
                    closeList.Add(panel);
                }
            }
        }

        for (int i = 0; i < closeList.Count; ++i) {
            UIPanelBase panel = closeList[i];
            if (panel != null) {
                ClosePanel(panel, null);
            }
        }
    }

    private UIPanelBase GetReusablePanel(UIWindowConfig config) {
        if (activePanels.TryGetValue(config.UIId, out List<UIPanelBase> activeList) && activeList.Count > 0) {
            return activeList[activeList.Count - 1];
        }

        if (cachedPanels.TryGetValue(config.UIId, out Stack<UIPanelBase> cachedList) && cachedList.Count > 0) {
            return cachedList.Pop();
        }

        return null;
    }

    private void ActivatePanel(UIWindowConfig config, UIPanelBase panel, UIOpenDataBase openData, Action<UIResultBase> resultCallback) {
        panel.transform.SetParent(layerRoot.GetLayer(config.Layer), false);
        panel.SetResultCallback(resultCallback);
        AddActivePanel(panel);
        AddBackStackPanel(panel);
        panel.OpenPanel(openData);
    }

    private void ActivatePanel(UIPanelBase panel, UIWindowConfig config, UIOpenDataBase openData, Action<UIResultBase> resultCallback) {
        ActivatePanel(config, panel, openData, resultCallback);
    }

    private void CreatePanelInstance<TPanel>(UIWindowConfig config, UIOpenDataBase openData, Action<UIResultBase> resultCallback)
        where TPanel : UIPanelBase {
        loader.LoadPanel(config, layerRoot.GetLayer(config.Layer), (loadedObject) => {
            GameObject panelObject = loadedObject;
            if (panelObject == null) {
                panelObject = new GameObject(config.UIId);
                RectTransform rectTransform = panelObject.AddComponent<RectTransform>();
                rectTransform.SetParent(layerRoot.GetLayer(config.Layer), false);
                UIRuntimeWidgetFactory.StretchRect(rectTransform);
            }

            TPanel panel = panelObject.GetComponent<TPanel>();
            if (panel == null) {
                panel = panelObject.AddComponent<TPanel>();
            }

            UIPresenterBase presenter = Activator.CreateInstance(config.PresenterType) as UIPresenterBase;
            panel.Setup(this, config, presenter);
            ActivatePanel(config, panel, openData, resultCallback);
        });
    }

    private void AddActivePanel(UIPanelBase panel) {
        if (!activePanels.TryGetValue(panel.UIId, out List<UIPanelBase> list)) {
            list = new List<UIPanelBase>();
            activePanels[panel.UIId] = list;
        }

        if (!list.Contains(panel)) {
            list.Add(panel);
        }
    }

    private void RemoveActivePanel(UIPanelBase panel) {
        if (!activePanels.TryGetValue(panel.UIId, out List<UIPanelBase> list)) {
            return;
        }

        list.Remove(panel);
        if (list.Count == 0) {
            activePanels.Remove(panel.UIId);
        }
    }

    private void CachePanel(UIPanelBase panel) {
        if (!cachedPanels.TryGetValue(panel.UIId, out Stack<UIPanelBase> list)) {
            list = new Stack<UIPanelBase>();
            cachedPanels[panel.UIId] = list;
        }

        list.Push(panel);
    }

    private void AddBackStackPanel(UIPanelBase panel) {
        if (!panel.Config.UseBackStack) {
            return;
        }

        RemoveBackStackPanel(panel);
        backStack.Add(panel);
    }

    private void RemoveBackStackPanel(UIPanelBase panel) {
        for (int i = backStack.Count - 1; i >= 0; --i) {
            if (backStack[i] == panel) {
                backStack.RemoveAt(i);
            }
        }
    }

    private void CloseAllPanels() {
        List<UIPanelBase> panels = new List<UIPanelBase>();
        foreach (KeyValuePair<string, List<UIPanelBase>> pair in activePanels) {
            panels.AddRange(pair.Value);
        }
        foreach (KeyValuePair<string, Stack<UIPanelBase>> pair in cachedPanels) {
            panels.AddRange(pair.Value);
        }

        for (int i = 0; i < panels.Count; ++i) {
            UIPanelBase panel = panels[i];
            if (panel == null) {
                continue;
            }

            panel.ClosePanel();
            panel.DestroyPanel();
            if (panel.gameObject != null) {
                UnityEngine.Object.Destroy(panel.gameObject);
            }
        }
    }
}
