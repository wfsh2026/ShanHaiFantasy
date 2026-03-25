using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Loading 面板。
/// 负责把场景切换上下文转换成可见的加载提示。
/// </summary>
public sealed class LoadingPanel : UIPanelBase {
    private Text titleText;
    private Text stepText;
    private Image progressFillImage;

    protected override void OnCreate() {
        Image mask = UIRuntimeWidgetFactory.CreateImage("Mask", RectTransform, new Color(0f, 0f, 0f, 0.82f));
        UIRuntimeWidgetFactory.StretchRect(mask.rectTransform);

        Image background = UIRuntimeWidgetFactory.CreateImage("Background", mask.rectTransform, new Color(0.1f, 0.13f, 0.2f, 0.96f));
        SetAnchor(background.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(480f, 180f), Vector2.zero);

        titleText = CreateLabel(background.rectTransform, "Title", new Vector2(0f, -35f), new Vector2(360f, 30f), 30, TextAnchor.MiddleCenter);
        stepText = CreateLabel(background.rectTransform, "StepText", new Vector2(0f, -82f), new Vector2(360f, 24f), 20, TextAnchor.MiddleCenter);

        Image progressBackground = UIRuntimeWidgetFactory.CreateImage("ProgressBackground", background.rectTransform, new Color(0f, 0f, 0f, 0.45f));
        SetAnchor(progressBackground.rectTransform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(360f, 20f), new Vector2(0f, 35f));

        progressFillImage = UIRuntimeWidgetFactory.CreateImage("ProgressFill", progressBackground.rectTransform, new Color(0.32f, 0.7f, 0.98f, 1f));
        UIRuntimeWidgetFactory.StretchRect(progressFillImage.rectTransform);
        progressFillImage.type = Image.Type.Filled;
        progressFillImage.fillMethod = Image.FillMethod.Horizontal;
        progressFillImage.fillOrigin = (int) Image.OriginHorizontal.Left;
        progressFillImage.fillAmount = 0f;
    }

    public override void Refresh(UIStateBase state) {
        LoadingPanelUIState loadingState = state as LoadingPanelUIState;
        if (loadingState == null) {
            return;
        }

        titleText.text = string.IsNullOrEmpty(loadingState.Title) ? "Loading" : loadingState.Title;
        stepText.text = string.IsNullOrEmpty(loadingState.StepText) ? "Preparing..." : loadingState.StepText;
        progressFillImage.fillAmount = Mathf.Clamp01(loadingState.Progress);
    }

    public void RefreshByContext(SceneLoadingContext loadingContext) {
        if (loadingContext == null) {
            return;
        }

        LoadingPanelUIState state = new LoadingPanelUIState();
        state.Title = "Loading";
        state.StepText = loadingContext.StepText;
        state.Progress = loadingContext.Progress;
        Refresh(state);
    }

    private static Text CreateLabel(Transform parent, string name, Vector2 anchoredPosition, Vector2 sizeDelta, int fontSize, TextAnchor anchor) {
        Text text = UIRuntimeWidgetFactory.CreateText(name, parent, string.Empty, fontSize, anchor, Color.white);
        SetAnchor(text.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), sizeDelta, anchoredPosition);
        return text;
    }

    private static void SetAnchor(RectTransform rectTransform, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 sizeDelta, Vector2 anchoredPosition) {
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.pivot = pivot;
        rectTransform.sizeDelta = sizeDelta;
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.localScale = Vector3.one;
        rectTransform.localRotation = Quaternion.identity;
    }
}
