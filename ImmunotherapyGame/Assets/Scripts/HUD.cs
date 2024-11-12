using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [SerializeField] private Image pauseButtonImage;
    [SerializeField] private Sprite pauseSprite;
    [SerializeField] private Sprite resumeSprite;

    [SerializeField] private GameObject pauseMenu;

    private bool paused;

    private void Awake()
    {
        EventManager.Win += Win;
        EventManager.Lose += Lose;
    }

    public void TogglePause()
    {
        paused = !paused;
        pauseMenu.SetActive(paused);

        if (paused)
        {
            pauseButtonImage.overrideSprite = resumeSprite;
            Time.timeScale = 0;
        }
        else
        {
            pauseButtonImage.overrideSprite = pauseSprite;
            Time.timeScale = 1;
        }
    }

    public void Win()
    {
        // Find the immediate child named "WinScreen"
        Transform winScreen = GetImmediateChildByName(transform, "WinScreen");
        if (winScreen != null)
        {
            winScreen.gameObject.SetActive(true);
        }

        // Pause the game
        Time.timeScale = 0;
    }

    public void Lose()
    {
        // Find the immediate child named "LoseScreen"
        Transform loseScreen = GetImmediateChildByName(transform, "LoseScreen");
        if (loseScreen != null)
        {
            loseScreen.gameObject.SetActive(true);
        }

        // Pause the game
        Time.timeScale = 0;
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");

        Time.timeScale = 1;
    }
    public void ReloadLevel()
    {
        // Get the active scene and reload it
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);

        Time.timeScale = 1;
    }
    public void LoadNextLevel()
    {

    }

    private Transform GetImmediateChildByName(Transform parent, string name)
    {
        // Iterate through all immediate children of the parent transform
        foreach (Transform child in parent)
        {
            if (child.name == name)
            {
                return child;
            }
        }
        return null;
    }

}