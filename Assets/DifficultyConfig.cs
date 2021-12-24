/// <summary>
/// Difficulty Configurations that allows different levels of difficulty
/// with the game.
/// </summary>
public struct DifficultyConfig
{

    public DifficultyConfig(string tag, int totalLanes, float initDuration, float durationDelta, float durationCap, float levelDividend)
    {
        Tag = tag;
        StartingLaneCount = totalLanes;
        InitDuration = initDuration;
        DurationDelta = durationDelta;
        DurationCap = durationCap;
        LevelDividend = levelDividend;
    }

    public string Tag { get; private set; }

    #region Starting Lanes
    public int StartingLaneCount { get; private set; }
    #endregion

    #region Timer Related
    public float InitDuration { get; private set; }
    public float DurationDelta { get; set; }
    public float DurationCap { get; private set; }
    public float LevelDividend { get; private set; } 
    #endregion
}
