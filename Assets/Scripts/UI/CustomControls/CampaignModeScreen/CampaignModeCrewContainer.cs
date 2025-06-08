using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class CampaignModeCrewContainer : VisualElement
{
    public static readonly Color backgroundColor = UiUtils.HexToRgb("#384246");
    public List<CrewCooldownIndicator> cooldownIndicators = new();

    public CampaignModeCrewContainer()
    {
        style.height = UiUtils.GetLengthPercentage(95);
        style.width = 150;
        style.backgroundColor = backgroundColor;
        style.alignSelf = Align.Center;
        style.marginLeft = 12;
        style.borderTopLeftRadius = 8;
        style.borderTopRightRadius = 8;
        style.borderBottomLeftRadius = 8;
        style.borderBottomRightRadius = 8;
        style.display = DisplayStyle.Flex;
        style.flexDirection = FlexDirection.Column;
        style.justifyContent = Justify.SpaceEvenly;
        style.alignItems = Align.FlexStart;

        UiUtils.ToggleBorder(this, true, Color.white);
        UiUtils.SetBorderWidth(this, 1);

        for (int i = 0; i < CampaignModeManager.NUMBER_OF_CREWS; i++)
        {
            VisualElement imageContainer = new()
            {
                style =
                {
                    position = Position.Relative,
                    width = UiUtils.GetLengthPercentage(100),
                    height = 100,
                },
            };

            imageContainer.Add(
                new Image()
                {
                    sprite = UiUtils.LoadSprite("crew_icon", GameplayMode.CampaignMode),
                    style =
                    {
                        width = 100,
                        height = 100,
                        marginLeft = 8,
                    },
                }
            );

            CrewCooldownIndicator cooldownContainer = new();
            imageContainer.Add(cooldownContainer);

            cooldownIndicators.Add(cooldownContainer);

            Add(imageContainer);
        }
    }

    public void RefreshCooldown()
    {
        for (int i = 0; i < CampaignModeManager.Instance.CrewCooldowns.Length; i++)
        {
            cooldownIndicators[i].NumberLabel.text = CampaignModeManager
                .Instance.CrewCooldowns[i]
                .ToString();
        }
    }
}
