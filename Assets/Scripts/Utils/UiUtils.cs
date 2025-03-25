using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public static class UiUtils
{
    public static Length GetLengthPercentage(int percentage)
    {
        return new Length()
        {
            unit = LengthUnit.Percent,
            value = percentage
        };
    }

    public static void SetBorderWidth(VisualElement element, int width)
    {
        element.style.borderTopWidth = width;
        element.style.borderBottomWidth = width;
        element.style.borderLeftWidth = width;
        element.style.borderRightWidth = width;
    }

    public static void ToggleBorder(VisualElement element, bool on)
    {
        if (on)
        {
            element.style.borderTopColor = Color.black;
            element.style.borderBottomColor = Color.black;
            element.style.borderLeftColor = Color.black;
            element.style.borderRightColor = Color.black;
        }
        else
        {
            element.style.borderTopColor = Color.clear;
            element.style.borderBottomColor = Color.clear;
            element.style.borderLeftColor = Color.clear;
            element.style.borderRightColor = Color.clear;
        }
    }

    public static Texture2D LoadTexture(string fileName, string extension = "png")
    {
        return AssetDatabase.LoadAssetAtPath<Texture2D>($"Assets/Images/{fileName}.{extension}");
    }

    public static Sprite LoadSprite(string fileName, string extension = "png")
    {
        return AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/Images/{fileName}.{extension}");
    }

    public static Color HexToRgb(string hex)
    {
        if (hex.StartsWith("#"))
            hex = hex[1..];

        float r = int.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber) / 255f;
        float g = int.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber) / 255f;
        float b = int.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber) / 255f;

        return new(r, g, b);
    }
}
