public enum MissionType
{
    Rescue,
    Resupply,
    Documentation,
}

public enum PassengerStatus
{
    Comfortable,
    Cold,
    Sick,
    Terminal,
    Critical,
    Death,
}

public enum MaterialType
{
    Payments,
    Supplies,
    Resources,
    Residents,
    Citizens,
};

public enum ConsoleOutputLevel
{
    Error,
    Info,
    Success,
}

public enum MissionStatus
{
    Pending,
    Deployed,
    Arrived,
    Completed,
}

public enum Direction
{
    Up,
    Down,
    Left,
    Right,
    Center,
}

public enum ActionType
{
    Medical,
    Warming,
}

public enum Scene
{
    MainMenu = 0,
    CityMode = 1,
    CampaignMode = 2,
    DialogMode,
}

public enum DialogSceneType
{
    Title,
    Subtitle,
    Blank,
    Dialog,
    RhythmGame,
}
