using Cinemachine;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayTestController : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    [Header("Tilemap References")]
    [SerializeField] private TileBase futuristicGroundTile1;
    [SerializeField] private TileBase futuristicGroundTile2;
    [SerializeField] private TileBase forestGroundTile1;
    [SerializeField] private TileBase forestGroundTile2;
    [SerializeField] private TileBase castleGroundTile1;
    [SerializeField] private TileBase castleGroundTile2;

    [Header("Game Prefab References")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject finishPrefab;
    [SerializeField] private GameObject programmableOjectPrefab;

    private void Awake()
    {
        SetupField();
    }

    private void SetupField()
    {
        Dictionary<Vector3Int, GridCellContent> levelLayout = GameManager.Instance.GameData.levelLayout;

        foreach (KeyValuePair<Vector3Int, GridCellContent> cell in levelLayout)
        {
            switch (cell.Value)
            {
                case GridCellContent.GroundTileVariation1:
                    SetGroundTile1(cell.Key);
                    break;

                case GridCellContent.GroundTileVariation2:
                    SetGroundTile2(cell.Key);
                    break;

                case GridCellContent.Player:
                    GameObject player = Instantiate(playerPrefab, cell.Key, Quaternion.identity);
                    player.GetComponent<SpriteRenderer>().sprite = GameManager.Instance.GameData.playerSprite;
                    virtualCamera.Follow = player.transform;
                    break;

                case GridCellContent.Finish:
                    GameObject finishObject = Instantiate(finishPrefab, cell.Key, Quaternion.identity);
                    finishObject.GetComponent<SpriteRenderer>().sprite = GameManager.Instance.GameData.finishSprite;
                    break;

                case GridCellContent.Enemy:
                    GameObject enemyObject = Instantiate(programmableOjectPrefab, cell.Key, Quaternion.identity);
                    ProgrammableObject enemyScript = enemyObject.GetComponent<ProgrammableObject>();
                    enemyScript.SetUpProgrammableEvents(GameManager.Instance.GameData.programmableEnemyEventsActions);
                    enemyObject.GetComponent<SpriteRenderer>().sprite = GameManager.Instance.GameData.programmableEnemySprite;
                    break;

                case GridCellContent.ProgrammableObject1:
                    GameObject programmableObject1 = Instantiate(programmableOjectPrefab, cell.Key, Quaternion.identity);
                    ProgrammableObject object1Script = programmableObject1.GetComponent<ProgrammableObject>();
                    object1Script.SetUpProgrammableEvents(GameManager.Instance.GameData.programmableEnemyEventsActions);
                    programmableObject1.GetComponent<SpriteRenderer>().sprite = GameManager.Instance.GameData.programmableObject1Sprite;
                    break;

                case GridCellContent.ProgrammableObject2:
                    GameObject programmableObject2 = Instantiate(programmableOjectPrefab, cell.Key, Quaternion.identity);
                    ProgrammableObject object2Script = programmableObject2.GetComponent<ProgrammableObject>();
                    object2Script.SetUpProgrammableEvents(GameManager.Instance.GameData.programmableEnemyEventsActions);
                    programmableObject2.GetComponent<SpriteRenderer>().sprite = GameManager.Instance.GameData.programmableObject2Sprite;
                    break;
            }
        }
    }

    private void SetGroundTile1(Vector3Int placementPosition)
    {
        TileBase tile = GameManager.Instance.GameData.gameTheme switch
        {
            GameTheme.SciFi => futuristicGroundTile1,
            GameTheme.Forest => forestGroundTile1,
            GameTheme.Castle => castleGroundTile1,
            _ => futuristicGroundTile1,
        };

        groundTilemap.SetTile(placementPosition, tile);
    }

    private void SetGroundTile2(Vector3Int placementPosition)
    {
        TileBase tile = GameManager.Instance.GameData.gameTheme switch
        {
            GameTheme.SciFi => futuristicGroundTile2,
            GameTheme.Forest => forestGroundTile2,
            GameTheme.Castle => castleGroundTile2,
            _ => futuristicGroundTile2,
        };

        groundTilemap.SetTile(placementPosition, tile);
    }

    public void PlayTestTurnEnd()
    {
        GameManager.Instance.NextTurn();
    }
}

