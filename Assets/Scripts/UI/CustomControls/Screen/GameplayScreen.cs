using UnityEngine.UIElements;

[UxmlElement]
public partial class GameplayScreen : VisualElement
{
    public GameplayScreen()
    {
        VisualElement left = new()
        {
            style =
            {
                width = new StyleLength()
                {
                    value = new Length()
                    {
                        unit = LengthUnit.Percent,
                        value = 50
                    }
                }
            }
        };
        VisualElement right = new();

        left.Add(new AssetsBar());
        left.Add(new MissionTypeTab());

        Add(left);
        Add(right);
    }
}
