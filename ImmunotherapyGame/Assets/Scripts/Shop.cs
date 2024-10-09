using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private AIController aiController;

    private Vector2 shopPosition;

    public void PurchaseVaccine()
    {
        if (playerController.GetCurrency() < 3)
            return;

        aiController.CreateCell(1, shopPosition).GetComponent<CellAI>().SetDummy(true);
        playerController.SetCurrency(playerController.GetCurrency() - 3);
    }

    public void PurchaseProteinAnalyzer()
    {
        if (playerController.GetCurrency() < 2)
            return;

        playerController.GetInventory().AddItem(Inventory.ItemType.ProteinAnalyzer);
        playerController.SetCurrency(playerController.GetCurrency() - 2);
    }

    public void PurchaseCARTCell()
    {
        if (playerController.GetCurrency() < 6)
            return;

        aiController.CreateCell(2, shopPosition).GetComponent<TCellAI>().SetDetectCancer(true);
        playerController.SetCurrency(playerController.GetCurrency() - 6);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void SetShopPosition(Vector2 position)
    {
        shopPosition = position;
    }
}
