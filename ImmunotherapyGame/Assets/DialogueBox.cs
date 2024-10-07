using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class DialogueBox : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        if (TutorialDriver.Instance.CanClickToAdvance())
        {
            // Call the AdvanceDialogue function in the TutorialDriver
            TutorialDriver.Instance.AdvanceDialogue();
        }
    }
}
