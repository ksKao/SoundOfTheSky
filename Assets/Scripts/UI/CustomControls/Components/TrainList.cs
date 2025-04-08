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
                    scrollView.Add(
                        new TrainCard(train, e.destinationPanel.visualTree.layout.height / 6)
                    );
                }
            }
        );
    }
}
