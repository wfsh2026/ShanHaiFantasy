using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public sealed class UILayerRoot {
    private readonly GameObject rootObject;
    private readonly Canvas rootCanvas;
    private readonly Dictionary<UILayer, RectTransform> layerDict = new Dictionary<UILayer, RectTransform>();

    public Transform RootTransform {
        get {
            return rootObject.transform;
        }
    }

    public UILayerRoot() {
        rootObject = new GameObject("UIRoot");
        rootCanvas = rootObject.AddComponent<Canvas>();
        rootCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        rootCanvas.sortingOrder = 500;
        rootObject.AddComponent<GraphicRaycaster>();

        CanvasScaler scaler = rootObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 1f;

        RectTransform rootRect = rootObject.GetComponent<RectTransform>();
        UIRuntimeWidgetFactory.StretchRect(rootRect);

        Object.DontDestroyOnLoad(rootObject);
        EnsureEventSystem();
        CreateLayer(UILayer.Background);
        CreateLayer(UILayer.Normal);
        CreateLayer(UILayer.Popup);
        CreateLayer(UILayer.HUD);
        CreateLayer(UILayer.Toast);
        CreateLayer(UILayer.Guide);
        CreateLayer(UILayer.System);
    }

    public RectTransform GetLayer(UILayer layer) {
        return layerDict[layer];
    }

    public void Dispose() {
        if (rootObject != null) {
            Object.Destroy(rootObject);
        }
    }

    private void CreateLayer(UILayer layer) {
        GameObject layerObject = new GameObject(layer.ToString());
        RectTransform rect = layerObject.AddComponent<RectTransform>();
        rect.SetParent(rootObject.transform, false);
        UIRuntimeWidgetFactory.StretchRect(rect);
        layerDict[layer] = rect;
    }

    private void EnsureEventSystem() {
        EventSystem eventSystem = Object.FindObjectOfType<EventSystem>();
        if (eventSystem != null) {
            return;
        }

        GameObject eventObject = new GameObject("EventSystem");
        eventObject.AddComponent<EventSystem>();
        eventObject.AddComponent<StandaloneInputModule>();
        Object.DontDestroyOnLoad(eventObject);
    }
}
