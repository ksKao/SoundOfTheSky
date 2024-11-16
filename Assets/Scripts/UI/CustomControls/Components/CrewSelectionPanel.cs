using System;
using UnityEngine.UIElements;

[UxmlElement]
public partial class CrewSelectionPanel : VisualElement
{
    private Crew[] _crews = { };
    private Action<Crew[]> _onSelect;

    public CrewSelectionPanel()
    {
    }

    public void OnCrewSelectChange()
    {
        _onSelect?.Invoke(_crews);
    }

    public void Show(Crew[] crews, Action<Crew[]> onSelect)
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
