using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private int numberOfCells;
    private List<GameObject> cells = new List<GameObject>();

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

        cellFactory = new CellFactory(cellPrefab, shapeSpritePrefabs);

        //Instantiate the cells
        for (int i = 0; i < numberOfCells; i++)
        {
            Vector3 randomPosition = cellFactory.GetRandomPosition();

            //Makes a few of the cells generated cancer cells
            bool isCancer = false;
            if (i < numberOfCells / 5) isCancer = true;
            
            GameObject newCell = cellFactory.CreateCell(randomPosition, usedAnimations, isCancer);
            cells.Add(newCell);
            StartCellAI(newCell);
        }
    }

    public void ReplicateCell(GameObject parentCell)
    {
        GameObject newCell = cellFactory.ReplicateCell(parentCell, usedAnimations);
        cells.Add(newCell);
        StartCellAI(newCell);
    }

    private void StartCellAI(GameObject cell)
    {
        CellAI cellAI = cell.GetComponent<CellAI>();
        if (cellAI != null)
        {
            StartCoroutine(DecideActivityRoutine(cellAI));
        }
    }

    private IEnumerator DecideActivityRoutine(CellAI cellAI)
    {
        while (true)
        {
            float delay = Random.Range(3f, 8f);
            yield return new WaitForSeconds(delay);
            cellAI.DecideActivity();
        }
    }
}
