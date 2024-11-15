using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private Transform canvas;
    [SerializeField] private Transform[] levelSelectors;
    [SerializeField] private TextMeshProUGUI StartContinueText;

    private Transform mainMenu;
    private Transform levelSelectMenu;

    private int farthestBeatenLevel;

    private void Start()
    {
        mainMenu = canvas.Find("MainMenu");
        levelSelectMenu = canvas.Find("LevelSelectMenu");

        mainMenu.gameObject.SetActive(true);
        levelSelectMenu.gameObject.SetActive(false);

        // Load saved data to check which levels are accessible
        UpdateLevelAccess();
    }

    public void LevelSelectButtonClicked()
    {
        mainMenu.gameObject.SetActive(false);
        levelSelectMenu.gameObject.SetActive(true);
    }
    public void ReturnButtonClicked()
    {
        levelSelectMenu.gameObject.SetActive(false);
        mainMenu.gameObject.SetActive(true);
    }

    public void StartOrContinueButtonClicked()
    {
        if (farthestBeatenLevel != 3)
        {
            SceneManager.LoadScene($"Level{farthestBeatenLevel + 1}Scene");
        }
        else
        {
            SceneManager.LoadScene("Level3Scene");
        }
    }
    public void TutorialButtonClicked()
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

    private void FindFarthestBeatenLevel()
    {
        farthestBeatenLevel = 0;
        for (int i = 1; i <= 3; i++)
        {
            if (SaveSystem.IsLevelBeaten(i)) { farthestBeatenLevel++; }
            else { break; }
        }
    }

    private void UpdateLevelAccess()
    {
        FindFarthestBeatenLevel();

        // Sets text of start/continue button appropriately
        if (farthestBeatenLevel == 0) StartContinueText.text = "Start";
        else { StartContinueText.text = "Continue"; }

        for (int i = 0; i < levelSelectors.Length; i++)
        {
            bool isUnlocked = i <= farthestBeatenLevel;

            // Set interactability and grey-out panel
            levelSelectors[i].Find("LevelButton").GetComponent<Button>().interactable = isUnlocked;
            levelSelectors[i].Find("GreyOutPanel").gameObject.SetActive(!isUnlocked);

            // Update completion widget and time text if the level is completed
            bool levelBeaten = i <= farthestBeatenLevel && SaveSystem.IsLevelBeaten(i + 1);
            levelSelectors[i].Find("CompletionWidget").gameObject.SetActive(levelBeaten);
            levelSelectors[i].Find("CompletionWidget/TimeText").GetComponent<TextMeshProUGUI>().text =
                BestTimeToString(SaveSystem.GetHighScore(i + 1));
        }
    }

    // Converts a float time in seconds to an organized string in minutes and seconds
    private string BestTimeToString(float time)
    {
        string minutesString = "";
        if (Mathf.RoundToInt(time / 60) > 0)
        {
            string minutesPlural = "";
            if (Mathf.RoundToInt(time / 60) != 1)
            {
                minutesPlural = "s";
            }

            minutesString = $"{Mathf.RoundToInt(time / 60)} minute{minutesPlural}";
        }

        string secondsString = "";
        if (Math.Round(time % 60, 1) > 0)
        {
            string secondsPlural = "";
            if (Math.Round(time % 60, 1) != 1)
            {
                secondsPlural = "s";
            }

            secondsString = $"{Math.Round(time % 60, 1)} second{secondsPlural}";
        }

        string finalText = "Time: ";

        finalText += minutesString;
        if (!minutesString.Equals("") && !secondsString.Equals("")) finalText += " and ";
        finalText += secondsString;

        return finalText;
    }
}
