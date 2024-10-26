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
        _statusLabel.text = _status.ToString();
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
}
