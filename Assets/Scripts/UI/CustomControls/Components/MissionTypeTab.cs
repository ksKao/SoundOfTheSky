using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class MissionTypeTab : VisualElement
{
    public MissionTypeTab()
    {
        style.display = DisplayStyle.Flex;
        style.flexDirection = FlexDirection.Row;

        TabView tabView = new();
        VisualElement tabViewHeaderContainer = tabView.Q("unity-tab-view__header-container");

        tabViewHeaderContainer.style.minHeight = new StyleLength()
        {
            value = new Length()
            {
                unit = LengthUnit.Percent,
                value = 100
            }
        };

        tabView.activeTabChanged += (Tab _, Tab selected) =>
        {
            if (selected is not null)
            {
                Debug.Log(selected.label);
            }
        };

        Button refreshButton = new()
        {
            text = "Refresh",
            style =
            {
                flexGrow = 1,
                unityTextAlign = new StyleEnum<TextAnchor>
                {
                    value = TextAnchor.MiddleLeft
                }
            }
        };

        string[] missionTypes = { "Rescue", "Resupply", "Documentation" };

        foreach (string missionType in missionTypes)
        {
            Tab tab = new() { label = missionType };
            tab.Q(Tab.tabHeaderUssClassName).style.minHeight = new StyleLength()
            {
                value = new Length
                {
                    unit = LengthUnit.Percent,
                    value = 100
                }
            };
            tabView.Add(tab);
        }

        Add(tabView);
        Add(refreshButton);
    }
}
