using System;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class ConsoleUi : VisualElement
{
    public readonly ScrollView outputContainer = new()
    {
        mode = ScrollViewMode.Vertical,
        style =
        {
            display = DisplayStyle.Flex,
            flexDirection = FlexDirection.Column,
            flexGrow = 1,
            width = UiUtils.GetLengthPercentage(100),
            paddingLeft = 12,
            paddingRight = 12
        }
    };

    public readonly TextField textField = new()
    {
        textEdition =
        {
            placeholder = "Enter \"help\" to get a list of command",
            hidePlaceholderOnFocus = true,
        },
        style =
        {
            height = 36,
            width = UiUtils.GetLengthPercentage(100)
        }
    };

    public ConsoleUi()
    {
        style.position = Position.Absolute;
        style.width = UiUtils.GetLengthPercentage(50);
        style.height = UiUtils.GetLengthPercentage(50);
        style.top = UiUtils.GetLengthPercentage(50);
        style.left = UiUtils.GetLengthPercentage(50);
        style.backgroundColor = UiUtils.HexToRgb("#222222");
        style.translate = new Translate(UiUtils.GetLengthPercentage(-50), UiUtils.GetLengthPercentage(-50));
        style.display = DisplayStyle.Flex;
        style.flexDirection = FlexDirection.Column;

        Add(outputContainer);

        Add(textField);
    }

    public void AddOutput(string message, ConsoleOutputLevel consoleOutputLevel)
    {
        Label output = new()
        {
            text = message,
            style =
            {
                whiteSpace = WhiteSpace.Normal,
                color = consoleOutputLevel switch
                {
                    ConsoleOutputLevel.Error => Color.red,
                    ConsoleOutputLevel.Info => Color.white,
                    ConsoleOutputLevel.Success => Color.green,
                    _ => throw new NotImplementedException("Invalid console output level")
                }
            }
        };
        outputContainer.Add(output);
    }
}
