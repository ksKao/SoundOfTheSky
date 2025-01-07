using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class TrainUpgradePanel : VisualElement
{
    public TrainUpgradePanel()
    {
        Debug.LogWarning($"Detected calling default constructor of {nameof(TrainUpgradePanel)}.");
    }

    public TrainUpgradePanel(Train train)
    {
        Add(new Label(train.trainSO.name));
    }
}
