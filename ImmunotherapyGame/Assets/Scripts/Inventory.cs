using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private AIController aiController;
    [SerializeField] private GameObject InventoryPanel;
    [SerializeField] private TextMeshProUGUI vaccineNumber;
    [SerializeField] private TextMeshProUGUI proteinAnalyzerNumber;
    [SerializeField] private TextMeshProUGUI CARTCellNumber;

    private Transform player;

    private int tutorialStage;

    // Dictionary to hold the number of each item
    private Dictionary<ItemType, int> itemInventory = new Dictionary<ItemType, int>();

    private void Awake()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "TutorialScene") { tutorialStage = 0; }
        else { tutorialStage = -1; }

        player = transform;
    }

    // Enumeration of the different item types in the game
    public enum ItemType
    {
        Vaccine,
        ProteinAnalyzer,
        CAR_T_Cell
    }

    // Initialize the inventory
    private void Start()
    {
        // Initialize all item counts to a starting value
        itemInventory[ItemType.Vaccine] = 0;
        itemInventory[ItemType.ProteinAnalyzer] = 0;
        itemInventory[ItemType.CAR_T_Cell] = 0;

        UpdateUI(); // Update the UI initially with these starting values
    }

    // Function to add items to the inventory
    public void AddItem(ItemType itemType, int amount = 1)
    {
        if (itemInventory.ContainsKey(itemType))
        {
            itemInventory[itemType] += amount;
            UpdateUI(); // Update the UI after adding items
        }
    }

    // Wrapper functions for UseItem
    public void OnVaccineButtonClick() { UseItem(ItemType.Vaccine); }
    public void OnProteinAnalyzerButtonClick() { UseItem(ItemType.ProteinAnalyzer); }
    public void OnCARTCellButtonClick() { UseItem(ItemType.CAR_T_Cell); }


    // Function to remove an item from the inventory when used
    private bool UseItem(ItemType itemType)
    {
        if (!itemInventory.ContainsKey(itemType) || itemInventory[itemType] <= 0)
        {
            Debug.Log($"Usage of item {itemType} failed; no items available.");
            return false; // Failed usage (no items available)
        }
        else if (tutorialStage != -1 && ((itemType == ItemType.Vaccine && tutorialStage != 0) || (itemType == ItemType.ProteinAnalyzer && tutorialStage != 1) || (itemType == ItemType.CAR_T_Cell && tutorialStage != 2)))
        {
            Debug.Log($"Usage of item {itemType} failed; incorrect tutorial stage.");
            return false; // Failed usage (in wrong stage of tutorial)
        }

        switch (itemType)
        {
            case ItemType.Vaccine:
                UseVaccine();
                break;
            case ItemType.ProteinAnalyzer:
                UseProteinAnalyzer();
                break;
            case ItemType.CAR_T_Cell:
                UseCAR_T_Cell();
                break;
        }

        itemInventory[itemType]--;
        UpdateUI(); // Update the UI after using an item

        return true; // Successful usage
    }

    // Individual functions to use specific items
    private void UseVaccine()
    {
        if (tutorialStage != -1) { TutorialEventManager.DoUsedVaccine(); tutorialStage = 1; }

        aiController.CreateCell(1, player.position + Vector3.up).GetComponent<CellAI>().SetDummy(true);
    }

    private void UseProteinAnalyzer()
    {
        if (tutorialStage != -1) { TutorialEventManager.DoUsedProteinAnalyzer(); tutorialStage = 2; }
    }

    private void UseCAR_T_Cell()
    {
        if (tutorialStage != -1) { TutorialEventManager.DoUsedCARTCell(); tutorialStage = -1; }

        aiController.CreateCell(2, player.position + Vector3.up).GetComponent<TCellAI>().SetDetectCancer(true);
    }

    // Get the number of a specific item type in the inventory
    public int GetItemAmount(ItemType itemType)
    {
        if (itemInventory.ContainsKey(itemType))
        {
            return itemInventory[itemType];
        }

        return 0; // Item doesn't exist in the inventory
    }

    // Update the UI text elements with the current inventory counts
    private void UpdateUI()
    {
        vaccineNumber.text = itemInventory[ItemType.Vaccine].ToString();
        proteinAnalyzerNumber.text = itemInventory[ItemType.ProteinAnalyzer].ToString();
        CARTCellNumber.text = itemInventory[ItemType.CAR_T_Cell].ToString();
    }

    // Enable the inventory panel and update UI when enabled
    public void EnableInventoryPanel()
    {
        InventoryPanel.SetActive(true);
        UpdateUI(); // Make sure the UI is up to date when opening the inventory panel
    }

    public void DisableInventoryPanel()
    {
        InventoryPanel.SetActive(false);
    }

    // Debug: Print the inventory contents to the console
    public void PrintInventory()
    {
        foreach (var item in itemInventory)
        {
            Debug.Log(item.Key + ": " + item.Value);
        }
    }
}
