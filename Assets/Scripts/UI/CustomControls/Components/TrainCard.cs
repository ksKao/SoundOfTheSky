using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class TrainCard : VisualElement
{
    private readonly Train _train;
    private readonly VisualElement _buttonSlot = new();
    private Button _button = new();

    public TrainCard()
    {
        Debug.LogWarning($"Detected calling default constructor of {nameof(TrainCard)}.");
    }

    public TrainCard(Train train, float height)
    {
        _train = train;

        style.height = height;
        Add(new Label(train.trainSO.name));

        Refresh();

        Add(_buttonSlot);
        _buttonSlot.Add(_button);
    }

    private void Refresh()
    {
        if (_train.unlocked)
        {
            style.backgroundColor = new StyleColor() { keyword = StyleKeyword.None };
            style.color = Color.black;

            _button = new() { text = "Upgrade" };
            _button.clicked += () =>
            {
                UiManager.Instance.GameplayScreen.ChangeRightPanel(new TrainUpgradePanel(_train));
            };
        }
        else
        {
            Add(new Label($"${_train.trainSO.price}"));
            style.backgroundColor = new Color(0, 0, 0, 0.5f);
            style.color = Color.white;

            _button = new() { text = "Buy" };
            _button.clicked += () =>
            {
                if (
                    GameManager.Instance.GetMaterialValue(MaterialType.Payments)
                    < _train.trainSO.price
                )
                {
                    Debug.Log("You don't have enough payments to buy this train.");
                    return;
                }

                GameManager.Instance.IncrementMaterialValue(
                    MaterialType.Payments,
                    -_train.trainSO.price
                );

                _train.unlocked = true;
                Refresh();
            };
        }

        _buttonSlot.Clear();
        _buttonSlot.Add(_button);
    }
}
