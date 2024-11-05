using UnityEngine;

public static class SaveSystem
{
    // Define keys for saving level progress and high scores
    private static string LevelBeatenKey(int levelNumber) => $"Level{levelNumber}_Beaten";
    private static string LevelHighScoreKey(int levelNumber) => $"Level{levelNumber}_HighScore";

    // Save beaten status of a level
    public static void SaveLevelBeaten(int levelNumber)
    {
        PlayerPrefs.SetInt(LevelBeatenKey(levelNumber), 1); // 1 means beaten
        PlayerPrefs.Save();
    }

    // Save high score if it's a new record
    public static void SaveHighScore(int levelNumber, float time)
    {
        float currentBest = GetHighScore(levelNumber);
        if (time < currentBest || currentBest == 0) // Only save if it's a new record
        {
            PlayerPrefs.SetFloat(LevelHighScoreKey(levelNumber), time);
            PlayerPrefs.Save();
        }
    }

    // Load beaten status of a level (returns true if beaten)
    public static bool IsLevelBeaten(int levelNumber)
    {
        return PlayerPrefs.GetInt(LevelBeatenKey(levelNumber), 0) == 1;
    }

    // Load high score for a level (returns 0 if not set)
    public static float GetHighScore(int levelNumber)
    {
        return PlayerPrefs.GetFloat(LevelHighScoreKey(levelNumber), 0);
    }
}
