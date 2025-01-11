using System;

public class Train
{
    public readonly TrainSO trainSO;
    public bool unlocked = false;

    private int _cartLevel = 1;
    private int _speedLevel = 1;
    private int _warmthLevel = 1;

    public int CartLevel
    {
        get => _cartLevel;
        set => _cartLevel = Math.Min(value, GameManager.MAX_UPGRADE_LEVEL);
    }
    public int SpeedLevel
    {
        get => _speedLevel;
        set => _speedLevel = Math.Min(value, GameManager.MAX_UPGRADE_LEVEL);
    }
    public int WarmthLevel
    {
        get => _warmthLevel;
        set => _warmthLevel = Math.Min(value, GameManager.MAX_UPGRADE_LEVEL);
    }

    public Train(TrainSO trainSO)
    {
        this.trainSO = trainSO;
    }
}
