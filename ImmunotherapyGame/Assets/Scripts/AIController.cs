using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public GameObject cellPrefab;           // Assign your Cell prefab in the inspector
    public int numberOfCells = 10;          // Number of cells to instantiate
    private List<GameObject> cells = new List<GameObject>();

    [SerializeField] private List<AnimationClip> allCellAnimations;
    private List<AnimationClip> usedAnimations;

    void Start()
    {
        usedAnimations = new List<AnimationClip>();
        for (int i = Mathf.Min(5, allCellAnimations.Count); i > 0; i--)
        {
            int rand = Random.Range(0, allCellAnimations.Count);
            usedAnimations.Add(allCellAnimations[rand]);
            allCellAnimations.RemoveAt(rand);
        }

        // Instantiate the cells
        for (int i = 0; i < numberOfCells; i++)
        {
            GameObject newCell = Instantiate(cellPrefab, GetRandomPosition(), Quaternion.identity);
            cells.Add(newCell);
            RandomizeCell(newCell);
            newCell.GetComponent<CellAI>().SetCellAnimations(usedAnimations.ToArray());
            StartCellAI(newCell);
        }
    }

    // Assign random colors and uniqueness to each cell
    private void RandomizeCell(GameObject cell)
    {
        // Get references to the Border and Center
        Transform border = cell.transform.Find("Border");
        Transform center = cell.transform.Find("Center");
        Transform square = center.Find("Square");
        Transform diamond = center.Find("Diamond");
        Transform hexagon = center.Find("Hexagon");

        // Generate a moderately dark random color for the border
        Color randomBorderColor = new Color(Random.value * 0.5f, Random.value * 0.5f, Random.value * 0.5f);

        // Create a lighter version of the border color for the center
        Color randomCenterColor = Color.Lerp(randomBorderColor, Color.white, 0.8f);

        // Assign colors to the Border and Center
        border.GetComponent<SpriteRenderer>().color = randomBorderColor;
        center.GetComponent<SpriteRenderer>().color = randomCenterColor;

        // Random colors for each of the shapes in the center
        square.GetComponent<SpriteRenderer>().color = GetRandomColor();
        diamond.GetComponent<SpriteRenderer>().color = GetRandomColor();
        hexagon.GetComponent<SpriteRenderer>().color = GetRandomColor();

        // You can also assign a unique ID to each cell for identification if needed
        cell.name = "Cell_" + Random.Range(1000, 9999); // Assign a unique name
    }

    // Generates a random color
    private Color GetRandomColor()
    {
        return new Color(Random.value, Random.value, Random.value);
    }

    // Set a random position for each cell (you can customize the range)
    private Vector3 GetRandomPosition()
    {
        return new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), 0f);
    }

    // Start the CellAI and call DecideActivity every 3-8 seconds
    private void StartCellAI(GameObject cell)
    {
        CellAI cellAI = cell.GetComponent<CellAI>();
        if (cellAI != null)
        {
            StartCoroutine(DecideActivityRoutine(cellAI));
        }
    }

    // Coroutine to call DecideActivity every 3-8 seconds
    private IEnumerator DecideActivityRoutine(CellAI cellAI)
    {
        while (true)
        {
            float delay = Random.Range(3f, 8f); // Random delay between 3-8 seconds
            yield return new WaitForSeconds(delay);
            cellAI.DecideActivity();
        }
    }
}
