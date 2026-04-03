using UnityEngine;
using UnityEngine.UI;

public sealed class BattleQuickSetupPanel : UIPanelBase {
    private readonly string[] buttonLabels = { "跳阶段", "换环境", "换阵眼", "气运+1", "剑意+1", "前位", "中位", "后位", "刷新养成" };
    private Button[] buttons;
    private Text statusText;
    private BattleQuickSetupPanelController controller;

    protected override void OnCreate() {
        Image root = UIRuntimeWidgetFactory.CreateImage("QuickSetupRoot", RectTransform, new Color(0.05f, 0.07f, 0.11f, 0.92f));
        RectTransform rootRect = root.rectTransform;
        rootRect.anchorMin = new Vector2(0f, 1f);
        rootRect.anchorMax = new Vector2(0f, 1f);
        rootRect.pivot = new Vector2(0f, 1f);
        rootRect.sizeDelta = new Vector2(184f, 392f);
        rootRect.anchoredPosition = new Vector2(18f, -286f);
        buttons = new Button[buttonLabels.Length];
        for (int i = 0; i < buttons.Length; i++) {
            buttons[i] = UIRuntimeWidgetFactory.CreateButton("QuickBtn_" + i, rootRect, buttonLabels[i], new Vector2(156f, 28f));
            RectTransform rect = buttons[i].GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            rect.anchoredPosition = new Vector2(14f, -14f - i * 34f);
            int index = i;
            buttons[i].onClick.RemoveAllListeners();
            buttons[i].onClick.AddListener(() => { if (controller != null) controller.Trigger(index); });
        }

        statusText = UIRuntimeWidgetFactory.CreateText("Status", rootRect, "QuickSetup", 14, TextAnchor.UpperLeft, Color.white);
        statusText.fontStyle = FontStyle.Normal;
        RectTransform statusRect = statusText.rectTransform;
        statusRect.anchorMin = new Vector2(0f, 0f);
        statusRect.anchorMax = new Vector2(1f, 0f);
        statusRect.pivot = new Vector2(0f, 0f);
        statusRect.sizeDelta = new Vector2(-28f, 52f);
        statusRect.anchoredPosition = new Vector2(14f, 12f);
    }

    protected override void OnOpen() { controller = new BattleQuickSetupPanelController(this); controller.Bind(); }
    protected override void OnClose() { if (controller != null) { controller.Unbind(); controller = null; } }
    public void RefreshStatus(string text) { if (statusText != null) statusText.text = text; }
}
