using UnityEngine.UI;
using UnityEngine;
using TMPro;

public enum GridCellContent
{
    Player = 0,
    Enemy = 1,
    ProgrammableObject1 = 2,
    ProgrammableObject2 = 3,
    GroundTileVariation1 = 4,
    GroundTileVariation2 = 5,
    Finish = 6,
}

public class DesignController : MonoBehaviour
{
    [Header("Settings")]
    public Rect gameArea;

    [Header("References")]
    [SerializeField] private LineRenderer borderLine;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TileBrushController tileBrushController;

    [Header("UI Images")]
    [SerializeField] private Image playerButtonImage;
    [SerializeField] private Image enemyButtonImage;
    [SerializeField] private Image programmable1ButtonImage;
    [SerializeField] private Image programmable2ButtonImage;
    [SerializeField] private Image finishButtonImage;
    [SerializeField] private Image mainGroundTileButtonImage;
    [SerializeField] private Image secondaryGroundTileButtonImage;

    [Header("Lock References")]
    [SerializeField] private GameObject lockProgrammableObject1;
    [SerializeField] private GameObject lockProgrammableEnemy;
    [SerializeField] private GameObject lockProgrammableObject2;
    [SerializeField] private GameObject lockPlayer;
    [SerializeField] private GameObject lockFinish;

    [Header("Themed Tiles")]
    [SerializeField] private Sprite mainFuturisticTile;
    [SerializeField] private Sprite secondaryFuturisticTile;
    [SerializeField] private Sprite mainCastleTile;
    [SerializeField] private Sprite secondaryCastleTile;
    [SerializeField] private Sprite mainForestTile;
    [SerializeField] private Sprite secondaryForestTile;

    [Header("Themed Backgrounds")]
    [SerializeField] private GameObject futuristicBackground;
    [SerializeField] private GameObject castleBackground;
    [SerializeField] private GameObject forestBackground;

    [Header("Sprite From Sprite Type")]
    [SerializeField] private SerializableDictionary<ProgrammableObjectSpriteType, Sprite> spriteFromSpriteType = new();

    private float timer;

    private void Awake()
    {
        SetupBorder();
    }

    private void Start()
    {
        SetThemedSprites();
        ApplyDesignerLocks();
    }

    private void SetThemedSprites()
    {
        if (GameManager.Instance.GameData.playerSprite != null)
        { playerButtonImage.sprite = GameManager.Instance.GameData.playerSprite; }

        if (GameManager.Instance.GameData.finishSprite != null)
        { finishButtonImage.sprite = GameManager.Instance.GameData.finishSprite; }

        if (GameManager.Instance.GameData.programmableObject1SpriteType != null)
        {
            GameManager.Instance.GameData.programmableObject1Sprite = spriteFromSpriteType[GameManager.Instance.GameData.programmableObject1SpriteType.programmableObjectSpriteType];
            programmable1ButtonImage.sprite = spriteFromSpriteType[GameManager.Instance.GameData.programmableObject1SpriteType.programmableObjectSpriteType];
        }

        if (GameManager.Instance.GameData.programmableObject2SpriteType != null)
        {
            GameManager.Instance.GameData.programmableObject2Sprite = spriteFromSpriteType[GameManager.Instance.GameData.programmableObject2SpriteType.programmableObjectSpriteType];
            programmable2ButtonImage.sprite = spriteFromSpriteType[GameManager.Instance.GameData.programmableObject2SpriteType.programmableObjectSpriteType];
        }

        if (GameManager.Instance.GameData.programmableEnemySprite != null)
        { enemyButtonImage.sprite = GameManager.Instance.GameData.programmableEnemySprite; }

        switch (GameManager.Instance.GameData.gameTheme)
        {
            case GameTheme.SciFi:
                mainGroundTileButtonImage.sprite = mainFuturisticTile;
                secondaryGroundTileButtonImage.sprite = secondaryFuturisticTile;
                futuristicBackground.SetActive(true);
                forestBackground.SetActive(false);
                castleBackground.SetActive(false);
                break;
            case GameTheme.Castle:
                mainGroundTileButtonImage.sprite = mainCastleTile;
                secondaryGroundTileButtonImage.sprite = secondaryCastleTile;
                futuristicBackground.SetActive(false);
                castleBackground.SetActive(true);
                forestBackground.SetActive(false);
                break;
            case GameTheme.Forest:
                mainGroundTileButtonImage.sprite = mainForestTile;
                secondaryGroundTileButtonImage.sprite = secondaryForestTile;
                futuristicBackground.SetActive(false);
                castleBackground.SetActive(false);
                forestBackground.SetActive(true);
                break;
        }
    }

    private void ApplyDesignerLocks()
    {
        lockProgrammableObject1.SetActive(!GameManager.Instance.CurrentTurnData.programmableObject1Unlocked);
        lockProgrammableEnemy.SetActive(!GameManager.Instance.CurrentTurnData.programmableEnemyUnlocked);
        lockProgrammableObject2.SetActive(!GameManager.Instance.CurrentTurnData.programmableObject2Unlocked);
        lockPlayer.SetActive(!GameManager.Instance.CurrentTurnData.playerDrawUnlocked);
        lockFinish.SetActive(!GameManager.Instance.CurrentTurnData.finishDrawUnlocked);
    } 

    private void Update()
    {
        if (timer >= GameManager.Instance.CurrentTurnData.timer)
        {
            OnDesignTurnEnd();
        }
        else
        {
            timer += Time.deltaTime;
            timerText.text = ((int)GameManager.Instance.CurrentTurnData.timer - (int)timer).ToString();
        }
    }

    private void SetupBorder()
    {
        borderLine.positionCount = 4;

        borderLine.SetPositions(new Vector3[4]
        {
           new Vector3(gameArea.x, gameArea.y, 0),
           new Vector3(gameArea.x + gameArea.width, gameArea.y, 0),
           new Vector3(gameArea.x + gameArea.width, gameArea.y + gameArea.height, 0),
           new Vector3(gameArea.x, gameArea.y + gameArea.height, 0),
        });
    }

    public void OnDesignTurnEnd()
    {
        GameManager.Instance.SetLevelLayout(tileBrushController.occupiedLocations);
        GameManager.Instance.NextTurn();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(new Vector3(gameArea.center.x, gameArea.center.y, 0.01f), new Vector3(gameArea.size.x, gameArea.size.y, 0.01f));
    }
}

