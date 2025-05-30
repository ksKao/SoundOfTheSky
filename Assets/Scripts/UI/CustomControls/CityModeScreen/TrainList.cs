using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class TrainList : VisualElement
{
    public Train activeTrain = null;

    private readonly List<TrainCard> _trainCards = new();
    private readonly ScrollView _scrollView = new();
    private readonly VisualElement _noTrainText = new()
    {
        style =
        {
            width = UiUtils.GetLengthPercentage(100),
            height = UiUtils.GetLengthPercentage(100),
            justifyContent = Justify.Center,
            alignItems = Align.Center,
        },
    };
    private Action<Train> _previousOnSelect = null;

    public TrainList()
    {
        style.height = UiUtils.GetLengthPercentage(100);
        style.width = UiUtils.GetLengthPercentage(100);

        _noTrainText.Add(
            new Label("No train is eligible for this route.")
            {
                style =
                {
                    color = Color.white,
                    fontSize = 24,
                    whiteSpace = WhiteSpace.Normal,
                },
            }
        );

        Add(_noTrainText);
        Add(_scrollView);
    }

    public void Show(Train[] trains, Action<Train> newOnSelect = null, Train activeTrain = null)
    {
        if (_previousOnSelect is not null)
        {
            foreach (TrainCard trainCard in _trainCards)
                trainCard.OnSelectClicked -= _previousOnSelect;
        }

        _previousOnSelect = newOnSelect;
        this.activeTrain = activeTrain;

        _trainCards.Clear();
        _scrollView.Clear();

        foreach (Train train in trains)
        {
            TrainCard trainCard = new(train, layout.height / 6, newOnSelect is not null);

            if (newOnSelect is not null)
                trainCard.OnSelectClicked += newOnSelect;

            _trainCards.Add(trainCard);
            _scrollView.Add(trainCard);
        }

        if (trains.Length == 0)
        {
            _scrollView.style.display = DisplayStyle.None;
            _noTrainText.style.display = DisplayStyle.Flex;
        }
        else
        {
            _scrollView.style.display = DisplayStyle.Flex;
            _noTrainText.style.display = DisplayStyle.None;
        }

        UiManager.Instance.CityModeScreen.ChangeRightPanel(this);
    }

    public void Refresh()
    {
        _trainCards.ForEach(c => c.Refresh());
    }
}
