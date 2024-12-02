using UnityEngine;
using UnityEngine.UI;

public class ButtonSoundPlayer : MonoBehaviour
{
    public AudioClip sound;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => AudioClipPlayer.PlayClipAtPoint(sound, Vector3.zero));
    }
}