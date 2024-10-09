using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoPanel : MonoBehaviour
{
    TextMeshProUGUI timeAliveNumber, timeSinceReproducingNumber;
    [SerializeField] private Button proteinAnalysisButton;

    private PlayerController playerController;

    private void Awake()
    {
        timeAliveNumber = transform.Find("Lifespan Display").Find("Time Alive Number").GetComponent<TextMeshProUGUI>();
        timeSinceReproducingNumber = transform.Find("Reproduction Display").Find("Reproduction Number").GetComponent<TextMeshProUGUI>();
    }

    public void UpdateInfo(CellAI cell, PlayerController player)
    {
        timeAliveNumber.text = Math.Round(cell.timeAlive, 1).ToString() + "s";
        timeSinceReproducingNumber.text = Math.Round(cell.timeSinceReproducing, 1).ToString() + "s";
        playerController = player;
        proteinAnalysisButton.enabled = playerController.GetInventory().GetItemAmount(Inventory.ItemType.ProteinAnalyzer) > 0;
    }

    public void ClosePanel()
    {
        gameObject.SetActive(false);
    }
}
