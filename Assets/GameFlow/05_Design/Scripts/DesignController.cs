using UnityEngine.UI;
using UnityEngine;

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
    [SerializeField] private TileBrushController tileBrushController;

    [Header("UI Images")]
    [SerializeField] private Image playerButtonImage;
    [SerializeField] private Image enemyButtonImage;
    [SerializeField] private Image programmable1ButtonImage;
    [SerializeField] private Image programmable2ButtonImage;
    [SerializeField] private Image finishButtonImage;
    [SerializeField] private Image mainGroundTileButtonImage;
    [SerializeField] private Image secondaryGroundTileButtonImage;

    [Header("Themed Tiles")]
    [SerializeField] private Sprite mainFuturisticTile;
    [SerializeField] private Sprite secondaryFuturisticTile;

    [Header("Testing (TEMPORARY)")]
    [SerializeField] private Sprite testPlayerSprite;
    [SerializeField] private Sprite testEnemySprite;
    [SerializeField] private Sprite testProgrammable1Sprite;
    [SerializeField] private Sprite testProgrammable2Sprite;
    [SerializeField] private Sprite testFinishSprite;

    private void Awake()
    {
        SetupBorder();
    }

    private void Start()
    {
        GameManager.Instance.SetPlayerSprite(testPlayerSprite);
        GameManager.Instance.SetEnemySprite(testEnemySprite);
        GameManager.Instance.SetProgrammableObject1Sprite(testProgrammable1Sprite);
        GameManager.Instance.SetProgrammableObject2Sprite(testProgrammable2Sprite);
        GameManager.Instance.SetFinishSprite(testFinishSprite);

        playerButtonImage.sprite = testPlayerSprite;
        enemyButtonImage.sprite = testEnemySprite;
        programmable1ButtonImage.sprite = testProgrammable1Sprite;
        programmable2ButtonImage.sprite = testProgrammable2Sprite;
        finishButtonImage.sprite = testFinishSprite;

        mainGroundTileButtonImage.sprite = mainFuturisticTile;
        secondaryGroundTileButtonImage.sprite = secondaryFuturisticTile;
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
        GameManager.Instance.SetLevelLayout(TileBrushController.occupiedLocations);
        GameManager.Instance.NextTurn();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(new Vector3(gameArea.center.x, gameArea.center.y, 0.01f), new Vector3(gameArea.size.x, gameArea.size.y, 0.01f));
    }
}

