using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class Passenger
{
    private static readonly Texture2D _backgroundImage = UiUtils.LoadTexture(
        "passenger_selection_background"
    );
    private static readonly Texture2D _backgroundImageSelected = UiUtils.LoadTexture(
        "passenger_selection_background_glow"
    );
    private static readonly Color _comfortableColor = UiUtils.HexToRgb("#39b54a");
    private static readonly Color _coldColor = UiUtils.HexToRgb("#0000ff");
    private static readonly Color _sickColor = UiUtils.HexToRgb("#ff00ff");
    private static readonly Color _terminalColor = UiUtils.HexToRgb("#662d91");
    private static readonly Color _criticalColor = UiUtils.HexToRgb("#ff0000");
    private static readonly Color _deathColor = Color.black;

    public bool selectable = true;
    public readonly VisualElement ui = new()
    {
        style =
        {
            display = DisplayStyle.Flex,
            flexDirection = FlexDirection.Column,
            alignItems = Align.Center,
        },
    };

    public string Name { get; protected set; } = DataManager.Instance.GetRandomName();

    private readonly VisualElement _imageContainer = new() { style = { width = 100, height = 90 } };
    private readonly Label _nameLabel = new();

    private bool _selected = false;
    private PassengerStatus _status = PassengerStatus.Comfortable;

    protected Label StatusLabel { get; } = new() { style = { color = _comfortableColor } };
    protected virtual Texture2D BackgroundImage => _backgroundImage;
    protected virtual Texture2D BackgroundImageSelected => _backgroundImageSelected;

    public PassengerStatus Status
    {
        get => _status;
        private set
        {
            _status = value;
            StatusLabel.text = $"({value})";
            StatusLabel.style.color = StatusToColor(value);
        }
    }
    public bool Selected
    {
        get => _selected;
        set
        {
            _imageContainer.style.backgroundImage = value
                ? BackgroundImageSelected
                : BackgroundImage;
            _selected = value;
        }
    }

    public Passenger()
    {
        _imageContainer.style.backgroundImage = BackgroundImage;
        ui.Add(_imageContainer);
        ui.Add(_nameLabel);
        ui.Add(StatusLabel);

        RefreshLabels();

        ui.RegisterCallback<ClickEvent>(OnClick);
    }

    public Passenger(PassengerSerializable passengerSerializable)
        : this()
    {
        Name = passengerSerializable.name;
        ChangeStatus((int)passengerSerializable.status);

        RefreshLabels();
    }

    public static Color StatusToColor(PassengerStatus status)
    {
        return status switch
        {
            PassengerStatus.Comfortable => _comfortableColor,
            PassengerStatus.Cold => _coldColor,
            PassengerStatus.Sick => _sickColor,
            PassengerStatus.Terminal => _terminalColor,
            PassengerStatus.Critical => _criticalColor,
            _ => _deathColor,
        };
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
        if (!selectable)
            return;

        Selected = !Selected;
    }

    protected void ChangeStatus(int newStatus)
    {
        IEnumerable<int> values = Enum.GetValues(typeof(PassengerStatus)).Cast<int>();
        int min = values.Min();
        int max = values.Max();

        Status = (PassengerStatus)Math.Clamp(newStatus, min, max);
    }

    protected void RefreshLabels()
    {
        _nameLabel.text = Name;
        StatusLabel.text = $"({_status})";
    }
}
