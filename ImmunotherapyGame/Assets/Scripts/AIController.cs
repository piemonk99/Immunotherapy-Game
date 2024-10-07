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

    // List to store all instantiated cells
    private List<GameObject> allCells;

    private void Start()
    {
        usedAnimations = new List<AnimationClip>();
        for (int i = Mathf.Min(5, allCellAnimations.Count); i > 0; i--)
        {
            int rand = Random.Range(0, allCellAnimations.Count);
            usedAnimations.Add(allCellAnimations[rand]);
            allCellAnimations.RemoveAt(rand);
        }

        cellFactory = new CellFactory(cellPrefab, tCellPrefab, currencyPrefab, shapeSpritePrefabs);
        allCells = new List<GameObject>(); // Initialize the cell list

        // Instantiate the cells
        for (int i = 0; i < numberOfCells; i++)
        {
            Vector3 randomPosition = cellFactory.GetRandomPosition();
            int cellType = (i < numberOfCells / 10) ? 1 : 0; // 1 for cancer, 0 for normal
            CreateCell(cellType, randomPosition);
        }

        // Instantiate the T-cells
        for (int i = 0; i < numberOfTCells; i++)
        {
            Vector3 randomPosition = cellFactory.GetRandomPosition();
            CreateCell(2, randomPosition); // 2 for T-cell
        }
    }

    private void Update()
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
        allCells.Add(newCell); // Add the replicated cell to the list
    }

    // Creates a new cell. cellType 0 is normal, 1 is cancer, 2 is T-cell
    public void CreateCell(int cellType, Vector3 position)
    {
        GameObject newCell;

        switch (cellType)
        {
            case 0: // Normal cell
                newCell = cellFactory.CreateCell(position, usedAnimations, false); // Normal cell, not cancer
                break;
            case 1: // Cancer cell
                newCell = cellFactory.CreateCell(position, usedAnimations, true); // Cancer cell
                break;
            case 2: // T-cell
                newCell = cellFactory.CreateTCell(position);
                break;
            default:
                return; // Invalid cell type
        }

        allCells.Add(newCell); // Add the new cell to the list
    }

    // Destroys all cells in the list
    public void DestroyAllCells()
    {
        foreach (GameObject cell in allCells)
        {
            CellAI cellAI = cell.GetComponent<CellAI>();
            if (cellAI != null) // Check if the component is not null
            {
                cellAI.DestroyCell(); // Destroy the cell GameObject
            }
        }

    }
}
