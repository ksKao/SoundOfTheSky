using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class CrewList : VisualElement
{
    private readonly ScrollView _crewListContainer = new() { style = { flexGrow = 1 } };

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

        VisualElement buttonsContainer =
            new() { style = { display = DisplayStyle.Flex, flexDirection = FlexDirection.Row } };

        Button restButton = new() { text = "Rest" };
        Button cureButton = new() { text = "Cure" };

        Button newCrewButton =
            new() { text = "Crew ($300)", style = { marginLeft = StyleKeyword.Auto } };

        buttonsContainer.Add(restButton);
        buttonsContainer.Add(cureButton);
        buttonsContainer.Add(newCrewButton);

        Add(buttonsContainer);

        RegisterCallback<AttachToPanelEvent>(
            (e) =>
            {
                RefreshCrewList();
            }
        );
    }

    private void RefreshCrewList()
    {
        if (GameManager.Instance == null)
            return;

        _crewListContainer.Clear();

        foreach (Crew crew in GameManager.Instance.crews)
        {
            _crewListContainer.Add(new Label(crew.Status.ToString()));
        }
    }
}
