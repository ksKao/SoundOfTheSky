using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

[UxmlElement]
public partial class CrewSelectionPanelButton : VisualElement
{
    private List<Crew> _selectedCrews = new();

    public List<Crew> SelectedCrews => _selectedCrews;

    public CrewSelectionPanelButton()
    {
        Label crewNumberLabel = new("0");
        RegisterCallback<ClickEvent>(
            (_) =>
            {
                UiManager.Instance.CityModeScreen.crewSelectionPanel.Show(
                    CityModeManager.Instance.crews.Where(c => !c.isResting).ToArray(),
                    (crews) =>
                    {
                        _selectedCrews = crews.Where(c => c.Selected).ToList();

                        crewNumberLabel.text = _selectedCrews.Count.ToString();
                    },
                    (crew) => crew.deployedMission is null ? "" : "Deployed",
                    () =>
                    {
                        _selectedCrews.Clear();
                        crewNumberLabel.text = "0";
                        UiManager.Instance.CityModeScreen.ChangeRightPanel(null);
                    }
                );

                foreach (Crew crew in _selectedCrews)
                    crew.Selected = true;
            }
        );
        Add(new Label("Crews"));
        Add(crewNumberLabel);
    }
}
