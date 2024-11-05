using UnityEngine;

public class GameProgressManager : MonoBehaviour
{
    public int currentLevelNumber = 1; // Set the level number for this scene
    private float levelCompletionTime = 0f;

    private void OnEnable()
    {
        EventManager.Win += OnWin;
        EventManager.Lose += OnLose;
    }

    private void OnDisable()
    {
        EventManager.Win -= OnWin;
        EventManager.Lose -= OnLose;
    }

    private void Update()
    {
        // Update level completion time, used only upon win
        levelCompletionTime += Time.deltaTime;
    }

    private void OnWin()
    {
        // Save the beaten status and high score upon win
        SaveSystem.SaveLevelBeaten(currentLevelNumber);
        SaveSystem.SaveHighScore(currentLevelNumber, levelCompletionTime);

        // Reset time for consistency on reloads
        levelCompletionTime = 0;
    }

    private void OnLose()
    {
        // Handle actions upon losing, if we need
    }
}
