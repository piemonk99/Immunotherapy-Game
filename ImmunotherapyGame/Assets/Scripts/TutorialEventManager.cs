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
    public static event Action ShopOpened;
    public static event Action PurchasedVaccine;
    public static event Action UsedVaccine;
    public static event Action PurchasedProteinAnalyzer;
    public static event Action UsedProteinAnalyzer;
    public static event Action PurchasedCARTCell;
    public static event Action UsedCARTCell;

    public static event Action PromptNextItemUsage;


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

    public static void DoShopOpened()
    {
        ShopOpened?.Invoke();
    }

    public static void DoPurchasedVaccine()
    {
        PurchasedVaccine?.Invoke();
    }

    public static void DoUsedVaccine()
    {
        UsedVaccine?.Invoke();
    }

    public static void DoPurchasedProteinAnalyzer()
    {
        PurchasedProteinAnalyzer?.Invoke();
    }

    public static void DoUsedProteinAnalyzer()
    {
        UsedProteinAnalyzer?.Invoke();
    }

    public static void DoPurchasedCARTCell()
    {
        PurchasedCARTCell?.Invoke();
    }

    public static void DoUsedCARTCell()
    {
        UsedCARTCell?.Invoke();
    }

    

    public static void DoPromptNextItemUsage()
    {
        PromptNextItemUsage?.Invoke();
    }

    // Private constructor to enforce the singleton pattern
    private TutorialEventManager() { }
}
