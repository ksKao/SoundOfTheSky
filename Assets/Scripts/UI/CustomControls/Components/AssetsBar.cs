using UnityEngine.UIElements;

[UxmlElement]
public partial class AssetsBar : VisualElement
{
    public AssetsBar()
    {
        style.display = DisplayStyle.Flex;
        style.flexDirection = FlexDirection.Row;

        string[] assets = { "Payments", "Supply", "Resources", "Citizens" };

        foreach (string asset in assets)
        {
            VisualElement container = new();
            container.style.display = DisplayStyle.Flex;
            container.style.flexDirection = FlexDirection.Row;

            VisualElement icon = new();
            icon.style.marginRight = 10;

            container.Add(icon);
            container.Add(new Label(asset));

            Add(container);
        }
    }
}
