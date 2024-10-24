using System;
using System.Collections.Generic;
using System.Linq;

public class Passenger
{
    public PassengerStatus Status { get; private set; } = PassengerStatus.Comfortable;

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
