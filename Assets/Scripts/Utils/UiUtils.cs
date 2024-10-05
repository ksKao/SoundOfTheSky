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
}
