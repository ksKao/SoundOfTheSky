using UnityEngine.UIElements;

[UxmlElement]
public partial class GameplayScreen : VisualElement
{
    public GameplayScreen()
    {
        Add(new AssetsBar());
    }
}
