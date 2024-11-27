using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class CrewSelectionPanelButton : VisualElement
{
    public CrewSelectionPanelButton()
    {
        Debug.LogWarning(
            $"Detected calling default constructor for {nameof(CrewSelectionPanelButton)}."
        );
    }

    public CrewSelectionPanelButton(ref List<Crew> preselectedCrews)
    {
        Label crewNumberLabel = new("0");
        List<Crew> preselectedCrewTemp = preselectedCrews;
        RegisterCallback<ClickEvent>(
            (_) =>
            {
                UiManager.Instance.GameplayScreen.crewSelectionPanel.Show(
                    GameManager.Instance.crews.ToArray(),
                    (crews) =>
                    {
                        List<Crew> selectedCrews = crews.Where(c => c.Selected).ToList();

                        // need to do this to maintain the same reference (cannot directly assign)
                        preselectedCrewTemp.Clear();
                        preselectedCrewTemp.AddRange(selectedCrews);

                        crewNumberLabel.text = selectedCrews.Count.ToString();
                    },
                    () =>
                    {
                        preselectedCrewTemp.Clear();
                        crewNumberLabel.text = "0";
                        UiManager.Instance.GameplayScreen.ChangeRightPanel(null);
                    }
                );

                foreach (Crew crew in preselectedCrewTemp)
                    crew.Selected = true;
            }
        );
        Add(new Label("Crews"));
        Add(crewNumberLabel);
    }
}
