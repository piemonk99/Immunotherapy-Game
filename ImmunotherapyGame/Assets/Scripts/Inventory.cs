using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI vaccineNumber;
    [SerializeField] private TextMeshProUGUI proteinAnalyzerNumber;
    [SerializeField] private TextMeshProUGUI CARTCellNumber;

    // Enumeration of the different item types in the game
    public enum ItemType
    {
        Vaccine,
        ProteinAnalyzer,
        CAR_T_Cell
    }

    // Dictionary to hold the number of each item
    private Dictionary<ItemType, int> itemInventory = new Dictionary<ItemType, int>();

    // Initialize the inventory
    private void Awake()
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

    // Function to remove an item from the inventory when used
    public bool UseItem(ItemType itemType)
    {
        if (itemInventory.ContainsKey(itemType) && itemInventory[itemType] > 0)
        {
            itemInventory[itemType]--;
            UpdateUI(); // Update the UI after using an item
            return true; // Successful usage
        }

        return false; // Failed usage (no items available)
    }

    // Individual functions to use specific items
    public void UseVaccine()
    {
        // UseItem(ItemType.Vaccine);
    }

    public void UseProteinAnalyzer()
    {
        // UseItem(ItemType.ProteinAnalyzer);
    }

    public void UseCAR_T_Cell()
    {
        // UseItem(ItemType.CAR_T_Cell);
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
        gameObject.SetActive(true);
        UpdateUI(); // Make sure the UI is up to date when opening the inventory panel
    }

    public void DisableInventoryPanel()
    {
        gameObject.SetActive(false);
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
