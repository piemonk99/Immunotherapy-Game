using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InfoPanel : MonoBehaviour
{
    TextMeshProUGUI timeAliveNumber, timeSinceReproducingNumber;
    private void Awake()
    {
        timeAliveNumber = transform.Find("Lifespan Display").Find("Time Alive Number").GetComponent<TextMeshProUGUI>();
        timeSinceReproducingNumber = transform.Find("Reproduction Display").Find("Reproduction Number").GetComponent<TextMeshProUGUI>();
    }

    public void UpdateInfo(CellAI cell)
    {
        timeAliveNumber.text = Math.Round(cell.timeAlive, 1).ToString() + "s";
        timeSinceReproducingNumber.text = Math.Round(cell.timeSinceReproducing, 1).ToString() + "s";
    }
    public void ClosePanel()
    {
        gameObject.SetActive(false);
    }
}
