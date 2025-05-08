using System;
using UnityEngine;
using UnityEngine.UIElements;

public class Crew : Passenger
{
    private static readonly Texture2D _backgroundImage = UiUtils.LoadTexture(
        "crew_selection_background"
    );
    private static readonly Texture2D _backgroundImageSelected = UiUtils.LoadTexture(
        "crew_selection_background_glow"
    );

    public const int MAX_ENDURANCE_LEVEL = 8;

    public static event Action<Crew> OnSelect;

    public bool isResting = false;
    public readonly Label bracketLabel = new();

    public Mission deployedMission;

    private int _medicLevel = 1;
    private int _enduranceLevel = 1;

    public int MedicLevel
    {
        get => _medicLevel;
        set => _medicLevel = Math.Min(value, GameManager.MAX_UPGRADE_LEVEL);
    }
    public int EnduranceLevel
    {
        get => _enduranceLevel;
        set => _enduranceLevel = Math.Min(value, MAX_ENDURANCE_LEVEL);
    }
    public int MedicLevelPercentage => (_medicLevel - 1) * 2;
    public int EnduranceLevelPercentage => (_enduranceLevel - 1) * 5;

    protected override Texture2D BackgroundImage => _backgroundImage;
    protected override Texture2D BackgroundImageSelected => _backgroundImageSelected;

    public Crew()
        : base()
    {
        ui.Add(bracketLabel);
    }

    protected override void OnClick(ClickEvent _)
    {
        if (!selectable)
            return;

        Selected = !Selected;
        OnSelect.Invoke(this);
    }
}
