using System;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class TrainList : VisualElement
{
    public TrainList()
    {
        style.height = UiUtils.GetLengthPercentage(100);
        style.width = UiUtils.GetLengthPercentage(100);

        ScrollView scrollView = new();
        Add(scrollView);

        RegisterCallback<AttachToPanelEvent>(
            (e) =>
            {
                if (GameManager.Instance == null)
                    return;

                scrollView.Clear();

                foreach (Train train in GameManager.Instance.Trains)
                {
                    VisualElement trainCard = new();
                    scrollView.Add(
                        new TrainCard(train, e.destinationPanel.visualTree.layout.height / 5)
                    );

                    if (!train.unlocked)
                    {
                        trainCard.Add(new Label($"${train.trainSO.price}"));
                        trainCard.style.backgroundColor = new Color(0, 0, 0, 0.5f);
                        trainCard.style.color = Color.white;

                        Button buyButton = new() { text = "Buy" };

                        trainCard.Add(buyButton);
                    }
                    else
                    {
                        Button upgradeButton = new() { text = "Upgrade" };

                        upgradeButton.clicked += () =>
                        {
                            UiManager.Instance.GameplayScreen.ChangeRightPanel(
                                new TrainUpgradePanel(train)
                            );
                        };

                        trainCard.Add(upgradeButton);
                    }
                }
            }
        );
    }

    private void OnBuyButtonClicked(Button buyButton, VisualElement trainCard)
    {
        trainCard.style.backgroundColor = new Color(0, 0, 0, 0);
        trainCard.style.color = Color.black;

        buyButton.text = "Upgrade";
        //buyButton.clicked -= OnBuyButtonClicked;
    }
}
