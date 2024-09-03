using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;


//This script handles all button or other interactable events on the main menu scene
public class MainMenuManager : MonoBehaviour
{
    public void StartButtonClicked()
    {
        SceneManager.LoadScene("Level1Scene");
    }
}
