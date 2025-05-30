using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class MissionTypeTab : VisualElement
{
    private const string NON_ACTIVE_BUTTON_TEXT_COLOR_HEX_CODE = "#74def4";

    public static Action<Tab, Tab> OnMissionTabChanged;

    private readonly Color _nonActiveButtonTextColor = UiUtils.HexToRgb(
        NON_ACTIVE_BUTTON_TEXT_COLOR_HEX_CODE
    );
    private readonly List<(VisualElement backdrop, Button tabButton)> _tabElements = new();
    private readonly Button _refreshButton = new()
    {
        text = "REFRESH (1200 payments)",
        style =
        {
            unityTextAlign = TextAnchor.MiddleLeft,
            marginLeft = 0,
            marginRight = 0,
            marginTop = 0,
            marginBottom = 0,
            backgroundColor = Color.clear,
            color = UiUtils.HexToRgb(NON_ACTIVE_BUTTON_TEXT_COLOR_HEX_CODE),
        },
    };
    private readonly Button _citiesButton = new()
    {
        text = "Cities",
        style =
        {
            unityTextAlign = TextAnchor.MiddleLeft,
            marginLeft = 0,
            marginRight = 0,
            marginTop = 0,
            marginBottom = 0,
            backgroundColor = Color.clear,
            color = UiUtils.HexToRgb(NON_ACTIVE_BUTTON_TEXT_COLOR_HEX_CODE),
        },
    };
    private MissionType _activeTab = MissionType.Rescue;

    public MissionType ActiveTab => _activeTab;

    public MissionTypeTab()
    {
        style.display = DisplayStyle.Flex;
        style.flexDirection = FlexDirection.Row;
        style.backgroundImage = UiUtils.LoadTexture("mission_type_tab_background");
        style.alignItems = Align.Center;
        style.paddingLeft = 10;
        style.paddingRight = 10;

        List<MissionType> missionTypes = Enum.GetValues(typeof(MissionType))
            .Cast<MissionType>()
            .ToList();
        _tabElements.Capacity = missionTypes.Count;

        Add(CreateSeparator());

        foreach (MissionType missionType in missionTypes)
        {
            VisualElement singleTabContainer = new();
            singleTabContainer.style.position = Position.Relative;

            Button tab = new()
            {
                text = missionType.ToString().ToUpper(),
                style = { backgroundColor = Color.clear },
            };
            tab.clicked += () =>
            {
                _activeTab = missionType;
                UiManager.Instance.CityModeScreen.RefreshMissionList(missionType);
                RefreshTabHighlight();
            };

            UiUtils.ToggleBorder(tab, false);

            VisualElement tabBackdrop = new()
            {
                style =
                {
                    position = Position.Absolute,
                    top = UiUtils.GetLengthPercentage(50),
                    left = 0,
                    height = UiUtils.GetLengthPercentage(50),
                    width = UiUtils.GetLengthPercentage(100),
                    translate = new Translate(0, UiUtils.GetLengthPercentage(-50)),
                },
            };

            _tabElements.Add((tabBackdrop, tab));
            singleTabContainer.Add(tabBackdrop);
            singleTabContainer.Add(tab);
            Add(singleTabContainer);
            Add(CreateSeparator());
        }

        GenerateRefreshButton();
        GenerateCitiesButton();
    }

    public void RefreshTabHighlight()
    {
        for (int i = 0; i < _tabElements.Count; i++)
        {
            if (i == (int)ActiveTab)
            {
                _tabElements[i].backdrop.style.backgroundColor = UiUtils.HexToRgb("#183361");
                _tabElements[i].tabButton.style.color = Color.white;
            }
            else
            {
                _tabElements[i].backdrop.style.backgroundColor = Color.clear;
                _tabElements[i].tabButton.style.color = _nonActiveButtonTextColor;
            }
        }
    }

    private VisualElement CreateSeparator()
    {
        VisualElement separator = new()
        {
            style =
            {
                height = UiUtils.GetLengthPercentage(65),
                width = 3,
                borderLeftColor = Color.white,
                borderRightColor = Color.white,
                borderTopColor = Color.white,
                borderBottomColor = Color.white,
            },
        };

        UiUtils.SetBorderWidth(separator, 1);

        return separator;
    }

    private void GenerateRefreshButton()
    {
        UiUtils.ToggleBorder(_refreshButton, false);

        _refreshButton.clicked += () =>
        {
            if (CityModeManager.Instance.GetMaterialValue(MaterialType.Payments) < 1200)
            {
                UiUtils.ShowError("Not enough payments");
                return;
            }

            CityModeManager.Instance.IncrementMaterialValue(MaterialType.Payments, -1200);
            CityModeManager.Instance.RefreshAllMissions();
        };

        Add(_refreshButton);
        Add(CreateSeparator());
    }

    private void GenerateCitiesButton()
    {
        UiUtils.ToggleBorder(_citiesButton, false);

        _citiesButton.clicked += () =>
        {
            UiManager.Instance.CityModeScreen.ChangeRightPanel(
                UiManager.Instance.CityModeScreen.map
            );
        };

        Add(_citiesButton);
        Add(CreateSeparator());
    }
}
