using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CellFactory
{
    private GameObject cellPrefab;
    private GameObject[] shapeSpritePrefabs;

    public CellFactory(GameObject prefab, GameObject[] shapes)
    {
        cellPrefab = prefab;
        shapeSpritePrefabs = shapes;
    }

    public GameObject CreateCell(Vector3 position, List<AnimationClip> usedAnimations, bool isCancer)
    {
        GameObject newCell = GameObject.Instantiate(cellPrefab, position, Quaternion.identity);
        RandomizeCell(newCell);
        newCell.GetComponent<CellAI>().SetCellBehavior(isCancer, usedAnimations.ToArray());
        return newCell;
    }

    public void RandomizeCell(GameObject cell)
    {
        cell.name = "Cell_" + Random.Range(1000, 9999);

        Color randomBorderColor = new Color(Random.value * 0.5f, Random.value * 0.5f, Random.value * 0.5f);
        Color randomCenterColor = Color.Lerp(randomBorderColor, Color.white, 0.8f);

        List<GameObject> remainingShapes = shapeSpritePrefabs.ToList();
        GameObject[] randomShapes = new GameObject[3];
        Color[] randomShapeColors = new Color[3];
        for (int i = 0; i < randomShapes.Length; i++)
        {
            int rand = Random.Range(0, remainingShapes.Count);
            randomShapes[i] = remainingShapes[rand];
            remainingShapes.RemoveAt(rand);

            randomShapeColors[i] = GetRandomColor();
        }

        cell.GetComponent<CellAI>().SetRandomizedCellParameters(randomBorderColor, randomCenterColor, randomShapes, randomShapeColors);
    }

    public GameObject ReplicateCell(GameObject parentCell, List<AnimationClip> usedAnimations)
    {
        GameObject childCell;

        CellAI parentCellAI = parentCell.GetComponent<CellAI>();
        CellSpecifications parentSpecifications = parentCellAI.GetSpecifications();
        CellSpecifications childSpecifications = ApplyRandomVariations(parentSpecifications);

        childCell = GameObject.Instantiate(cellPrefab, parentCell.transform.position + RandomizeReplicationOffset(), Quaternion.identity);
        childCell.GetComponent<CellAI>().SetRandomizedCellParameters(
            childSpecifications.borderColor,
            childSpecifications.centerColor,
            childSpecifications.randomShapes,
            childSpecifications.randomShapeColors
        );
        childCell.GetComponent<CellAI>().SetCellBehavior(parentCellAI.GetIsCancer(), usedAnimations.ToArray());

        childCell.name = "Cell_" + Random.Range(1000, 9999);

        return childCell;
    }

    private Vector3 RandomizeReplicationOffset()
    {
        //Generate a random angle in radians
        float randomAngle = Random.Range(0f, Mathf.PI * 2);

        //Calculate the x and y offsets based on the random angle
        float xOffset = 0.5f * Mathf.Cos(randomAngle);
        float yOffset = 0.5f * Mathf.Sin(randomAngle);

        return new Vector3 (xOffset, yOffset, 0);
    }

    private CellSpecifications ApplyRandomVariations(CellSpecifications parentSpecifications)
    {
        Color borderColor = parentSpecifications.borderColor;
        Color centerColor = parentSpecifications.centerColor;
        GameObject[] randomShapes = parentSpecifications.randomShapes.ToArray();
        Color[] randomShapeColors = parentSpecifications.randomShapeColors.ToArray();

        int numberOfVariations = Random.Range(2, 4);

        for (int i = 0; i < numberOfVariations; i++)
        {
            int variationType = Random.Range(0, 4);
            switch (variationType)
            {
                case 0:
                    int randomShapeIndex = Random.Range(0, randomShapeColors.Length);
                    randomShapeColors[randomShapeIndex] = SlightlyAlterColor(randomShapeColors[randomShapeIndex]);
                    break;

                case 1:
                    randomShapeIndex = Random.Range(0, randomShapeColors.Length);
                    randomShapeColors[randomShapeIndex] = GetRandomColor();
                    break;

                case 2:
                    borderColor = SlightlyAlterColor(borderColor);
                    centerColor = Color.Lerp(borderColor, Color.white, 0.8f);
                    break;

                case 3:
                    randomShapeIndex = Random.Range(0, randomShapes.Length);
                    randomShapes[randomShapeIndex] = GetRandomShape();
                    break;
            }
        }

        return new CellSpecifications(borderColor, centerColor, randomShapes, randomShapeColors);
    }

    private Color SlightlyAlterColor(Color originalColor)
    {
        float offset = 0.1f;
        return new Color(
            Mathf.Clamp01(originalColor.r + Random.Range(-offset, offset)),
            Mathf.Clamp01(originalColor.g + Random.Range(-offset, offset)),
            Mathf.Clamp01(originalColor.b + Random.Range(-offset, offset))
        );
    }

    private GameObject GetRandomShape()
    {
        return shapeSpritePrefabs[Random.Range(0, shapeSpritePrefabs.Length)];
    }

    private Color GetRandomColor()
    {
        return new Color(Random.value, Random.value, Random.value);
    }

    public Vector3 GetRandomPosition()
    {
        return new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), 0f);
    }
}
