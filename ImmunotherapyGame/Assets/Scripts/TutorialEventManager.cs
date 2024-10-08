using System;
using UnityEngine;

public class TutorialEventManager : MonoBehaviour
{
    private static TutorialEventManager _instance;
    public static TutorialEventManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new TutorialEventManager();
            }
            return _instance;
        }
    }

    // Static event Actions
    public static event Action PlayerMoved;
    public static event Action BindingActivated;
    public static event Action CellBound;
    public static event Action CellDestroyed;
    public static event Action PlayerPickedSample;
    

    // Static methods to invoke the actions
    public static void DoPlayerMoved()
    {
        PlayerMoved?.Invoke();
    }

    public static void DoBindingActivated()
    {
        BindingActivated?.Invoke();
    }

    public static void DoCellBound()
    {
        CellBound?.Invoke();
    }

    public static void DoCellDestroyed()
    {
        CellDestroyed?.Invoke();
    }

    public static void DoPlayerPickedSample()
    {
        PlayerPickedSample?.Invoke();
    }

    

    // Private constructor to enforce the singleton pattern
    private TutorialEventManager() { }
}
