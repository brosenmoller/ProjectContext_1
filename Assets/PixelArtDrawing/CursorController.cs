using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CursorController : MonoBehaviour
{
    public static Dictionary<Vector3, GameObject> occupiedLocations = new();

    [Header("Cursor Config")]
    [SerializeField] private float GridCellSize = 1;

    [Header("Factory Object Parent")]
    [SerializeField] private Transform parent;

    [Header("Selected Object")]
    [SerializeField] private Color[] selectableColors;
    [SerializeField] private GameObject placeableObject;

    [Header("Color Pallete")]
    [SerializeField] private RectTransform palleteLayoutGroup;
    [SerializeField] private GameObject colorPalleteColorImage;

    private Dictionary<int, Outline> colorPalleteOutlines = new();

    private int selectedColorIndex;
    private Vector3 mousePos;

    private void Awake()
    {
        for (int i = 0; i < selectableColors.Length; i++)
        {
            GameObject newColorImage = Instantiate(colorPalleteColorImage, palleteLayoutGroup);
            newColorImage.GetComponent<Image>().color = selectableColors[i];
            colorPalleteOutlines.Add(i, newColorImage.GetComponent<Outline>());
        }

        colorPalleteOutlines[0].enabled = true;
    }

    private void Update()
    {
        CursorMovement();
        ColorSelection();

        if (Input.GetMouseButton(0)) ObjectPlacement(selectableColors[selectedColorIndex]);
        if (Input.GetMouseButton(1)) ObjectRemovement();
    }

    private void CursorMovement()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos = SnapPositionToGrid(mousePos, GridCellSize);
        transform.position = mousePos;
    }

    private void ColorSelection()
    {
        if (Input.mouseScrollDelta.y == 0) { return; }

        colorPalleteOutlines[selectedColorIndex].enabled = false;

        if (Input.mouseScrollDelta.y > 0)
        {
            selectedColorIndex++;
            if (selectedColorIndex >= selectableColors.Length) { selectedColorIndex = 0; }
        }
        else if (Input.mouseScrollDelta.y < 0)
        {
            selectedColorIndex--;
            if (selectedColorIndex < 0) { selectedColorIndex = selectableColors.Length - 1; }
        }

        colorPalleteOutlines[selectedColorIndex].enabled = true;
    }

    private void ObjectPlacement(Color color)
    {
        Vector3 placementPosition = SnapPositionToGrid(transform.position, GridCellSize);

        if (occupiedLocations.ContainsKey(placementPosition))
        {
            ObjectRemovement();
        }

        GameObject newPixel = Instantiate(placeableObject, placementPosition, Quaternion.identity);
        newPixel.transform.parent = parent;
        occupiedLocations.Add(placementPosition, newPixel);

        newPixel.GetComponent<SpriteRenderer>().color = color;
        
    }

    private void ObjectRemovement()
    {
        Vector3 removePosition = SnapPositionToGrid(transform.position, GridCellSize);
        if (occupiedLocations.ContainsKey(removePosition))
        {
            GameObject pixelToBeRemoved = occupiedLocations[removePosition];
            occupiedLocations.Remove(removePosition);
            Destroy(pixelToBeRemoved);
        }
    }

    private Vector3 SnapPositionToGrid(Vector3 vector, float Cellsize)
    {
        vector.x = Mathf.Round(vector.x / Cellsize) * Cellsize;
        vector.y = Mathf.Round(vector.y / Cellsize) * Cellsize;
        vector.z = 0;
        return vector;
    }
}



