using System;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class MaterialsBar : VisualElement
{
    public MaterialsBar()
    {
        style.display = DisplayStyle.Flex;
        style.flexDirection = FlexDirection.Row;
        style.alignItems = Align.Center;

        MaterialType[] allMaterialTypes = (MaterialType[])Enum.GetValues(typeof(MaterialType));
        foreach (MaterialType materialType in allMaterialTypes)
        {
            AddMaterialLabel(materialType);
        }
    }

    public void RefreshAllMaterialAmountUi()
    {
        UpdateMaterialAmount(
            MaterialType.Payments,
            CityModeManager.Instance.GetMaterialValue(MaterialType.Payments)
        );
        UpdateMaterialAmount(
            MaterialType.Resources,
            CityModeManager.Instance.GetMaterialValue(MaterialType.Resources)
        );
        UpdateMaterialAmount(
            MaterialType.Supplies,
            CityModeManager.Instance.GetMaterialValue(MaterialType.Supplies)
        );

        int numberOfResidents = 0;
        int numberOfCitizens = 0;

        foreach (Location location in CityModeManager.Instance.Locations)
        {
            numberOfResidents += location.Residents;
            numberOfCitizens += location.Citizens;
        }

        UpdateMaterialAmount(MaterialType.Residents, numberOfResidents);
        UpdateMaterialAmount(MaterialType.Citizens, numberOfCitizens);
    }

    private void UpdateMaterialAmount(MaterialType materialType, int amount)
    {
        Label materialLabel = this.Query<Label>(name: materialType.ToString()).First();

        if (materialLabel is null)
        {
            Debug.LogError($"Could not find label with name {materialType}.");
            return;
        }

        materialLabel.text = $"{amount}x";
    }

    private void AddMaterialLabel(MaterialType materialType)
    {
        VisualElement container = new();
        container.style.display = DisplayStyle.Flex;
        container.style.flexDirection = FlexDirection.Row;

        Image icon = new()
        {
            style = { height = 24 },
            sprite = UiUtils.LoadSprite(materialType.ToString().ToLower()),
        };

        Label materialTypeLabel = new()
        {
            text = materialType.ToString(),
            style = { color = Color.white },
        };

        Label amountLabel = new()
        {
            text = "0x",
            name = materialType.ToString(),
            style = { color = Color.white },
        };

        container.Add(icon);
        container.Add(materialTypeLabel);
        container.Add(amountLabel);

        Add(container);
    }
}
