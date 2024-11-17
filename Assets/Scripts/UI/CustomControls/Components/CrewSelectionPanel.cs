using System;
using UnityEngine.UIElements;

[UxmlElement]
public partial class CrewSelectionPanel : VisualElement
{
    private Crew[] _crews = { };
    private Button _cancelButton = new();
    private Button _supplyButton = new();
    private VisualElement _crewListContainer = new();
    private Action<Crew[]> _onSelect;
    private Action _onCancel;

    public CrewSelectionPanel()
    {
        _cancelButton.text = "Cancel";
        _supplyButton.text = "Heal with supply";

        Add(_crewListContainer);
        Add(_cancelButton);
        Add(_supplyButton);
    }

    public void OnCrewSelectChange()
    {
        _onSelect?.Invoke(_crews);
    }

    public void Show(Crew[] crews, Action<Crew[]> onSelect, Action onCancel)
    {
        _crews = crews;
        _onSelect = onSelect;

        _crewListContainer.Clear();

        foreach (Crew crew in _crews)
        {
            crew.Selected = false;
            _crewListContainer.Add(crew.Ui);
        }

        // unsubscribe old event and subscribe to the new event passed in
        if (_onCancel is not null)
            _cancelButton.clicked -= _onCancel;

        _onCancel = onCancel;
        _cancelButton.clicked += _onCancel;

        UiManager.Instance.GameplayScreen.ChangeRightPanel(this);
    }
}
