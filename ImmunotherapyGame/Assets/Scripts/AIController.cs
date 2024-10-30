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

    [SerializeField] private PlayerController playerController;

    [SerializeField] private Transform pathfindingNodeContainer;

    private void Start()
    {
        usedAnimations = new List<AnimationClip>();
        for (int i = Mathf.Min(5, allCellAnimations.Count); i > 0; i--)
        {
            int rand = Random.Range(0, allCellAnimations.Count);
            usedAnimations.Add(allCellAnimations[rand]);
            allCellAnimations.RemoveAt(rand);
        }

        PathfindingNode[] pathfindingNodes = new PathfindingNode[pathfindingNodeContainer.childCount];

        for (int i = 0; i < pathfindingNodeContainer.childCount; ++i)
            pathfindingNodes[i] = pathfindingNodeContainer.GetChild(i).GetComponent<PathfindingNode>();

        cellFactory = new CellFactory(cellPrefab, tCellPrefab, currencyPrefab, shapeSpritePrefabs, playerController, pathfindingNodes);
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

        EventManager.CheckWinLoss += CheckWinLoss;
    }

    private void CheckWinLoss()
    {
        int cancerCells = 0;

        foreach (CellAI cell in cellFactory.GetCells())
            if (cell.GetIsCancer())
                ++cancerCells;

        if (cancerCells == 0) 
            EventManager.DoWin(); // Win

        float percentageCancer = (float)cancerCells / cellFactory.GetCells().Count;

        if (percentageCancer >= 0.2f) 
            EventManager.DoLose(); // Lose
    }

    public void ReplicateCell(GameObject parentCell)
    {
        GameObject newCell = cellFactory.ReplicateCell(parentCell, usedAnimations);
        allCells.Add(newCell); // Add the replicated cell to the list
    }

    // Creates a new cell. cellType 0 is normal, 1 is cancer, 2 is T-cell
    public GameObject CreateCell(int cellType, Vector3 position)
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
                return null; // Invalid cell type
        }

        allCells.Add(newCell); // Add the new cell to the list
        return newCell;
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

    public CellFactory GetCellFactory()
    {
        return cellFactory;
    }
}
