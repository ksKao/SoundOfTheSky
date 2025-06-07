using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class ChoiceButton : VisualElement
{
    public ChoiceButton()
    {
        Debug.LogWarning($"Detected calling default constructor of {nameof(ChoiceButton)}");
    }

    public ChoiceButton(ActionSO actionSO)
    {
        style.marginBottom = 8;

        Button button = new()
        {
            style =
            {
                borderTopLeftRadius = 8,
                borderTopRightRadius = 8,
                borderBottomLeftRadius = 8,
                borderBottomRightRadius = 8,
                display = DisplayStyle.Flex,
                flexDirection = FlexDirection.Column,
                alignItems = Align.FlexStart,
                justifyContent = Justify.SpaceBetween,
                paddingTop = 4,
                paddingBottom = 4,
                paddingLeft = 8,
                paddingRight = 8,
            },
        };

        UiUtils.ToggleBorder(button, false);

        if (actionSO.type == ActionType.Warming)
        {
            button.style.backgroundColor = MainChoicesContainer.redColor;
            button.style.color = Color.white;
            button.Add(
                new Label(
                    $"{actionSO.name}: <b>+{actionSO.valueIncrease}{WeatherBar.DEGREE_SYMBOL}</b>"
                )
                {
                    style =
                    {
                        paddingTop = 0,
                        paddingBottom = 0,
                        paddingLeft = 0,
                        paddingRight = 0,
                    },
                }
            );
        }
        else
        {
            button.style.backgroundColor = Color.white;
            button.style.color = MainChoicesContainer.redColor;
            button.Add(
                new Label($"{actionSO.name}: <b>+{actionSO.valueIncrease} health</b>")
                {
                    style =
                    {
                        paddingTop = 0,
                        paddingBottom = 0,
                        paddingLeft = 0,
                        paddingRight = 0,
                    },
                }
            );
        }

        button.Add(
            new Label($"Crew: {actionSO.crewsNeeded}")
            {
                style =
                {
                    paddingTop = 0,
                    paddingBottom = 0,
                    paddingLeft = 0,
                    paddingRight = 0,
                },
            }
        );
        button.Add(
            new Label($"Cooldown: <b>{actionSO.cooldownMin} - {actionSO.cooldownMax} days</b>")
            {
                style =
                {
                    paddingTop = 0,
                    paddingBottom = 0,
                    paddingLeft = 0,
                    paddingRight = 0,
                },
            }
        );

        button.clicked += () => CampaignModeManager.Instance.ApplyAction(actionSO);

        Add(button);
    }
}
