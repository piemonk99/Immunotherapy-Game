using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private Button level1Button;
    [SerializeField] private Button level2Button;
    [SerializeField] private Button level3Button;

    private void Start()
    {
        // Load saved data to check which levels are accessible
        UpdateLevelAccess();

        ///Debug.Log($"Level 1 beaten? {SaveSystem.IsLevelBeaten(1)} | Time: {SaveSystem.GetHighScore(1)} seconds");
    }

    public void StartButtonClicked()
    {
        SceneManager.LoadScene("TutorialScene");
    }

    public void Level1ButtonClicked()
    {
        SceneManager.LoadScene("Level1Scene");
    }

    public void Level2ButtonClicked()
    {
        if (SaveSystem.IsLevelBeaten(1)) // Only allow access if Level 1 is beaten
        {
            SceneManager.LoadScene("Level2Scene");
        }
    }

    public void Level3ButtonClicked()
    {
        if (SaveSystem.IsLevelBeaten(2)) // Only allow access if Level 1 is beaten
        {
            SceneManager.LoadScene("Level3Scene");
        }
    }

    public void QuitButtonClicked()
    {
        Application.Quit();
    }

    private void UpdateLevelAccess()
    {
        // Enable level buttons based on progress
        level1Button.interactable = true; // Level 1 is always available
        //level2Button.interactable = SaveSystem.IsLevelBeaten(1);
        //level3Button.interactable = SaveSystem.IsLevelBeaten(2);
    }
}
