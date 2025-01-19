using System;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class MissionTypeTab : VisualElement
{
    public static Action<Tab, Tab> OnMissionTabChanged;

    public readonly TabView tabView = new();
    public readonly Button refreshButton = new();

    public MissionTypeTab()
    {
        style.display = DisplayStyle.Flex;
        style.flexDirection = FlexDirection.Row;

        VisualElement tabViewHeaderContainer = tabView.Q("unity-tab-view__header-container");

        tabViewHeaderContainer.style.minHeight = UiUtils.GetLengthPercentage(100);

        string[] missionTypes = Enum.GetNames(typeof(MissionType));

        foreach (string missionType in missionTypes)
        {
            Tab tab = new() { label = missionType };
            VisualElement tabHeader = tab.Q(Tab.tabHeaderUssClassName);
            tabHeader.style.minHeight = UiUtils.GetLengthPercentage(100);
            tabView.Add(tab);
        }

        Add(tabView);

        GenerateRefreshButton();
    }

    private void GenerateRefreshButton()
    {
        refreshButton.text = "Refresh (1200 payments)";
        refreshButton.style.flexGrow = 1;
        refreshButton.style.unityTextAlign = TextAnchor.MiddleLeft;
        refreshButton.style.marginLeft = 0;
        refreshButton.style.marginRight = 0;
        refreshButton.style.marginTop = 0;
        refreshButton.style.marginBottom = 0;

        refreshButton.clicked += () =>
        {
            if (GameManager.Instance.GetMaterialValue(MaterialType.Payments) < 1200)
            {
                Debug.Log("Not enough payments");
                return;
            }

            GameManager.Instance.IncrementMaterialValue(MaterialType.Payments, -1200);
            GameManager.Instance.RefreshAllMissions();
        };

        Add(refreshButton);
    }
}
