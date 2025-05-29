using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class NumberInput : VisualElement
{
    private string _text = "";
    private int _value = 0;

    private float _secondsSinceStartHold = 0;
    private readonly Label _textLabel = new();
    private readonly Label _valueLabel = new();
    private readonly Button _incrementButton = new()
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
    private readonly Button _decrementButton = new()
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
            _textLabel.text = value;
        }
    }

    [UxmlAttribute]
    public int Value
    {
        get => _value;
        set
        {
            _value = Math.Max(0, value);
            _valueLabel.text = _value.ToString();
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

        _incrementButton.clicked += IncrementValue;
        _incrementButton.text = ">";
        _incrementButton.visible = false;
        UiUtils.ToggleBorder(_incrementButton, false);

        _decrementButton.clicked += DecrementValue;
        _decrementButton.text = "<";
        _decrementButton.visible = false;
        UiUtils.ToggleBorder(_decrementButton, false);

        Add(_textLabel);

        VisualElement buttonsContainer = new()
        {
            style =
            {
                display = DisplayStyle.Flex,
                flexDirection = FlexDirection.Row,
                alignItems = Align.Center,
            },
        };
        buttonsContainer.Add(_decrementButton);
        buttonsContainer.Add(_valueLabel);
        buttonsContainer.Add(_incrementButton);

        Add(buttonsContainer);

        Coroutine incrementCoroutine = null;
        Coroutine decrementCoroutine = null;

        _incrementButton.RegisterCallback<PointerDownEvent>(
            e =>
            {
                _secondsSinceStartHold = 0;
                incrementCoroutine = GameManager.Instance.StartCoroutine(IncrementCoroutine());
            },
            TrickleDown.TrickleDown
        );
        _incrementButton.RegisterCallback<PointerUpEvent>(
            e =>
            {
                if (incrementCoroutine is not null)
                    GameManager.Instance.StopCoroutine(incrementCoroutine);
            },
            TrickleDown.TrickleDown
        );

        _decrementButton.RegisterCallback<PointerDownEvent>(
            e =>
            {
                _secondsSinceStartHold = 0;
                decrementCoroutine = GameManager.Instance.StartCoroutine(DecrementCoroutine());
            },
            TrickleDown.TrickleDown
        );
        _decrementButton.RegisterCallback<PointerUpEvent>(
            e =>
            {
                if (decrementCoroutine is not null)
                    GameManager.Instance.StopCoroutine(decrementCoroutine);
            },
            TrickleDown.TrickleDown
        );
    }

    private void IncrementValue()
    {
        if (_secondsSinceStartHold >= 10)
            Value += 20;
        else if (_secondsSinceStartHold >= 5)
            Value += 10;
        else if (_secondsSinceStartHold >= 3)
            Value += 5;
        else
            Value++;
    }

    private void DecrementValue()
    {
        if (_secondsSinceStartHold >= 10)
            Value -= 20;
        else if (_secondsSinceStartHold >= 5)
            Value -= 10;
        else if (_secondsSinceStartHold >= 3)
            Value -= 5;
        else
            Value--;
    }

    private IEnumerator IncrementCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            _secondsSinceStartHold += 0.1f;
            IncrementValue();
        }
    }

    private IEnumerator DecrementCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            _secondsSinceStartHold += 0.1f;
            DecrementValue();
        }
    }
}
