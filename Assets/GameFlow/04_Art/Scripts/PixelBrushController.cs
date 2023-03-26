using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PixelBrushController : MonoBehaviour 
{
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

    private readonly Dictionary<Vector3, SpriteRenderer> occupiedLocations = new();
    private Color[,] currentPixelGrid;

    private Color selectedColor;

    private float gridOffset = 0;

    private enum BrushMode { Painting, Erasing, None }
    private BrushMode currentBrushMode = BrushMode.None;

    private SpriteRenderer brushSpriteRender;

    private Outline currentSelectionOutline;

    [HideInInspector] public bool canDraw = false;

    private void Awake()
    {
        brushSpriteRender = GetComponent<SpriteRenderer>();

        selectedColor = selectableColors[0];
        SetBrushColor(selectableColors[0]);


        RectTransform startingColor = palleteLayoutGroup.GetChild(0) as RectTransform;
        SetBrushColor(startingColor.GetComponent<Image>().color);

        currentSelectionOutline = startingColor.GetComponent<Outline>();
        currentSelectionOutline.enabled = true;

        foreach (RectTransform child in palleteLayoutGroup)
        {
            child.GetComponent<Button>().onClick.AddListener(() =>
            {
                SetBrushColor(child.GetComponent<Image>().color);

                if (currentSelectionOutline != null) { currentSelectionOutline.enabled = false; }
                currentSelectionOutline = child.GetComponent<Outline>();
                currentSelectionOutline.enabled = true;
            });
        }
    }

    private void Start()
    {
        GameManager.InputManager.controls.Default.Paint.started += SetBrushModePainting;
        GameManager.InputManager.controls.Default.Paint.canceled += SetBrushModeNone;

        GameManager.InputManager.controls.Default.Erase.started += SetBrushModeErasing;
        GameManager.InputManager.controls.Default.Erase.canceled += SetBrushModeNone;

        GameManager.InputManager.controls.Default.CursorMovement.performed += CursorMovement;
    }

    public void SetPixelGrid(Color[,] newPixelGrid)
    {
        foreach (Transform child in pixelParent)
        {
            Destroy(child.gameObject);
        }

        occupiedLocations.Clear();

        currentPixelGrid = newPixelGrid;
        for (int x = 0; x < newPixelGrid.GetLength(0); x++)
        {
            for (int y = 0; y < newPixelGrid.GetLength(1); y++)
            {
                if (newPixelGrid[x, y] == Color.clear) { continue; }

                Vector3 placementPosition = new
                (
                    (float)(.5f * x + .5f),
                    (float)(.5f * y - 3.5f),
                    0
                );

                InstantiatePixel(placementPosition, newPixelGrid[x, y]);
            }
        }

    }

    public void ClearCanvas()
    {
        foreach (Transform child in pixelParent)
        {
            Destroy(child.gameObject);
        }

        occupiedLocations.Clear();

        for (int x = 0; x < currentPixelGrid.GetLength(0); x++)
        {
            for (int y = 0; y < currentPixelGrid.GetLength(1); y++)
            {
                currentPixelGrid[x, y] = Color.clear;
            }
        }
    }

    public void SetBrushColor(Color color)
    {
        selectedColor = color;
    }

    private void OnDisable()
    {
        GameManager.InputManager.controls.Default.Paint.started -= SetBrushModePainting;
        GameManager.InputManager.controls.Default.Paint.canceled -= SetBrushModeNone;

        GameManager.InputManager.controls.Default.Erase.started -= SetBrushModeErasing;
        GameManager.InputManager.controls.Default.Erase.canceled -= SetBrushModeNone;

        GameManager.InputManager.controls.Default.CursorMovement.performed -= CursorMovement;
    }


    private void SetBrushModePainting(InputAction.CallbackContext callbackContext) => currentBrushMode = BrushMode.Painting;
    private void SetBrushModeNone(InputAction.CallbackContext callbackContext) => currentBrushMode = BrushMode.None;
    private void SetBrushModeErasing(InputAction.CallbackContext callbackContext) => currentBrushMode = BrushMode.Erasing;


    private void Update()
    {
        switch (currentBrushMode)
        {
            case BrushMode.Painting: PaintPixel(); break;
            case BrushMode.Erasing: ErasePixel(); break;
        }
    }

    private void CursorMovement(InputAction.CallbackContext callbackContext)
    {
        if (!canDraw) {  return; }

        Vector2 mousePosition = callbackContext.ReadValue<Vector2>();
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
        if (!canDraw) { return; }

        Vector3 placementPosition = transform.position;
        if (!IsPositionInCanvas(placementPosition)) { return; }
        

        if (occupiedLocations.ContainsKey(placementPosition))
        {
            if (occupiedLocations[placementPosition].color == selectedColor) { return; }
            ErasePixel();
        }

        currentPixelGrid[
            (int)(2 * placementPosition.x - 1),
            (int)(2 * placementPosition.y + 7)
        ] = selectedColor;

        InstantiatePixel(placementPosition, selectedColor);
    }

    private void InstantiatePixel(Vector3 placementPosition, Color color)
    {
        GameObject newPixel = Instantiate(placeableObject, placementPosition, Quaternion.identity);
        newPixel.transform.parent = pixelParent;

        SpriteRenderer pixelSpriteRenderer = newPixel.GetComponent<SpriteRenderer>();
        pixelSpriteRenderer.color = color;
        occupiedLocations.Add(placementPosition, pixelSpriteRenderer);
    }

    private void ErasePixel()
    {
        if (!canDraw) { return; }

        Vector3 removePosition = transform.position;
        if (!occupiedLocations.ContainsKey(removePosition)) { return; }
        
        currentPixelGrid[
            (int)(2 * removePosition.x - 1),
            (int)(2 * removePosition.y + 7)
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
