using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileBrushController : MonoBehaviour 
{
    public static Dictionary<Vector3Int, GridCellContent> occupiedLocations = new();

    [Header("Tilemap & Grid")]
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Grid grid;

    [Header("Tiles")]
    [SerializeField] private TileBase groundTileVariation1;
    [SerializeField] private TileBase groundTileVariation2;

    [Header("References")]
    [SerializeField] private SpriteRenderer brushSpriteRender;
    [SerializeField] private DesignController designController;

    private GridCellContent selectedCellContent;

    private enum BrushMode { Painting, Erasing, None }
    private BrushMode currentBrushMode = BrushMode.None;

    private Dictionary<GridCellContent, TileBase> cellContentToTileBase;


    private void Awake()
    {
        occupiedLocations.Clear();
        selectedCellContent = GridCellContent.GroundTileVariation1;

        cellContentToTileBase = new()
        {
            { GridCellContent.GroundTileVariation1, groundTileVariation1 },
            { GridCellContent.GroundTileVariation2, groundTileVariation2 },
        };
    }

    public void ClearTilemap()
    {
        groundTilemap.ClearAllTiles();
    }

    public void SetTileBrush(GridCellContent gridCellContent)
    {
        selectedCellContent = gridCellContent;
    }

    private void Start()
    {
        GameManager.InputManager.controls.Default.Paint.started += _ => currentBrushMode = BrushMode.Painting;
        GameManager.InputManager.controls.Default.Paint.canceled += _ => currentBrushMode = BrushMode.None;

        GameManager.InputManager.controls.Default.Erase.started += _ => currentBrushMode = BrushMode.Erasing;
        GameManager.InputManager.controls.Default.Erase.canceled += _ => currentBrushMode = BrushMode.None;

        GameManager.InputManager.controls.Default.CursorMovement.performed += 
            ctx => CursorMovement(ctx.ReadValue<Vector2>());

        GameManager.InputManager.controls.Default.Jump.started += _ => { if (selectedCellContent == GridCellContent.GroundTileVariation1) selectedCellContent = GridCellContent.GroundTileVariation2; else selectedCellContent = GridCellContent.GroundTileVariation1; };
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
        transform.position = grid.WorldToCell(mousePos);

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
        Vector3Int placementPosition = grid.WorldToCell(transform.position);
        if (!IsPositionInCanvas(placementPosition)) { return; }

        if (occupiedLocations.ContainsKey(placementPosition))
        {
            if (occupiedLocations[placementPosition] == selectedCellContent) { return; }
            occupiedLocations[placementPosition] = selectedCellContent;
        }
        else
        {
            occupiedLocations.Add(placementPosition, selectedCellContent);
        }

        
        groundTilemap.SetTile(placementPosition, cellContentToTileBase[selectedCellContent]);
    }

    private void ErasePixel()
    {
        Vector3Int removePosition = grid.WorldToCell(transform.position);
        if (!occupiedLocations.ContainsKey(removePosition)) { return; }

        groundTilemap.SetTile(removePosition, null);
        occupiedLocations.Remove(removePosition);
    }

    private bool IsPositionInCanvas(Vector3 position)
    {
        return designController.gameArea.Contains(position);
    }
}
