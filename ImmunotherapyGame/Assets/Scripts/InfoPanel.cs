using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoPanel : MonoBehaviour
{
    TextMeshProUGUI timeAliveNumber, timeSinceReproducingNumber, proteinInfo;
    [SerializeField] private Button proteinAnalysisButton;

    private PlayerController playerController;
    private bool cancerCell;

    private void Awake()
    {
        timeAliveNumber = transform.Find("Lifespan Display").Find("Time Alive Number").GetComponent<TextMeshProUGUI>();
        timeSinceReproducingNumber = transform.Find("Reproduction Display").Find("Reproduction Number").GetComponent<TextMeshProUGUI>();
        proteinInfo = transform.Find("Protein Information").GetComponent<TextMeshProUGUI>();
    }

    public void UpdateInfo(CellAI cell, PlayerController player)
    {
        timeAliveNumber.text = FormatTime(cell.baseTimeAlive + cell.timeAlive);
        timeSinceReproducingNumber.text = FormatTime(cell.timeSinceReproducing);
        playerController = player;
        cancerCell = cell.GetIsCancer();
        proteinAnalysisButton.enabled = playerController.GetInventory().GetItemAmount(Inventory.ItemType.ProteinAnalyzer) > 0;
    }

    public void ClosePanel()
    {
        proteinInfo.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    public void ShowProteins()
    {
        if (!playerController.GetInventory().UseItem(Inventory.ItemType.ProteinAnalyzer))
            return;

        proteinAnalysisButton.enabled = false;
        proteinInfo.gameObject.SetActive(true);
        proteinInfo.text = "Protein Analysis:\n";

        if (cancerCell)
            proteinInfo.text += "High CD47";
        else
            proteinInfo.text += "High RNF20";

        // Check here for further implementation? https://jhoonline.biomedcentral.com/articles/10.1186/s13045-020-01013-x
        // The tutorial will also need to explain this more thoroughly
    }

    private string FormatTime(float seconds)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);

        if (timeSpan.TotalMinutes < 1)
            return $"{(int)timeSpan.TotalSeconds}s";
        else if (timeSpan.TotalHours < 1)
            return $"{(int)timeSpan.TotalMinutes}m {timeSpan.Seconds}s";
        else if (timeSpan.TotalDays < 1)
            return $"{(int)timeSpan.TotalHours}h {timeSpan.Minutes}m {timeSpan.Seconds}s";
        else
            return $"{(int)timeSpan.TotalDays}d {timeSpan.Hours}h {timeSpan.Minutes}m {timeSpan.Seconds}s";
    }
}
