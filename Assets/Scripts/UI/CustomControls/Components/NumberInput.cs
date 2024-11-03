using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class NumberInput : VisualElement
{
    private string _text = "";
    private int _value = 0;

    private readonly Label TextLabel = new();
    private readonly Label ValueLabel = new();
    private readonly Button IncrementButton = new();
    private readonly Button DecrementButton = new();

    [UxmlAttribute] public string Text
    {
        get => _text;
        set
        {
            _text = value;
            TextLabel.text = value;
        }
    }
    [UxmlAttribute] public int Value
    {
        get => _value;
        set
        {
            if (value < 0) return;

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
        DecrementButton.clicked += DecrementValue;
        DecrementButton.text = "<";
        DecrementButton.visible = false;

        Add(TextLabel);
        Add(IncrementButton);
        Add(ValueLabel);
        Add(DecrementButton);
    }

    private void IncrementValue() => Value++;
    private void DecrementValue() => Value--;
}
