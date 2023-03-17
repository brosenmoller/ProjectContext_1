using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum ProgrammableObjectSpriteType
{
    JumpPad = 0,
    BuzzSaw = 1,
    Dragon = 2,
    Robot = 3,
    Frog = 4,
    Spikes = 5,
    Turkey = 6,
    MagicOrb = 7,
    Cloud = 8,
    Potion = 9,
    Mushroom = 10,
    Icicle = 11,
    GreenPuddle = 12,
    Sparkles = 13,
    Fire = 14,
    Clover = 15,
    Candle = 16
}

public class ProgrammableObjectSpriteTypeReference
{
    public ProgrammableObjectSpriteType programmableObjectSpriteType;
    public ProgrammableObjectSpriteTypeReference(ProgrammableObjectSpriteType programmableObjectSpriteType)
    {
        this.programmableObjectSpriteType = programmableObjectSpriteType;
    }
}

public class ArtistController : MonoBehaviour
{
    [Header("Settings")]
    public int canvasSize = 16;

    [Header("References")]
    [SerializeField] private PixelBrushController brushController;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private RectTransform spriteTypeOptions;
    [SerializeField] private GameObject artChose;

    [Header("Lock References")]
    [SerializeField] private GameObject lockPlayerDraw;
    [SerializeField] private GameObject lockEnemyDraw;
    [SerializeField] private GameObject lockFinishraw;
    [Space(6)]
    [SerializeField] private GameObject playerDrawUI;
    [SerializeField] private GameObject enemyDrawUI;
    [SerializeField] private GameObject finishDrawUI;
    [Space(12)]
    [SerializeField] private GameObject lockProgrammableObject1ArtChose;
    [SerializeField] private GameObject lockProgrammableObject2ArtChose;
    [Space(6)]
    [SerializeField] private GameObject programmableObject1ArtChoseUI;
    [SerializeField] private GameObject programmableObject2ArtChoseUI;

    [Header("RectTransform to SpriteType")]
    [SerializeField] private SerializableDictionary<RectTransform, ProgrammableObjectSpriteType> buttonToSpriteType = new();

    Dictionary<ProgrammableObjectSpriteType, RectTransform> spriteTypeToButton = new();

    private Color[,] playerPixelGrid;
    private Color[,] enemyPixelGrid;
    private Color[,] finishPixelGrid;

    private ProgrammableObjectSpriteTypeReference programmableObject1SpriteType;
    private ProgrammableObjectSpriteTypeReference programmableObject2SpriteType;

    private ProgrammableObjectSpriteTypeReference currentProgrammableObjectSpriteType;

    private Outline currentSelectionOutline;
    private ArtDrawTabs currentArtDrawTab;

    private float timer;
    private bool hasEnded;
    private void Update()
    {
        if (timer >= GameManager.Instance.CurrentTurnData.timer)
        {
            OnArtistTurnEnd();
        }
        else
        {
            timer += Time.deltaTime;
            timerText.text = ((int)GameManager.Instance.CurrentTurnData.timer - (int)timer).ToString();
        }
    }

    private void Start()
    {
        foreach (SerializableKeyValuePair<RectTransform, ProgrammableObjectSpriteType> keyValuePair in buttonToSpriteType)
        {
            spriteTypeToButton.Add(keyValuePair.value, keyValuePair.key);
        }

        AssignSpriteTypes();
        AssignPixelGrids();
        ApplyArtLocks();
    }

    private void ApplyArtLocks()
    {
        lockPlayerDraw.SetActive(!GameManager.Instance.CurrentTurnData.playerDrawUnlocked);
        lockEnemyDraw.SetActive(!GameManager.Instance.CurrentTurnData.programmableEnemyUnlocked);
        lockFinishraw.SetActive(!GameManager.Instance.CurrentTurnData.finishDrawUnlocked);

        switch (GameManager.Instance.CurrentTurnData.startingArtDrawTab)
        {
            case ArtDrawTabs.Player:
                SetPlayerDraw();
                playerDrawUI.SetActive(true);
                enemyDrawUI.SetActive(false);
                finishDrawUI.SetActive(false);
                currentArtDrawTab = ArtDrawTabs.Player;
                break;
            case ArtDrawTabs.Enemy:
                SetEnemyDraw();
                playerDrawUI.SetActive(false);
                enemyDrawUI.SetActive(true);
                finishDrawUI.SetActive(false);
                currentArtDrawTab = ArtDrawTabs.Enemy;
                break;
            case ArtDrawTabs.Finish:
                SetFinishDraw();
                playerDrawUI.SetActive(false);
                enemyDrawUI.SetActive(false);
                finishDrawUI.SetActive(true);
                currentArtDrawTab = ArtDrawTabs.Finish;
                break;
        }

        if (GameManager.Instance.CurrentTurnData.artChoseStart) { EnableArtChose(); }
        else { DisableArtChose(); }

        lockProgrammableObject1ArtChose.SetActive(!GameManager.Instance.CurrentTurnData.programmableObject1Unlocked);
        lockProgrammableObject2ArtChose.SetActive(!GameManager.Instance.CurrentTurnData.programmableObject2Unlocked);

        switch (GameManager.Instance.CurrentTurnData.startingArtChoseTab)
        {
            case ArtChoseTabs.ProgrammableObject1:
                programmableObject1ArtChoseUI.SetActive(true);
                programmableObject2ArtChoseUI.SetActive(false);
                SetChoseObject1();
                break;
            case ArtChoseTabs.ProgrammableObject2:
                programmableObject1ArtChoseUI.SetActive(false);
                programmableObject2ArtChoseUI.SetActive(true);
                SetChoseObject2();
                break;
        }
    }

    public void EnableArtChose()
    {
        artChose.SetActive(true);
        brushController.canDraw = false;
    }

    public void DisableArtChose()
    {
        artChose.SetActive(false);

        switch (currentArtDrawTab)
        {
            case ArtDrawTabs.Player:
                brushController.canDraw = GameManager.Instance.CurrentTurnData.playerDrawUnlocked;
                break;
            case ArtDrawTabs.Enemy:
                brushController.canDraw = GameManager.Instance.CurrentTurnData.programmableEnemyUnlocked;
                break;
            case ArtDrawTabs.Finish:
                brushController.canDraw = GameManager.Instance.CurrentTurnData.finishDrawUnlocked;
                break;
        }
    }

    private void AssignSpriteTypes()
    {
        programmableObject1SpriteType = GameManager.Instance.GameData.programmableObject1SpriteType ?? 
            new ProgrammableObjectSpriteTypeReference(ProgrammableObjectSpriteType.JumpPad);

        programmableObject2SpriteType = GameManager.Instance.GameData.programmableObject2SpriteType ?? 
            new ProgrammableObjectSpriteTypeReference(ProgrammableObjectSpriteType.BuzzSaw);

        foreach (RectTransform child in spriteTypeOptions)
        {
            child.GetComponent<Button>().onClick.AddListener(() =>
            {
                SetProgrammableObjectSpriteType(buttonToSpriteType[child], child);
            });
        }
    }

    private void SetProgrammableObjectSpriteType(ProgrammableObjectSpriteType spriteType, RectTransform rectTransform)
    {
        currentProgrammableObjectSpriteType.programmableObjectSpriteType = spriteType;

        if (currentSelectionOutline != null) { currentSelectionOutline.enabled = false; }
        currentSelectionOutline = rectTransform.GetComponent<Outline>();
        currentSelectionOutline.enabled = true;
    }

    private void AssignPixelGrids()
    {
        playerPixelGrid = GameManager.Instance.GameData.playerSprite == null ?
            GetEmptyPixelGrid() :
            GetColorArrayFromSprite(GameManager.Instance.GameData.playerSprite);

        enemyPixelGrid = GameManager.Instance.GameData.programmableEnemySprite == null ?
            GetEmptyPixelGrid() :
            GetColorArrayFromSprite(GameManager.Instance.GameData.programmableEnemySprite);

        finishPixelGrid = GameManager.Instance.GameData.finishSprite == null ?
            GetEmptyPixelGrid() :
            GetColorArrayFromSprite(GameManager.Instance.GameData.finishSprite);
    }
    public void SetPlayerDraw()
    {
        brushController.SetPixelGrid(playerPixelGrid);
        brushController.canDraw = GameManager.Instance.CurrentTurnData.playerDrawUnlocked;
        currentArtDrawTab = ArtDrawTabs.Player;
    }

    public void SetEnemyDraw()
    {
        brushController.SetPixelGrid(enemyPixelGrid);
        brushController.canDraw = GameManager.Instance.CurrentTurnData.programmableEnemyUnlocked;
        currentArtDrawTab = ArtDrawTabs.Enemy;
    }

    public void SetFinishDraw()
    {
        brushController.SetPixelGrid(finishPixelGrid);
        brushController.canDraw = GameManager.Instance.CurrentTurnData.finishDrawUnlocked;
        currentArtDrawTab = ArtDrawTabs.Finish;
    }

    public void SetChoseObject1()
    {
        currentProgrammableObjectSpriteType = programmableObject1SpriteType;
        RectTransform startButton = spriteTypeToButton[currentProgrammableObjectSpriteType.programmableObjectSpriteType];

        if (currentSelectionOutline != null) { currentSelectionOutline.enabled = false; }
        currentSelectionOutline = startButton.GetComponent<Outline>();
        currentSelectionOutline.enabled = true;
    }

    public void SetChoseObject2()
    {
        currentProgrammableObjectSpriteType = programmableObject2SpriteType;
        RectTransform startButton = spriteTypeToButton[currentProgrammableObjectSpriteType.programmableObjectSpriteType];

        if (currentSelectionOutline != null) { currentSelectionOutline.enabled = false; }
        currentSelectionOutline = startButton.GetComponent<Outline>();
        currentSelectionOutline.enabled = true;
    }


    public void OnArtistTurnEnd()
    {
        if (hasEnded) { return; }

        GameManager.Instance.SetProgrammableObject1SpriteType(programmableObject1SpriteType);
        GameManager.Instance.SetProgrammableObject2SpriteType(programmableObject2SpriteType);

        GameManager.Instance.SetPlayerSprite(SpriteFromPixelGrid(playerPixelGrid));
        GameManager.Instance.SetEnemySprite(SpriteFromPixelGrid(enemyPixelGrid));
        GameManager.Instance.SetFinishSprite(SpriteFromPixelGrid(finishPixelGrid));

        hasEnded = true;
        Debug.Log("Artist Turn end");

        GameManager.Instance.NextTurn();
    }

    private Color[,] GetEmptyPixelGrid()
    {
        Color[,] pixelGrid = new Color[canvasSize, canvasSize];

        for (int x = 0; x < pixelGrid.GetLength(0); x++)
        {
            for (int y = 0; y < pixelGrid.GetLength(1); y++)
            {
                pixelGrid[x, y] = Color.clear;
            }
        }

        return pixelGrid;
    }

    private Color[,] GetColorArrayFromSprite(Sprite sprite)
    {
        Color[,] pixelGrid = new Color[canvasSize, canvasSize];

        for (int x = 0; x < canvasSize; x++)
        {
            for (int y = 0; y < canvasSize; y++)
            {
                pixelGrid[x, y] = sprite.texture.GetPixel(x, y);
            }
        }

        return pixelGrid;
    }

    private Sprite SpriteFromPixelGrid(Color[,] pixelGrid)
    {
        Texture2D texture = new(canvasSize, canvasSize, TextureFormat.RGBA64, false);

        for (int x = 0; x < canvasSize; x++)
        {
            for (int y = 0; y < canvasSize; y++)
            {
                texture.SetPixel(x, y, pixelGrid[x, y]);
            }
        }

        texture.Apply();
        texture.filterMode = FilterMode.Point;

        Rect rect = new(0, 0, canvasSize, canvasSize);
        return Sprite.Create(texture, rect, new Vector2(.5f, .5f), canvasSize);
    }
}

