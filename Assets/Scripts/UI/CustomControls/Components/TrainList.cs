using System.Collections.Generic;
using UnityEngine.UIElements;

[UxmlElement]
public partial class TrainList : VisualElement
{
    private readonly List<TrainCard> _trainCards = new();

    public TrainList()
    {
        style.height = UiUtils.GetLengthPercentage(100);
        style.width = UiUtils.GetLengthPercentage(100);

        ScrollView scrollView = new();
        Add(scrollView);

        RegisterCallback<AttachToPanelEvent>(
            (e) =>
            {
                if (GameManager.Instance == null)
                    return;

                scrollView.Clear();

                foreach (Train train in GameManager.Instance.Trains)
                {
                    TrainCard trainCard = new(train, e.destinationPanel.visualTree.layout.height / 6);
                    _trainCards.Add(trainCard);
                    scrollView.Add(trainCard);
                }
            }
        );
    }

    public void Refresh()
    {
        _trainCards.ForEach(c => c.Refresh());
    }
}
