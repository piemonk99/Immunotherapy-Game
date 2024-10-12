using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private AIController aiController;

    private Vector2 shopPosition;

    private int tutorialStage;

    private void Awake()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "TutorialScene") { tutorialStage = 0; }
        else { tutorialStage = -1; }
    }

    private void OnEnable()
    {
        if (tutorialStage != -1) { TutorialEventManager.DoShopOpened(); }
    }

    public void PurchaseVaccine()
    {
        if (playerController.GetCurrency() < 3 || (tutorialStage != 0 && tutorialStage != -1))
            return;

        playerController.GetInventory().AddItem(Inventory.ItemType.Vaccine);
        playerController.SetCurrency(playerController.GetCurrency() - 3);

        if (tutorialStage == 0)
        {
            TutorialEventManager.DoPurchasedVaccine();
            tutorialStage = 1;
        }
    }

    public void PurchaseProteinAnalyzer()
    {
        if (playerController.GetCurrency() < 2 || (tutorialStage != 1 && tutorialStage != -1))
            return;

        playerController.GetInventory().AddItem(Inventory.ItemType.ProteinAnalyzer);
        playerController.SetCurrency(playerController.GetCurrency() - 2);

        if (tutorialStage == 1)
        {
            TutorialEventManager.DoPurchasedProteinAnalyzer();
            tutorialStage = 2;
        }
    }

    public void PurchaseCARTCell()
    {
        if (playerController.GetCurrency() < 6 || (tutorialStage != 2 && tutorialStage != -1))
            return;

        playerController.GetInventory().AddItem(Inventory.ItemType.CAR_T_Cell);
        playerController.SetCurrency(playerController.GetCurrency() - 6);

        if (tutorialStage == 2)
        {
            TutorialEventManager.DoPurchasedCARTCell();
            tutorialStage = -1;
        }
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
