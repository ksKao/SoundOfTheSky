using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class CrewList : VisualElement
{
    private readonly ScrollView _crewListContainer = new() { style = { flexGrow = 1 } };
    private readonly VisualElement _buttonsContainer =
        new() { style = { display = DisplayStyle.Flex, flexDirection = FlexDirection.Row } };
    private SelectionMode _selectionMode = SelectionMode.None;

    public CrewList()
    {
        style.display = DisplayStyle.Flex;
        style.flexDirection = FlexDirection.Column;
        style.height = UiUtils.GetLengthPercentage(100);

        Add(
            new Label("Crew")
            {
                style = { fontSize = 24, unityFontStyleAndWeight = FontStyle.Bold },
            }
        );

        Add(_crewListContainer);

        Button restButton = new() { text = "Rest" };
        restButton.clicked += () =>
        {
            if (_selectionMode == SelectionMode.Rest)
            {
                _selectionMode = SelectionMode.None;
                IEnumerable<Crew> selectedCrews = GameManager.Instance.crews.Where(c => c.Selected);

                foreach (Crew crew in selectedCrews)
                    crew.isResting = true;

                RefreshCrewList();
                EnableAllButtons();
            }
            else
            {
                _selectionMode = SelectionMode.Rest;
                RefreshCrewList();
                DisableAllButtonsOtherThan(restButton);
            }

            foreach (Crew crew in GameManager.Instance.crews)
                crew.Selected = false;
        };

        Button cureButton = new() { text = "Cure" };

        Button newCrewButton =
            new() { text = "Crew ($300)", style = { marginLeft = StyleKeyword.Auto } };

        _buttonsContainer.Add(restButton);
        _buttonsContainer.Add(cureButton);
        _buttonsContainer.Add(newCrewButton);

        Add(_buttonsContainer);

        RegisterCallback<AttachToPanelEvent>(
            (e) =>
            {
                RefreshCrewList();
            }
        );
    }

    public void RefreshCrewList()
    {
        if (GameManager.Instance == null)
            return;

        _crewListContainer.Clear();

        foreach (Crew crew in GameManager.Instance.crews)
        {
            string statusStr = crew.Status.ToString();

            if (crew.isResting)
                statusStr += " (Resting)";
            else if (crew.DeployedMission != null)
                statusStr += " (Occupied)";

            Label crewLabel = new(statusStr);
            UiUtils.SetBorderWidth(crewLabel, 2);
            _crewListContainer.Add(crewLabel);

            if (_selectionMode == SelectionMode.None)
                continue;

            if (
                crew.Status == PassengerStatus.Comfortable
                || crew.isResting
                || crew.DeployedMission != null
            )
            {
                crewLabel.style.opacity = 0.5f;
            }
            else
            {
                crewLabel.style.opacity = 1;
                crewLabel.RegisterCallback<ClickEvent>(
                    (e) =>
                    {
                        crew.Selected = !crew.Selected;

                        UiUtils.ToggleBorder(crewLabel, crew.Selected);
                    }
                );
            }
        }
    }

    private void DisableAllButtonsOtherThan(Button button)
    {
        List<Button> buttons = _buttonsContainer.Query<Button>().ToList();

        foreach (Button b in buttons)
            b.SetEnabled(b == button);
    }

    private void EnableAllButtons()
    {
        List<Button> buttons = _buttonsContainer.Query<Button>().ToList();

        foreach (Button button in buttons)
            button.SetEnabled(true);
    }

    private enum SelectionMode
    {
        None,
        Rest,
        Cure,
    }
}
