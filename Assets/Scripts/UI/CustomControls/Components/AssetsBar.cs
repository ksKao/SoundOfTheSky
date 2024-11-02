using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class AssetsBar : VisualElement
{
    private readonly Label _paymentsLabel = new();
    private readonly Label _suppliesLabel = new();
    private readonly Label _resourcesLabel = new();
    private readonly Label _citizensLabel = new();

    public AssetsBar()
    {
        style.display = DisplayStyle.Flex;
        style.flexDirection = FlexDirection.Row;

        AddAssetLabel(AssetType.Payments, _paymentsLabel);
        AddAssetLabel(AssetType.Supplies, _suppliesLabel);
        AddAssetLabel(AssetType.Resources, _resourcesLabel);
        AddAssetLabel(AssetType.Citizens, _citizensLabel);
    }

    public void UpdateAssetAmount(AssetType assetType, int amount)
    {
        Label assetLabel = this.Query<Label>(name: assetType.ToString()).First();

        if (assetLabel is null)
        {
            Debug.LogError($"Could not find label with name {assetType}.");
            return;
        }

        assetLabel.text = $"{amount}x";
    }

    private void AddAssetLabel(AssetType assetType, Label amountLabel)
    {
        VisualElement container = new();
        container.style.display = DisplayStyle.Flex;
        container.style.flexDirection = FlexDirection.Row;

        VisualElement icon = new();
        icon.style.marginRight = 10;

        amountLabel.text = "0x";
        amountLabel.name = assetType.ToString();
        amountLabel.style.marginLeft = 10;

        container.Add(icon);
        container.Add(new Label(assetType.ToString()));
        container.Add(amountLabel);

        Add(container);
    }
}
