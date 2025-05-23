using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class NumberInput : VisualElement
{
    private string _text = "";
    private int _value = 0;

    private readonly Label TextLabel = new();
    private readonly Label ValueLabel = new();
    private readonly Button IncrementButton = new()
    {
        style =
        {
            backgroundColor = Color.clear,
            paddingLeft = 0,
            paddingRight = 0,
            paddingTop = 0,
            paddingBottom = 0,
            height = UiUtils.GetLengthPercentage(100),
        },
    };
    private readonly Button DecrementButton = new()
    {
        style =
        {
            backgroundColor = Color.clear,
            paddingLeft = 0,
            paddingRight = 0,
            paddingTop = 0,
            paddingBottom = 0,
            height = UiUtils.GetLengthPercentage(100),
        },
    };

    [UxmlAttribute]
    public string Text
    {
        get => _text;
        set
        {
            _text = value;
            TextLabel.text = value;
        }
    }

    [UxmlAttribute]
    public int Value
    {
        get => _value;
        set
        {
            if (value < 0)
                return;

            _value = value;
            ValueLabel.text = _value.ToString();
        }
    }

    public NumberInput()
    {
        Debug.LogWarning($"Detected calling {nameof(NumberInput)}'s default constructor.");
    }

    public NumberInput(string text, int value = 0)
    {
        Text = text;
        Value = value;

        IncrementButton.clicked += IncrementValue;
        IncrementButton.text = ">";
        IncrementButton.visible = false;
        UiUtils.ToggleBorder(IncrementButton, false);

        DecrementButton.clicked += DecrementValue;
        DecrementButton.text = "<";
        DecrementButton.visible = false;
        UiUtils.ToggleBorder(DecrementButton, false);

        Add(TextLabel);

        VisualElement buttonsContainer = new()
        {
            style =
            {
                display = DisplayStyle.Flex,
                flexDirection = FlexDirection.Row,
                alignItems = Align.Center,
            },
        };
        buttonsContainer.Add(DecrementButton);
        buttonsContainer.Add(ValueLabel);
        buttonsContainer.Add(IncrementButton);

        Add(buttonsContainer);

        Coroutine incrementCoroutine = null;
        Coroutine decrementCoroutine = null;

        IncrementButton.RegisterCallback<PointerDownEvent>(
            e => incrementCoroutine = GameManager.Instance.StartCoroutine(IncrementCoroutine()),
            TrickleDown.TrickleDown
        );
        IncrementButton.RegisterCallback<PointerUpEvent>(
            e =>
            {
                if (incrementCoroutine is not null)
                    GameManager.Instance.StopCoroutine(incrementCoroutine);
            },
            TrickleDown.TrickleDown
        );

        DecrementButton.RegisterCallback<PointerDownEvent>(
            e => decrementCoroutine = GameManager.Instance.StartCoroutine(DecrementCoroutine()),
            TrickleDown.TrickleDown
        );
        DecrementButton.RegisterCallback<PointerUpEvent>(
            e =>
            {
                if (decrementCoroutine is not null)
                    GameManager.Instance.StopCoroutine(decrementCoroutine);
            },
            TrickleDown.TrickleDown
        );
    }

    private void IncrementValue() => Value++;

    private void DecrementValue() => Value--;

    private IEnumerator IncrementCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            IncrementValue();
        }
    }

    private IEnumerator DecrementCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            DecrementValue();
        }
    }
}
