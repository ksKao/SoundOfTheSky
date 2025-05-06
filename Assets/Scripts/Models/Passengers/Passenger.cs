using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class Passenger
{
    private static readonly Texture2D _backgroundImage = UiUtils.LoadTexture("passenger_selection_background");
    private static readonly Texture2D _backgroundImageSelected = UiUtils.LoadTexture("passenger_selection_background_glow");

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

    protected Label StatusLabel => _statusLabel;
    protected virtual Texture2D BackgroundImage => _backgroundImage;
    protected virtual Texture2D BackgroundImageSelected => _backgroundImageSelected;

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
            _imageContainer.style.backgroundImage = value ? BackgroundImageSelected : BackgroundImage;
            _selected = value;
        }
    }

    public Passenger()
    {
        _imageContainer.style.backgroundImage = BackgroundImage;
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
