using System;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class MissionTypeTab : VisualElement
{
    public static Action<Tab, Tab> OnMissionTabChanged;

    public TabView TabView { get; private set; } = new();
    public Button RefreshButton { get; private set; } = new();

    public MissionTypeTab()
    {
        style.display = DisplayStyle.Flex;
        style.flexDirection = FlexDirection.Row;

        VisualElement tabViewHeaderContainer = TabView.Q("unity-tab-view__header-container");

        tabViewHeaderContainer.style.minHeight = UiUtils.GetLengthPercentage(100);

        string[] missionTypes = Enum.GetNames(typeof(MissionType));

        foreach (string missionType in missionTypes)
        {
            Tab tab = new() { label = missionType };
            VisualElement tabHeader = tab.Q(Tab.tabHeaderUssClassName);
            tabHeader.style.minHeight = UiUtils.GetLengthPercentage(100);
            TabView.Add(tab);
        }

        Add(TabView);

        GenerateRefreshButton();
    }

    private void GenerateRefreshButton()
    {
        RefreshButton.text = "Refresh";
        RefreshButton.style.flexGrow = 1;
        RefreshButton.style.unityTextAlign = TextAnchor.MiddleLeft;
        RefreshButton.style.marginLeft = 0;
        RefreshButton.style.marginRight = 0;
        RefreshButton.style.marginTop = 0;
        RefreshButton.style.marginBottom = 0;

        Add(RefreshButton);
    }
}
