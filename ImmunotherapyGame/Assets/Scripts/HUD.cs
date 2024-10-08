using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [SerializeField] private Image pauseButtonImage;
    [SerializeField] private Sprite pauseSprite;
    [SerializeField] private Sprite resumeSprite;

    private bool paused;

    public void TogglePause()
    {
        paused = !paused;

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
}