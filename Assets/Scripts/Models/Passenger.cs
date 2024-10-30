using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

public class Passenger
{
    private readonly Label _statusLabel = new();
    private bool _selected = false;
    private PassengerStatus _status = PassengerStatus.Comfortable;

    public readonly VisualElement passengerUi = new();

    public Passenger()
    {
        passengerUi.Add(_statusLabel);
        UiUtils.SetBorderWidth(passengerUi, 1);
        _statusLabel.text = _status.ToString();

        passengerUi.RegisterCallback<ClickEvent>(OnClick);
    }

    ~Passenger()
    {
        passengerUi.UnregisterCallback<ClickEvent>(OnClick);
    }

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
        private set
        {
            UiUtils.ToggleBorder(passengerUi, value);
            _selected = value;
        }
    }

    public void IncrementStatus()
    {
        ChangeStatus((int)Status + 1);
    }

    public void DecrementStatus()
    {
        ChangeStatus((int)Status - 1);
    }

    private void ChangeStatus(int newStatus)
    {
        IEnumerable<int> values = Enum.GetValues(typeof(PassengerStatus)).Cast<int>();
        int min = values.Min();
        int max = values.Max();

        Status = (PassengerStatus)Math.Clamp(newStatus, min, max);
    }

    private void OnClick(ClickEvent _)
    {
        // do not select this if passenger is comfortable as there is no reason for player to use supply/crew on them anyways
        if (Status == PassengerStatus.Comfortable) return;

        Selected = !Selected;
    }
}
