using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public static class UiUtils
{
    public static readonly Color darkBlueTextColor = HexToRgb("#2c3064");

    public static Length GetLengthPercentage(float percentage)
    {
        return new Length() { unit = LengthUnit.Percent, value = percentage };
    }

    public static void SetBorderWidth(VisualElement element, int width)
    {
        element.style.borderTopWidth = width;
        element.style.borderBottomWidth = width;
        element.style.borderLeftWidth = width;
        element.style.borderRightWidth = width;
    }

    public static void ToggleBorder(VisualElement element, bool on, Color? color = null)
    {
        Color finalColor = !on ? Color.clear : color == null ? Color.black : (Color)color;

        element.style.borderTopColor = finalColor;
        element.style.borderBottomColor = finalColor;
        element.style.borderLeftColor = finalColor;
        element.style.borderRightColor = finalColor;
    }

    public static Texture2D LoadTexture(string fileName)
    {
        return Resources.Load<Texture2D>($"Images/{fileName}");
    }

    public static Sprite LoadSprite(string fileName)
    {
        return Resources.Load<Sprite>($"Images/{fileName}");
    }

    public static Color HexToRgb(string hex)
    {
        if (hex.StartsWith("#"))
            hex = hex[1..];

        float r =
            int.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber) / 255f;
        float g =
            int.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber) / 255f;
        float b =
            int.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber) / 255f;

        return new(r, g, b);
    }

    public static VisualElement WrapLabel(Label label, bool applyAdditionalStyle = true)
    {
        VisualElement wrapper = new();
        wrapper.style.width = GetLengthPercentage(100);
        wrapper.style.display = DisplayStyle.Flex;
        wrapper.style.alignItems = Align.Center;
        wrapper.style.flexDirection = FlexDirection.ColumnReverse;
        wrapper.style.justifyContent = Justify.Center;
        wrapper.Add(label);

        if (applyAdditionalStyle)
        {
            label.style.marginLeft = 0;
            label.style.marginRight = 0;
            label.style.whiteSpace = WhiteSpace.Normal;
            label.style.unityTextAlign = TextAnchor.UpperCenter;
        }

        return wrapper;
    }

    public static void ShowError(string message)
    {
        if (UiManager.Instance == null || UiManager.Instance.GameplayScreen == null)
            return;

        UiManager.Instance.GameplayScreen.AddError(message);
    }
}
