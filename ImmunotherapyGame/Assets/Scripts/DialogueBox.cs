using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class DialogueBox : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        TutorialDriver.Instance.DialogueClicked();
    }
}
