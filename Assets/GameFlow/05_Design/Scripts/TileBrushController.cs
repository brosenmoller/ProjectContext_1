using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] private GameObject emptySpriteRenderer;

    [Header("References")]
    [SerializeField] private SpriteRenderer brushSpriteRender;
    [SerializeField] private DesignController designController;

    private GridCellContent selectedCellContent;

    private enum BrushMode { Painting, Erasing, None }
    private BrushMode currentBrushMode = BrushMode.None;

    private Dictionary<Vector3Int, GameObject> locationsOfNonTilemapPrefabs = new();


    private void Awake()
    {
        occupiedLocations.Clear();
        locationsOfNonTilemapPrefabs.Clear();
        selectedCellContent = GridCellContent.GroundTileVariation1;
    }

    public void ClearTilemap()
    {
        groundTilemap.ClearAllTiles();
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
            case BrushMode.Painting: PaintTile(); break;
            case BrushMode.Erasing: EraseTile(); break;
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

    private void PaintTile()
    {
        Vector3Int placementPosition = grid.WorldToCell(transform.position);
        if (!IsPositionInCanvas(placementPosition)) { return; }

        if (occupiedLocations.ContainsKey(placementPosition))
        {
            if (occupiedLocations[placementPosition] == selectedCellContent) { return; }
            
            UnsetTile(placementPosition, occupiedLocations[placementPosition]);
            occupiedLocations.Remove(placementPosition);
        }

        if (selectedCellContent == GridCellContent.Player ||
            selectedCellContent == GridCellContent.Finish)
        { 
            RemoveAllCellContentOfType(selectedCellContent); 
        }

        occupiedLocations.Add(placementPosition, selectedCellContent);
        SetTile(placementPosition, selectedCellContent);
    }

    private void RemoveAllCellContentOfType(GridCellContent cellContent)
    {
        while (occupiedLocations.ContainsValue(cellContent))
        {
            Vector3Int firstKey = occupiedLocations.FirstOrDefault(
                keyValuePair => keyValuePair.Value == cellContent
            ).Key;

            UnsetTile(firstKey, cellContent);
            occupiedLocations.Remove(firstKey);
        }
    }

    private void EraseTile()
    {
        Vector3Int removePosition = grid.WorldToCell(transform.position);
        if (!occupiedLocations.ContainsKey(removePosition)) { return; }

        UnsetTile(removePosition, occupiedLocations[removePosition]);
        occupiedLocations.Remove(removePosition);
    }

    private void SetTile(Vector3Int placementPosition, GridCellContent cellContent)
    {
        switch (cellContent)
        {
            case GridCellContent.GroundTileVariation1:
                groundTilemap.SetTile(placementPosition, groundTileVariation1);
                break;
            case GridCellContent.GroundTileVariation2:
                groundTilemap.SetTile(placementPosition, groundTileVariation2);
                break;
            case GridCellContent.Player:
                locationsOfNonTilemapPrefabs.Add(
                    placementPosition,
                    InstantiateSprite(placementPosition, GameManager.Instance.GameData.playerSprite)
                );
                
                break;
            case GridCellContent.Enemy:
                locationsOfNonTilemapPrefabs.Add(
                    placementPosition,
                    InstantiateSprite(placementPosition, GameManager.Instance.GameData.programmableEnemySprite)
                );

                break;
            case GridCellContent.Finish:
                locationsOfNonTilemapPrefabs.Add(
                    placementPosition,
                    InstantiateSprite(placementPosition, GameManager.Instance.GameData.finishSprite)
                );

                break;
            case GridCellContent.ProgrammableObject1:
                locationsOfNonTilemapPrefabs.Add(
                    placementPosition, 
                    InstantiateSprite(placementPosition, GameManager.Instance.GameData.programmableObject1Sprite)
                );

                break;
            case GridCellContent.ProgrammableObject2:
                locationsOfNonTilemapPrefabs.Add(
                    placementPosition,
                    InstantiateSprite(placementPosition, GameManager.Instance.GameData.programmableObject2Sprite)
                );

                break;
        }
    }

    private void UnsetTile(Vector3Int removePosition, GridCellContent cellContent)
    {
        if (cellContent == GridCellContent.GroundTileVariation1 || 
            cellContent == GridCellContent.GroundTileVariation2)
        {
            groundTilemap.SetTile(removePosition, null);
        }
        else
        {
            DestroySprite(removePosition);
        }
    }

    private GameObject InstantiateSprite(Vector3Int placementPosition, Sprite sprite)
    {
        GameObject newObject = Instantiate(emptySpriteRenderer, (Vector3)placementPosition + new Vector3(.5f, .5f, 0), Quaternion.identity);
        SpriteRenderer newObjectRenderer = newObject.GetComponent<SpriteRenderer>();
        newObjectRenderer.sprite = sprite;

        return newObject;
    }

    private void DestroySprite(Vector3Int removePosition)
    {
        if (!locationsOfNonTilemapPrefabs.ContainsKey(removePosition)) { return; }

        GameObject objectToBeRemoved = locationsOfNonTilemapPrefabs[removePosition];
        locationsOfNonTilemapPrefabs.Remove(removePosition);
        Destroy(objectToBeRemoved);
    }

    private bool IsPositionInCanvas(Vector3 position)
    {
        return designController.gameArea.Contains(position);
    }

    #region Set Tile brush

    public void SetTileBrush(GridCellContent gridCellContent) => selectedCellContent = gridCellContent;
    public void SetTileBrush_Player() => selectedCellContent = GridCellContent.Player;
    public void SetTileBrush_Enemy() => selectedCellContent = GridCellContent.Enemy;
    public void SetTileBrush_Finish() => selectedCellContent = GridCellContent.Finish;
    public void SetTileBrush_ProgrammableObject1() => selectedCellContent = GridCellContent.ProgrammableObject1;
    public void SetTileBrush_ProgrammableObject2() => selectedCellContent = GridCellContent.ProgrammableObject2;
    public void SetTileBrush_GroundTileVariation1() => selectedCellContent = GridCellContent.GroundTileVariation1;
    public void SetTileBrush_GroundTileVariation2() => selectedCellContent = GridCellContent.GroundTileVariation2;

    #endregion
}
