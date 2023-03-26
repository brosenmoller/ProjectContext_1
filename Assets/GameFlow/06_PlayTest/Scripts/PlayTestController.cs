using Cinemachine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayTestController : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private RectTransform hearts;
    [SerializeField] private GameObject turnEndButton;
    [SerializeField] private AudioObject playTestMusic;

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

    [Header("Themed Backgrounds")]
    [SerializeField] private GameObject futuristicBackground;
    [SerializeField] private GameObject castleBackground;
    [SerializeField] private GameObject forestBackground;

    private List<ProgrammableObject> programmableObjects = new();

    private float timer = 0;
    private bool hasEnded;
    private const string timerSaveKey = "PlayTestTimer";

    private int health = 3;
    public int Health { 
        get { return health; } 
        set 
        {
            health = value;

            if (health > 3) { health = 3; }

            if (health <= 0)
            {
                ReloadScene();
            }
            else if (health == 3)
            {
                hearts.GetChild(0).gameObject.SetActive(true);
                hearts.GetChild(1).gameObject.SetActive(true);
                hearts.GetChild(2).gameObject.SetActive(true);
            }
            else if (health == 2)
            {
                hearts.GetChild(0).gameObject.SetActive(true);
                hearts.GetChild(1).gameObject.SetActive(true);
                hearts.GetChild(2).gameObject.SetActive(false);
            }
            else if (health == 1)
            {
                hearts.GetChild(0).gameObject.SetActive(true);
                hearts.GetChild(1).gameObject.SetActive(false);
                hearts.GetChild(2).gameObject.SetActive(false);
            }
        }
    }

    private void Awake()
    {
        timer = PlayerPrefs.GetFloat(timerSaveKey, 0);
        programmableObjects.Clear();
        if (GameManager.Instance.CurrentTurnData.infinitePlayTest) 
        { 
            timerText.gameObject.SetActive(false); 
            turnEndButton.SetActive(false);
        }
        SetBackground();
        SetupField();
    }

    public void ReloadScene()
    {
        PlayerPrefs.SetFloat(timerSaveKey, timer);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void Start()
    {
        playTestMusic.Play();
        StartCoroutine(TimerEvents());
    }

    private void Update()
    {
        UpdateTimer();
    }

    private void UpdateTimer()
    {
        if (GameManager.Instance.CurrentTurnData.infinitePlayTest) { return; }

        if (timer >= GameManager.Instance.CurrentTurnData.timer)
        {
            PlayTestTurnEnd();
        }
        else
        {
            timer += Time.deltaTime;
            timerText.text = ((int)GameManager.Instance.CurrentTurnData.timer - (int)timer).ToString();
        }
    }

    private void SetBackground()
    {
        switch (GameManager.Instance.GameData.gameTheme)
        {
            case GameTheme.SciFi:
                futuristicBackground.SetActive(true);
                forestBackground.SetActive(false);
                castleBackground.SetActive(false);
                break;
            case GameTheme.Castle:
                futuristicBackground.SetActive(false);
                castleBackground.SetActive(true);
                forestBackground.SetActive(false);
                break;
            case GameTheme.Forest:
                futuristicBackground.SetActive(false);
                castleBackground.SetActive(false);
                forestBackground.SetActive(true);
                break;
        }
    }

    private IEnumerator TimerEvents()
    {
        int halfSecondCounter = 0;

        while (true)
        {
            yield return new WaitForSeconds(.5f);
            halfSecondCounter++;
            EveryHalfSecond();
            if (halfSecondCounter == 6 || halfSecondCounter == 12 || halfSecondCounter == 18)
            {
                EveryThreeSeconds();
            }
            else if (halfSecondCounter == 20)
            {
                EveryTenSeconds();
                halfSecondCounter = 0;
            }
        }
    }

    private void EveryHalfSecond()
    {
        foreach (ProgrammableObject obj in programmableObjects)
        {
            obj.InvokeEvent(ProgrammableEventType.EVERY_HALF_SECOND);
        }
    }

    private void EveryThreeSeconds()
    {
        foreach (ProgrammableObject obj in programmableObjects)
        {
            obj.InvokeEvent(ProgrammableEventType.EVERY_3_SECONDS);
        }
    }

    private void EveryTenSeconds()
    {
        foreach (ProgrammableObject obj in programmableObjects)
        {
            obj.InvokeEvent(ProgrammableEventType.EVERY_10_SECONDS);
        }
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
        StopAllCoroutines();
    }

    private void SetupField()
    {
        if (GameManager.Instance.GameData.levelLayout == null) { return; }

        foreach (KeyValuePair<Vector3Int, GridCellContent> cell in GameManager.Instance.GameData.levelLayout)
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
        if (hasEnded) { return; }
        hasEnded = true;

        playTestMusic.Stop();

        PlayerPrefs.SetFloat(timerSaveKey, 0);
        GameManager.Instance.NextTurn();
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetFloat(timerSaveKey, 0);
    }
}

