using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private GameObject tCellPrefab;
    [SerializeField] private GameObject currencyPrefab;
    [SerializeField] private int numberOfCells;
    [SerializeField] private int numberOfTCells;

    [SerializeField] private List<AnimationClip> allCellAnimations;
    private List<AnimationClip> usedAnimations;

    [SerializeField] private GameObject[] shapeSpritePrefabs;

    private CellFactory cellFactory;

    void Start()
    {
        usedAnimations = new List<AnimationClip>();
        for (int i = Mathf.Min(5, allCellAnimations.Count); i > 0; i--)
        {
            int rand = Random.Range(0, allCellAnimations.Count);
            usedAnimations.Add(allCellAnimations[rand]);
            allCellAnimations.RemoveAt(rand);
        }

        cellFactory = new CellFactory(cellPrefab, tCellPrefab, currencyPrefab, shapeSpritePrefabs);

        // Instantiate the cells
        for (int i = 0; i < numberOfCells; i++)
        {
            Vector3 randomPosition = cellFactory.GetRandomPosition();

            //Makes a few of the cells generated cancer cells
            bool isCancer = false;
            if (i < numberOfCells / 10) isCancer = true;
            
            GameObject newCell = cellFactory.CreateCell(randomPosition, usedAnimations, isCancer);
        }

        // Instantiate the T-cells
        for (int i = 0; i < numberOfTCells; i++)
        {
            Vector3 randomPosition = cellFactory.GetRandomPosition();
            cellFactory.CreateTCell(randomPosition);
        }
    }

    void Update()
    {
        // FIXME would be more efficient to only check when a cell dies or replicates
        int cancerCells = 0;

        foreach (CellAI cell in cellFactory.GetCells())
            if (cell.GetIsCancer())
                ++cancerCells;

        if (cancerCells == 0)
        { } // Win

        float percentageCancer = (float)cancerCells / cellFactory.GetCells().Count;

        if (percentageCancer >= 0.2f)
        { } // Lose
    }

    public void ReplicateCell(GameObject parentCell)
    {
        GameObject newCell = cellFactory.ReplicateCell(parentCell, usedAnimations);
    }
}
