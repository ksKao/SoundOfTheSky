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
}
