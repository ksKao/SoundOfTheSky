using UnityEngine; // This is needed here for Application.Quit, otherwise the build will fail
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

[UxmlElement]
public partial class MainMenuScreen : VisualElement
{
    public MainMenuScreen()
    {
        style.position = Position.Relative;
        style.width = UiUtils.GetLengthPercentage(100);
        style.height = UiUtils.GetLengthPercentage(100);
        style.display = DisplayStyle.Flex;
        style.flexDirection = FlexDirection.Column;
        style.alignItems = Align.Center;
        style.justifyContent = Justify.Center;

        Button quitButton = new() { text = "Quit" };

        Button cityModeButton = new() { text = "City Mode" };

        quitButton.clicked += () =>
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        };

        cityModeButton.clicked += () =>
        {
            SceneManager.LoadScene(1);
        };

        Add(cityModeButton);
        Add(quitButton);
    }
}
