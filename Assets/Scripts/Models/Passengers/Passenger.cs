using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class Passenger
{
    private static readonly Texture2D _backgroundGray = UiUtils.LoadTexture("passenger_selection_background");
    private static readonly Texture2D _backgroundGraySelected = UiUtils.LoadTexture("passenger_selection_background_glow");
    private static readonly Texture2D _backgroundBlue = UiUtils.LoadTexture("crew_selection_background");
    private static readonly Texture2D _backgroundBlueSelected = UiUtils.LoadTexture("crew_selection_background_glow");

    public bool selectable = true;
    public readonly VisualElement ui = new()
    {
        style =
        {
            display = DisplayStyle.Flex,
            flexDirection = FlexDirection.Column,
            alignItems = Align.Center,
        }
    };

    private readonly Label _statusLabel = new();
    private readonly VisualElement _imageContainer = new()
    {
        style =
        {
            width = 100,
            height = 90
        }
    };

    private bool _selected = false;
    private PassengerStatus _status = PassengerStatus.Comfortable;
    private PassengerBackgroundStyle _backgroundStyle = PassengerBackgroundStyle.Blue;

    protected Label StatusLabel => _statusLabel;

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
            if (_backgroundStyle == PassengerBackgroundStyle.Blue)
                _imageContainer.style.backgroundImage = value ? _backgroundBlueSelected : _backgroundBlue;
            else
                _imageContainer.style.backgroundImage = value ? _backgroundGraySelected : _backgroundGray;
            _selected = value;
        }
    }
    public PassengerBackgroundStyle BackgroundStyle
    {
        get => _backgroundStyle;
        set
        {
            _imageContainer.style.backgroundImage = value == PassengerBackgroundStyle.Blue ? _backgroundBlue : _backgroundGray;
            _backgroundStyle = value;
        }
    }

    public Passenger()
    {
        _imageContainer.style.backgroundImage = _backgroundStyle == PassengerBackgroundStyle.Blue ? _backgroundBlue : _backgroundGray;
        ui.Add(_imageContainer);
        ui.Add(_statusLabel);

        _statusLabel.text = _status.ToString();

        ui.RegisterCallback<ClickEvent>(OnClick);
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
        if (!selectable) return;

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
