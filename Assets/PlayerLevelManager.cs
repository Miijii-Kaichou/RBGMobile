using System;
using UnityEngine;

public class PlayerLevelManager : Singleton<PlayerLevelManager>
{
    public static float PreviousLevelExperiencePoints = 0;
    public static float LevelExperiencePoints = 0;
    public static int CurrentLevel
    {
        get
        {
            return Mathf.FloorToInt(LevelExperiencePoints / ExperienceMax) + 1;
        }
    }
    public const float MaxLevel = 200;
    public const float ExperienceMax = 100;
    public static float Percentage => (LevelExperiencePoints / ExperienceMax) % 1f;
    
    public static void AddExperience(float value)
    {
        LevelExperiencePoints += value * (((MaxLevel + 1f) - (float)CurrentLevel) / MaxLevel);
    }

    internal static void Reset()
    {
        LevelExperiencePoints = 0;
        PreviousLevelExperiencePoints = LevelExperiencePoints;
    }
}
