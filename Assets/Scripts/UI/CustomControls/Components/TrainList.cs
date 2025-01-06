using UnityEngine.UIElements;

[UxmlElement]
public partial class TrainList : VisualElement
{
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
                    VisualElement trainCard = new();
                    scrollView.Add(trainCard);

                    trainCard.style.height = e.destinationPanel.visualTree.layout.height / 5;
                    trainCard.Add(new Label(train.trainSO.name));
                }
            }
        );
    }
}
