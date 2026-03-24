using UnityEngine;
using UnityEngine.UI;

public static class UIRuntimeWidgetFactory {
    private static Font defaultFont;

    public static void StretchRect(RectTransform rectTransform) {
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        rectTransform.localScale = Vector3.one;
        rectTransform.localRotation = Quaternion.identity;
    }

    public static Font GetDefaultFont() {
        if (defaultFont == null) {
            defaultFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (defaultFont == null) {
                defaultFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
            }
        }

        return defaultFont;
    }

    public static RectTransform CreateStretchChild(string name, Transform parent) {
        GameObject gameObject = new GameObject(name);
        RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
        rectTransform.SetParent(parent, false);
        StretchRect(rectTransform);
        return rectTransform;
    }

    public static Image CreateImage(string name, Transform parent, Color color) {
        GameObject gameObject = new GameObject(name);
        RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
        rectTransform.SetParent(parent, false);
        Image image = gameObject.AddComponent<Image>();
        image.color = color;
        return image;
    }

    public static Text CreateText(string name, Transform parent, string content, int fontSize, TextAnchor anchor, Color color) {
        GameObject gameObject = new GameObject(name);
        RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
        rectTransform.SetParent(parent, false);
        Text text = gameObject.AddComponent<Text>();
        text.font = GetDefaultFont();
        text.fontSize = fontSize;
        text.alignment = anchor;
        text.color = color;
        text.text = content;
        text.horizontalOverflow = HorizontalWrapMode.Overflow;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        return text;
    }

    public static Button CreateButton(string name, Transform parent, string textValue, Vector2 sizeDelta) {
        Image image = CreateImage(name, parent, new Color(0.18f, 0.2f, 0.26f, 0.92f));
        RectTransform rectTransform = image.rectTransform;
        rectTransform.sizeDelta = sizeDelta;

        Button button = image.gameObject.AddComponent<Button>();
        ColorBlock colors = button.colors;
        colors.normalColor = new Color(0.18f, 0.2f, 0.26f, 0.92f);
        colors.highlightedColor = new Color(0.25f, 0.28f, 0.35f, 0.96f);
        colors.pressedColor = new Color(0.1f, 0.12f, 0.16f, 0.96f);
        colors.selectedColor = colors.highlightedColor;
        button.colors = colors;

        Text label = CreateText("Label", rectTransform, textValue, 22, TextAnchor.MiddleCenter, Color.white);
        StretchRect(label.rectTransform);
        return button;
    }

    public static InputField CreateInputField(string name, Transform parent, string defaultValue, string placeholderText, Vector2 sizeDelta) {
        Image background = CreateImage(name, parent, new Color(0.1f, 0.12f, 0.16f, 0.9f));
        RectTransform rectTransform = background.rectTransform;
        rectTransform.sizeDelta = sizeDelta;

        InputField inputField = background.gameObject.AddComponent<InputField>();
        Text text = CreateText("Text", rectTransform, defaultValue, 24, TextAnchor.MiddleLeft, Color.white);
        Text placeholder = CreateText("Placeholder", rectTransform, placeholderText, 24, TextAnchor.MiddleLeft, new Color(1f, 1f, 1f, 0.35f));
        text.rectTransform.offsetMin = new Vector2(16f, 10f);
        text.rectTransform.offsetMax = new Vector2(-16f, -10f);
        placeholder.rectTransform.offsetMin = new Vector2(16f, 10f);
        placeholder.rectTransform.offsetMax = new Vector2(-16f, -10f);
        inputField.textComponent = text;
        inputField.placeholder = placeholder;
        inputField.text = defaultValue;
        inputField.lineType = InputField.LineType.SingleLine;
        inputField.contentType = InputField.ContentType.IntegerNumber;
        return inputField;
    }
}
