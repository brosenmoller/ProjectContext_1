using Cinemachine;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEditor.Searcher.Searcher.AnalyticsEvent;

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

    private List<ProgrammableObject> programmableObjects = new();

    private Timer halfSecondTimer;
    private Timer threeSecondTimer;
    private Timer tenSecondTimer;

    private void Awake()
    {
        programmableObjects.Clear();
        SetupField();
    }

    private void Start()
    {
        halfSecondTimer = new Timer(.5f, () =>
        {
            foreach (ProgrammableObject obj in programmableObjects)
            {
                obj.InvokeEvent(ProgrammableEventType.EVERY_HALF_SECOND);
            }
        }, true, true);

        threeSecondTimer = new Timer(3f, () =>
        {
            foreach (ProgrammableObject obj in programmableObjects)
            {
                obj.InvokeEvent(ProgrammableEventType.EVERY_3_SECONDS);
            }
        }, true, true);

        tenSecondTimer = new Timer(10f, () =>
        {
            foreach (ProgrammableObject obj in programmableObjects)
            {
                obj.InvokeEvent(ProgrammableEventType.EVERY_10_SECONDS);
            }
        }, true, true);
    }

    public void OnPlayerWalk()
    {
        foreach (ProgrammableObject obj in programmableObjects)
        {
            obj.InvokeEvent(ProgrammableEventType.ON_PLAYER_WALK);
        }
    }

    public void OnPlayerStop()
    {
        foreach (ProgrammableObject obj in programmableObjects)
        {
            obj.InvokeEvent(ProgrammableEventType.ON_PlAYER_STOP);
        }
    }
    public void OnPlayerJump()
    {
        foreach (ProgrammableObject obj in programmableObjects)
        {
            obj.InvokeEvent(ProgrammableEventType.ON_PLAYER_JUMP);
        }
    }

    private void OnDisable()
    {
        halfSecondTimer.Pause();
        threeSecondTimer.Pause();
        tenSecondTimer.Pause();
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
                    GameObject player = Instantiate(playerPrefab, (Vector3)cell.Key + new Vector3(.5f, .5f, 0), Quaternion.identity);
                    player.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = GameManager.Instance.GameData.playerSprite;
                    virtualCamera.Follow = player.transform;
                    break;

                case GridCellContent.Finish:
                    GameObject finishObject = Instantiate(finishPrefab, (Vector3)cell.Key + new Vector3(.5f, .5f, 0), Quaternion.identity);
                    finishObject.GetComponent<SpriteRenderer>().sprite = GameManager.Instance.GameData.finishSprite;
                    break;

                case GridCellContent.Enemy:
                    GameObject enemyObject = Instantiate(programmableOjectPrefab, (Vector3)cell.Key + new Vector3(.5f, .5f, 0), Quaternion.identity);
                    ProgrammableObject enemyScript = enemyObject.GetComponent<ProgrammableObject>();
                    enemyScript.SetUpProgrammableEvents(GameManager.Instance.GameData.programmableEnemyEventsActions);
                    enemyObject.GetComponent<SpriteRenderer>().sprite = GameManager.Instance.GameData.programmableEnemySprite;
                    programmableObjects.Add(enemyScript);
                    break;

                case GridCellContent.ProgrammableObject1:
                    GameObject programmableObject1 = Instantiate(programmableOjectPrefab, (Vector3)cell.Key + new Vector3(.5f, .5f, 0), Quaternion.identity);
                    ProgrammableObject object1Script = programmableObject1.GetComponent<ProgrammableObject>();
                    object1Script.SetUpProgrammableEvents(GameManager.Instance.GameData.programmableObject1EventsActions);
                    programmableObject1.GetComponent<SpriteRenderer>().sprite = GameManager.Instance.GameData.programmableObject1Sprite;
                    programmableObjects.Add(object1Script);
                    break;

                case GridCellContent.ProgrammableObject2:
                    GameObject programmableObject2 = Instantiate(programmableOjectPrefab, (Vector3)cell.Key + new Vector3(.5f, .5f, 0), Quaternion.identity);
                    ProgrammableObject object2Script = programmableObject2.GetComponent<ProgrammableObject>();
                    object2Script.SetUpProgrammableEvents(GameManager.Instance.GameData.programmableObject2EventsActions);
                    programmableObject2.GetComponent<SpriteRenderer>().sprite = GameManager.Instance.GameData.programmableObject2Sprite;
                    programmableObjects.Add(object2Script);
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

        tile = futuristicGroundTile1;

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

        tile = futuristicGroundTile2;

        groundTilemap.SetTile(placementPosition, tile);
    }

    public void PlayTestTurnEnd()
    {
        GameManager.Instance.NextTurn();
    }
}

