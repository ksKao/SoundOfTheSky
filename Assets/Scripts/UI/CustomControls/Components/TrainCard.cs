using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class TrainCard : VisualElement
{
    private static readonly Texture2D _trainCardBackground = UiUtils.LoadTexture("train_upgrade_overview_background");
    private static readonly Texture2D _buyButtonBackground = UiUtils.LoadTexture("train_buy_button");

    private readonly Train _train;
    private readonly VisualElement _topContainer = new()
    {
        style =
        {
            display = DisplayStyle.Flex,
            flexDirection = FlexDirection.Row,
            justifyContent = Justify.SpaceBetween,
            width = UiUtils.GetLengthPercentage(100)
        }
    };
    private readonly Label _priceLabel = new();
    private Button _button = new();
    private readonly VisualElement _overlay = new()
    {
        style =
        {
            position = Position.Absolute,
            left = 0,
            top = 0,
            backgroundColor = new Color(0, 0, 0, 0.5f),
            width = UiUtils.GetLengthPercentage(100),
            height = UiUtils.GetLengthPercentage(100),
            borderTopLeftRadius = 22,
            borderTopRightRadius = 22,
            borderBottomLeftRadius = 22,
            borderBottomRightRadius = 22
        }
    };

    public TrainCard()
    {
        Debug.LogWarning($"Detected calling default constructor of {nameof(TrainCard)}.");
    }

    public TrainCard(Train train, float height)
    {
        _train = train;

        style.height = height;
        style.backgroundImage = _trainCardBackground;
        style.color = UiUtils.darkBlueTextColor;
        style.marginBottom = 20;
        style.paddingTop = UiUtils.GetLengthPercentage(3);
        style.paddingBottom = UiUtils.GetLengthPercentage(3);
        style.paddingLeft = UiUtils.GetLengthPercentage(3);
        style.paddingRight = UiUtils.GetLengthPercentage(3);
        style.display = DisplayStyle.Flex;
        style.flexDirection = FlexDirection.Column;
        style.alignItems = Align.Center;
        style.position = Position.Relative;

        _topContainer.Add(new Label(train.trainSO.name.ToUpper()));
        _topContainer.Add(_priceLabel);
        _topContainer.Add(new() { style = { width = 100 } }); // add an empty element to align items properly because the button is absolutely positioned

        Add(_topContainer);
        Add(new Image()
        {
            sprite = train.trainSO.sprite,
            style =
            {
                width = 600,
                height = 100
            }
        });
        Add(_overlay);
        Add(_button);

        Refresh();
    }

    private void Refresh()
    {
        Remove(_button);
        _button = new()
        {
            style =
            {
                backgroundImage = _buyButtonBackground,
                backgroundColor = Color.clear,
                color = Color.white,
                width = 100,
                position = Position.Absolute,
                right = UiUtils.GetLengthPercentage(2.5f),
                top = 25
            }
        };
        UiUtils.ToggleBorder(_button, false);

        if (_train.unlocked)
        {
            _overlay.visible = false;
            _priceLabel.text = "";
            _button.text = "UPGRADE";
            _button.clicked += () =>
            {
                UiManager.Instance.GameplayScreen.ChangeRightPanel(new TrainUpgradePanel(_train));
            };
        }
        else
        {
            _overlay.visible = true;
            _priceLabel.text = $"${_train.trainSO.price}";
            _button.text = "BUY";
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

        Add(_button);
    }
}
