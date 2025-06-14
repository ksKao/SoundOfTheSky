using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class CrewSelectionPanel : VisualElement
{
    private static readonly Texture2D _crewSelectionPanelBackground = UiUtils.LoadTexture(
        "crew_selection_panel_background"
    );
    private static readonly Texture2D _buttonBackground = UiUtils.LoadTexture(
        "crew_selection_button"
    );

    private Crew[] _crews = { };
    private readonly Button _cancelButton = new();
    private readonly ScrollView _scrollView = new()
    {
        style =
        {
            backgroundImage = _crewSelectionPanelBackground,
            flexGrow = 1,
            paddingTop = UiUtils.GetLengthPercentage(4f),
            paddingBottom = UiUtils.GetLengthPercentage(4f),
            paddingLeft = UiUtils.GetLengthPercentage(3f),
        },
    };
    private readonly VisualElement _crewListContainer = new()
    {
        style =
        {
            display = DisplayStyle.Flex,
            flexDirection = FlexDirection.Row,
            flexWrap = Wrap.Wrap,
        },
    };
    private readonly VisualElement _additionalUi = new() { style = { flexGrow = 1 } };
    private readonly VisualElement _buttonContainer = new()
    {
        style =
        {
            display = DisplayStyle.Flex,
            flexDirection = FlexDirection.Row,
            alignItems = Align.Center,
            minHeight = UiUtils.GetLengthPercentage(10),
            width = UiUtils.GetLengthPercentage(100),
        },
    };

    private Action<Crew[]> _onSelect;
    private Action _onCancel;

    public CrewSelectionPanel()
    {
        _cancelButton.text = "CANCEL";

        style.display = DisplayStyle.Flex;
        style.flexDirection = FlexDirection.Column;

        Add(_scrollView);
        _scrollView.Add(_crewListContainer);

        _buttonContainer.Add(_cancelButton);
        _buttonContainer.Add(_additionalUi);

        Add(_buttonContainer);

        Crew.OnSelect += OnSelect;
    }

    public void Show(
        Crew[] crews,
        Action<Crew[]> onSelect,
        Func<Crew, string> getBracketText = null,
        Action onCancel = null,
        VisualElement additionalUi = null
    )
    {
        _crews = crews;
        _onSelect = onSelect;

        _crewListContainer.Clear();

        // deselect all crew first
        foreach (Crew crew in _crews)
        {
            crew.Selected = false;
            _crewListContainer.Add(crew.ui);

            string bracketText = "";

            if (getBracketText is not null)
                bracketText = getBracketText(crew);

            if (bracketText.Length <= 0)
            {
                crew.bracketLabel.style.display = DisplayStyle.None;
            }
            else
            {
                crew.bracketLabel.style.display = DisplayStyle.Flex;
                crew.bracketLabel.text = $"({bracketText})";
            }
        }

        // unsubscribe old event and subscribe to the new event passed in
        if (_onCancel is not null)
            _cancelButton.clicked -= _onCancel;

        _onCancel = onCancel;

        // check if new cancel action is not null, if yes, need to hide the button
        if (_onCancel is not null)
        {
            _cancelButton.style.display = DisplayStyle.Flex;
            _cancelButton.clicked += _onCancel;
        }
        else
        {
            _cancelButton.style.display = DisplayStyle.None;
        }

        UiManager.Instance.CityModeScreen.ChangeRightPanel(this);

        _additionalUi.Clear();
        if (additionalUi is not null)
            _additionalUi.Add(additionalUi);

        List<Button> buttons = _buttonContainer.Query<Button>().ToList();
        foreach (Button button in buttons)
        {
            button.style.backgroundImage = _buttonBackground;
            button.style.color = Color.white;
            button.style.width = 100;
            button.style.height = 90;
            button.style.fontSize = 24;
            button.style.backgroundColor = Color.clear;
            UiUtils.ToggleBorder(button, false);
        }
    }

    private void OnSelect(Crew crew)
    {
        if (UiManager.Instance.CityModeScreen.RightPanel == this)
            _onSelect?.Invoke(_crews);
    }
}
