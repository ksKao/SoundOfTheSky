using System;

[Serializable]
public class Beatmap
{
    public BeatmapNote[] notes;
}

[Serializable]
public class BeatmapNote
{
    // time for it to initially appear
    public int showSecond;
    public int startSecond;
    public int endSecond;
    public RhythmGameLane lane;
}
