using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

public class Passenger
{
    private readonly Label _statusLabel = new();
    private bool _selected = false;
    private PassengerStatus _status = PassengerStatus.Comfortable;
    
    protected Label StatusLabel => _statusLabel;

    public readonly VisualElement Ui = new();

    public PassengerStatus Status 
    {
        get => _status;
        private set
        {
            _status = value;
            _statusLabel.text = _status.ToString();
        }
    }
    public bool Selected 
    {
        get => _selected;
        set
        {
            UiUtils.ToggleBorder(Ui, value);
            _selected = value;
        }
    }

    public Passenger()
    {
        Ui.Add(_statusLabel);
        UiUtils.SetBorderWidth(Ui, 1);
        _statusLabel.text = _status.ToString();

        Ui.RegisterCallback<ClickEvent>(OnClick);
    }

    public void MakeWorse()
    {
        ChangeStatus((int)Status + 1);
    }

    public void MakeBetter()
    {
        ChangeStatus((int)Status - 1);
    }

    protected virtual void OnClick(ClickEvent _)
    {
        // do not select this if passenger is comfortable as there is no reason for player to use supply/crew on them anyways
        if (Status == PassengerStatus.Comfortable) return;

        Selected = !Selected;
    }

    private void ChangeStatus(int newStatus)
    {
        IEnumerable<int> values = Enum.GetValues(typeof(PassengerStatus)).Cast<int>();
        int min = values.Min();
        int max = values.Max();

        Status = (PassengerStatus)Math.Clamp(newStatus, min, max);
    }
}
