using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

[UxmlElement]
public partial class CrewSelectionPanel : VisualElement
{
    private List<Crew> _crews = new();
    private Action<List<Crew>> _onSelect;

    public CrewSelectionPanel()
    {
    }

    public void OnCrewSelectChange()
    {
        _onSelect?.Invoke(_crews);
    }

    public void Show(List<Crew> crews, Action<List<Crew>> onSelect)
    {
        _crews = crews;
        _onSelect = onSelect;

        Clear();

        foreach (Crew crew in _crews)
        {
            crew.Selected = false;
            Add(crew.Ui);
        }

        UiManager.Instance.GameplayScreen.ChangeRightPanel(this);
    }
}
