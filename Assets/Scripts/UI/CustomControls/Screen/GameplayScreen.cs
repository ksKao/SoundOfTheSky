using System.Linq;
using DG.Tweening;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;

[UxmlElement]
public partial class GameplayScreen : VisualElement
{
    public readonly MaterialsBar materialBar = new();
    public readonly MissionTypeTab missionTypeTab = new();
    public readonly BottomNavigationBar bottomNavigationBar = new();
    public readonly VisualElement pendingMissionList = new();
    public readonly DeployedMissionList deployedMissionList = new();
    public readonly CrewSelectionPanel crewSelectionPanel = new();
    public readonly TrainList trainList = new();

    private readonly VisualElement _right = new();

    public VisualElement RightPanel => _right.Children().FirstOrDefault();

    public GameplayScreen()
    {
        style.display = DisplayStyle.Flex;
        style.flexDirection = FlexDirection.Row;
        style.minHeight = UiUtils.GetLengthPercentage(100);
        style.backgroundImage = UiUtils.LoadTexture("wallpaper");
        style.display = DisplayStyle.Flex;
        style.alignItems = Align.Center;
        style.justifyContent = Justify.Center;
        style.unityFont = AssetDatabase.LoadAssetAtPath<Font>("Assets/Fonts/myriad_pro.otf");
        style.unityFontDefinition = new StyleFontDefinition(AssetDatabase.LoadAssetAtPath<FontAsset>("Assets/Fonts/myriad_pro.asset"));
        style.position = Position.Relative;

        VisualElement container = new()
        {
            style =
            {
                backgroundImage = UiUtils.LoadTexture("wallpaper_border"),
                width = UiUtils.GetLengthPercentage(95),
                height = UiUtils.GetLengthPercentage(95),
                paddingTop = 30,
                paddingBottom = 30,
                paddingLeft = 35,
                paddingRight = 35,
                display = DisplayStyle.Flex,
                flexDirection = FlexDirection.Row
            }
        };

        VisualElement left =
            new()
            {
                style =
                {
                    width = UiUtils.GetLengthPercentage(50),
                    height = UiUtils.GetLengthPercentage(100),
                    display = DisplayStyle.Flex,
                    flexDirection = FlexDirection.Column,
                },
            };
        pendingMissionList.style.height = UiUtils.GetLengthPercentage(100);

        left.Add(materialBar);
        left.Add(missionTypeTab);
        left.Add(pendingMissionList);
        left.Add(bottomNavigationBar);

        _right.style.width = UiUtils.GetLengthPercentage(50);

        Add(container);

        container.Add(left);
        container.Add(_right);
    }
    public void ChangeRightPanel(VisualElement element)
    {
        if (RightPanel == element)
            return;

        _right.Clear();

        if (element is not null)
        {
            _right.Add(element);
            element.style.height = UiUtils.GetLengthPercentage(100);
        }
    }

    public void AddError(string message)
    {
        float transitionDuration = 1f;

        Label error = new()
        {
            text = message,
            style =
                {
                    color = Color.white,
                    position = Position.Absolute,
                    left = UiUtils.GetLengthPercentage(50),
                    fontSize = 24,
                    unityTextOutlineWidth = 2,
                    unityTextOutlineColor = Color.black,
                    translate = new Translate(UiUtils.GetLengthPercentage(-50), 0),
                }
        };
        Add(error);

        Tweener opacityTween = DOTween.To(() => 1f, x => error.style.opacity = x, 0f, transitionDuration).SetEase(Ease.Linear).OnComplete(() => Remove(error));
        Tweener translateTween = DOTween.To(() => 15f, x => error.style.top = UiUtils.GetLengthPercentage(x), 14f, transitionDuration).SetEase(Ease.Linear);
    }
}
