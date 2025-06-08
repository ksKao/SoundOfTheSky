using System;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class PassengersWindow : VisualElement
{
    public readonly VisualElement modal = new()
    {
        style =
        {
            position = Position.Absolute,
            top = 0,
            left = 0,
            width = UiUtils.GetLengthPercentage(85),
            height = UiUtils.GetLengthPercentage(85),
            backgroundImage = UiUtils.LoadTexture(
                "passenger_window_background",
                GameplayMode.CampaignMode
            ),
        },
    };

    private readonly VisualElement _passengersContainer = new()
    {
        style =
        {
            display = DisplayStyle.Flex,
            flexDirection = FlexDirection.Row,
            justifyContent = Justify.SpaceBetween,
            flexWrap = Wrap.Wrap,
            flexGrow = 1,
        },
    };

    public PassengersWindow()
    {
        style.backgroundImage = UiUtils.LoadTexture(
            "passenger_window_background",
            GameplayMode.CampaignMode
        );
        style.position = Position.Absolute;
        style.width = UiUtils.GetLengthPercentage(20);
        style.height = UiUtils.GetLengthPercentage(80);
        style.top = 32;
        style.left = 32;

        UiUtils.ToggleBorder(modal, true, Color.white);
        UiUtils.SetBorderWidth(modal, 1);

        VisualElement modalContentContainer = new()
        {
            style =
            {
                position = Position.Relative,
                width = UiUtils.GetLengthPercentage(100),
                height = UiUtils.GetLengthPercentage(100),
                display = DisplayStyle.Flex,
                flexDirection = FlexDirection.Row,
                paddingLeft = 96,
                paddingTop = 16,
            },
        };

        modal.Add(modalContentContainer);

        modalContentContainer.Add(
            new()
            {
                style =
                {
                    position = Position.Absolute,
                    top = 0,
                    left = 0,
                    backgroundColor = Color.black,
                    opacity = 0.95f,
                    width = UiUtils.GetLengthPercentage(100),
                    height = UiUtils.GetLengthPercentage(100),
                },
            }
        );

        modalContentContainer.Add(_passengersContainer);

        VisualElement statusLabelContainer = new()
        {
            style =
            {
                borderTopLeftRadius = 16,
                borderTopRightRadius = 16,
                borderBottomLeftRadius = 16,
                borderBottomRightRadius = 16,
                display = DisplayStyle.Flex,
                flexDirection = FlexDirection.Column,
                alignItems = Align.FlexStart,
                paddingLeft = 24,
                paddingRight = 24,
                paddingTop = 16,
                paddingBottom = 16,
                backgroundColor = Color.black,
                opacity = 0.8f,
                width = UiUtils.GetLengthPercentage(10),
                marginRight = 24,
                marginBottom = new StyleLength() { keyword = StyleKeyword.Auto },
            },
        };

        UiUtils.ToggleBorder(statusLabelContainer, true, Color.white);
        UiUtils.SetBorderWidth(statusLabelContainer, 1);

        foreach (PassengerStatus status in Enum.GetValues(typeof(PassengerStatus)))
        {
            statusLabelContainer.Add(
                new Label()
                {
                    text = status == PassengerStatus.Death ? $"<s>{status}</s>" : status.ToString(),
                    style =
                    {
                        color =
                            status == PassengerStatus.Death
                                ? Color.white
                                : Passenger.StatusToColor(status),
                        paddingTop = 0,
                        paddingBottom = 0,
                        paddingLeft = 0,
                        paddingRight = 0,
                        marginTop = 0,
                        marginBottom = status == PassengerStatus.Death ? 0 : 8,
                        marginLeft = 0,
                        marginRight = 0,
                    },
                }
            );
        }

        modalContentContainer.Add(statusLabelContainer);

        RegisterCallback<ClickEvent>(
            (e) =>
            {
                UiManager.Instance.Modal.Show(modal);
                Refresh();
            }
        );
    }

    public void Refresh()
    {
        _passengersContainer.Clear();

        foreach ((string name, PassengerStatus status) in CampaignModeManager.Instance.Passengers)
        {
            VisualElement container = new()
            {
                style =
                {
                    display = DisplayStyle.Flex,
                    flexDirection = FlexDirection.Column,
                    alignItems = Align.Center,
                    justifyContent = Justify.Center,
                    width = UiUtils.GetLengthPercentage(20),
                    height = UiUtils.GetLengthPercentage(25),
                },
            };

            container.Add(
                new Image()
                {
                    sprite = UiUtils.LoadSprite("passenger_icon", GameplayMode.CampaignMode),
                    style = { marginBottom = 8 },
                }
            );

            container.Add(
                new Label()
                {
                    text = status == PassengerStatus.Death ? $"<s>{name}</s>" : name,
                    style =
                    {
                        color = Color.white,
                        paddingTop = 0,
                        paddingBottom = 0,
                        paddingLeft = 0,
                        paddingRight = 0,
                        marginTop = 0,
                        marginBottom = 4,
                        marginLeft = 0,
                        marginRight = 0,
                    },
                }
            );

            if (status != PassengerStatus.Death)
                container.Add(
                    new Label()
                    {
                        text = status.ToString(),
                        style =
                        {
                            color = Passenger.StatusToColor(status),
                            paddingTop = 0,
                            paddingBottom = 0,
                            paddingLeft = 0,
                            paddingRight = 0,
                            marginTop = 0,
                            marginBottom = 16,
                            marginLeft = 0,
                            marginRight = 0,
                        },
                    }
                );

            _passengersContainer.Add(container);
        }
    }
}
