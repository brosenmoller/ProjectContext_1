using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PixelBrushController : MonoBehaviour 
{
    public static Dictionary<Vector3, SpriteRenderer> occupiedLocations = new();
    public static Color[,] pixelGrid;

    [Header("General Settings")]
    [SerializeField] private float gridCellSize;
    [SerializeField] private Transform canvas;
    [SerializeField] private float canvasSize;

    [Header("Pixel Object Parent")]
    [SerializeField] private Transform pixelParent;

    [Header("Selected Object")]
    [SerializeField] private Color[] selectableColors;
    [SerializeField] private GameObject placeableObject;

    [Header("Color Pallete")]
    [SerializeField] private RectTransform palleteLayoutGroup;
    [SerializeField] private GameObject colorPalleteColorImage;

    private readonly Dictionary<Color, Outline> colorPalleteOutlines = new();

    private Color selectedColor;

    private float gridOffset = 0;

    private enum BrushMode { Painting, Erasing, None }
    private BrushMode currentBrushMode = BrushMode.None;

    private SpriteRenderer brushSpriteRender;

    private void Awake()
    {
        brushSpriteRender = GetComponent<SpriteRenderer>();
        occupiedLocations.Clear();

        pixelGrid = new Color[(int)canvasSize, (int)canvasSize];

        for (int x = 0; x < pixelGrid.GetLength(0); x++)
        {
            for (int y = 0; y < pixelGrid.GetLength(1); y++)
            {
                pixelGrid[x, y] = Color.clear;
            }
        }

        for (int i = 0; i < selectableColors.Length; i++)
        {
            GameObject colorPalleteButton = Instantiate(colorPalleteColorImage, palleteLayoutGroup);
            Image colorImage = colorPalleteButton.GetComponent<Image>();
            colorImage.color = selectableColors[i];
            colorPalleteOutlines.Add(colorImage.color, colorPalleteButton.GetComponent<Outline>());

            colorPalleteButton.GetComponent<Button>().onClick.AddListener(() => 
            {
                SetBrushColor(colorImage.color);
            });
        }

        SetBrushColor(selectableColors[0]);
        
        canvas.localScale = new Vector3(canvasSize / 2 - 0.5f, canvasSize / 2 - 0.5f, 1);
        //if (canvasSize % 2 == 0) { gridOffset = -.5f; }
    }

    public void ClearCanvas()
    {
        foreach (Transform child in pixelParent)
        {
            Destroy(child.gameObject);
        }

        occupiedLocations.Clear();
    }

    private void SetBrushColor(Color color)
    {
        colorPalleteOutlines[selectedColor].enabled = false;
        selectedColor = color;
        colorPalleteOutlines[selectedColor].enabled = true;
    }

    private void Start()
    {
        GameManager.InputManager.controls.Default.Paint.started += _ => currentBrushMode = BrushMode.Painting;
        GameManager.InputManager.controls.Default.Paint.canceled += _ => currentBrushMode = BrushMode.None;

        GameManager.InputManager.controls.Default.Erase.started += _ => currentBrushMode = BrushMode.Erasing;
        GameManager.InputManager.controls.Default.Erase.canceled += _ => currentBrushMode = BrushMode.None;

        GameManager.InputManager.controls.Default.CursorMovement.performed += 
            ctx => CursorMovement(ctx.ReadValue<Vector2>());
    }

    private void Update()
    {
        switch (currentBrushMode)
        {
            case BrushMode.Painting: PaintPixel(); break;
            case BrushMode.Erasing: ErasePixel(); break;
        }
    }

    private void CursorMovement(Vector2 mousePosition)
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(mousePosition);
        mousePos = SnapPositionToGrid(mousePos);
        transform.position = mousePos;

        if (IsPositionInCanvas(mousePos))
        {
            brushSpriteRender.enabled = true;
            Cursor.visible = false;
        }
        else
        {
            brushSpriteRender.enabled = false;
            Cursor.visible = true;
        }
    }

    private void PaintPixel()
    {
        Vector3 placementPosition = transform.position;
        if (!IsPositionInCanvas(placementPosition)) { return; }
        

        if (occupiedLocations.ContainsKey(placementPosition))
        {
            if (occupiedLocations[placementPosition].color == selectedColor) { return; }
            ErasePixel();
        }

        GameObject newPixel = Instantiate(placeableObject, placementPosition, Quaternion.identity);
        newPixel.transform.parent = pixelParent;

        pixelGrid[
            (int)(placementPosition.x - canvas.position.x + gridOffset + canvasSize / 2),
            (int)(placementPosition.y - canvas.position.y + gridOffset + canvasSize / 2)
        ] = selectedColor;

        SpriteRenderer pixelSpriteRenderer = newPixel.GetComponent<SpriteRenderer>();
        pixelSpriteRenderer.color = selectedColor;
        occupiedLocations.Add(placementPosition, pixelSpriteRenderer);
    }

    private void ErasePixel()
    {
        Vector3 removePosition = transform.position;
        if (!occupiedLocations.ContainsKey(removePosition)) { return; }
        
        pixelGrid[
            (int)(removePosition.x - canvas.position.x + gridOffset + canvasSize / 2),
            (int)(removePosition.y - canvas.position.y + gridOffset + canvasSize / 2)
        ] = Color.clear;

        GameObject pixelToBeRemoved = occupiedLocations[removePosition].gameObject;
        occupiedLocations.Remove(removePosition);
        Destroy(pixelToBeRemoved);
    }

    private Vector3 SnapPositionToGrid(Vector3 vector)
    {
        vector.x = Mathf.Round(vector.x / gridCellSize) * gridCellSize + gridOffset;
        vector.y = Mathf.Round(vector.y / gridCellSize) * gridCellSize + gridOffset;
        vector.z = 0;
        return vector;
    }
    private bool IsPositionInCanvas(Vector3 position)
    {
        Vector3 relativePosition = position - canvas.position;

        return relativePosition.x < canvasSize / 2 / 2 &&
               relativePosition.x > -canvasSize / 2 / 2 &&
               relativePosition.y < canvasSize / 2 / 2 &&
               relativePosition.y > -canvasSize / 2 / 2;
    }
}
