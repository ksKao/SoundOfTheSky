using System;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class MaterialsBar : VisualElement
{
    private readonly Label _paymentsLabel = new();
    private readonly Label _suppliesLabel = new();
    private readonly Label _resourcesLabel = new();
    private readonly Label _citizensLabel = new();

    public MaterialsBar()
    {
        style.display = DisplayStyle.Flex;
        style.flexDirection = FlexDirection.Row;

        AddMaterialLabel(MaterialType.Payments, _paymentsLabel);
        AddMaterialLabel(MaterialType.Supplies, _suppliesLabel);
        AddMaterialLabel(MaterialType.Resources, _resourcesLabel);
        AddMaterialLabel(MaterialType.Citizens, _citizensLabel);
    }

    public void RefreshAllMaterialAmountUi()
    {
        MaterialType[] allMaterialTypes = (MaterialType[])Enum.GetValues(typeof(MaterialType));
        foreach (MaterialType materialType in allMaterialTypes)
        {
            UpdateMaterialAmount(materialType, GameManager.Instance.GetMaterialValue(materialType));
        }
    }

    public void UpdateMaterialAmount(MaterialType materialType, int amount)
    {
        Label materialLabel = this.Query<Label>(name: materialType.ToString()).First();

        if (materialLabel is null)
        {
            Debug.LogError($"Could not find label with name {materialType}.");
            return;
        }

        materialLabel.text = $"{amount}x";
    }

    private void AddMaterialLabel(MaterialType materialType, Label amountLabel)
    {
        VisualElement container = new();
        container.style.display = DisplayStyle.Flex;
        container.style.flexDirection = FlexDirection.Row;

        VisualElement icon = new();
        icon.style.marginRight = 10;

        amountLabel.text = "0x";
        amountLabel.name = materialType.ToString();
        amountLabel.style.marginLeft = 10;

        container.Add(icon);
        container.Add(new Label(materialType.ToString()));
        container.Add(amountLabel);

        Add(container);
    }
}
