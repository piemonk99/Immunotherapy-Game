using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    private static EventManager _instance;
    public static EventManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new EventManager();
            }
            return _instance;
        }
    }

    public static event Action CheckWinLoss;
    public static event Action Win;
    public static event Action Lose;

    public static void DoCheckWinLoss()
    {
        if (SceneManager.GetActiveScene().name != "TutorialScene")
        {
            CheckWinLoss?.Invoke();
        }
    }
    public static void DoWin()
    {
        Win?.Invoke();
    }
    public static void DoLose()
    {
        Lose?.Invoke();
    }

    // Private constructor to enforce the singleton pattern
    private EventManager() { }
}
