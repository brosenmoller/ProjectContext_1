using System.Collections.Generic;
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
    [SerializeField] private RectTransform spriteTypeOptions;
    [SerializeField] private GameObject artChose;

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

    private void Start()
    {
        foreach (SerializableKeyValuePair<RectTransform, ProgrammableObjectSpriteType> keyValuePair in buttonToSpriteType)
        {
            spriteTypeToButton.Add(keyValuePair.value, keyValuePair.key);
        }

        AssignSpriteTypes();
        AssignPixelGrids();
    }

    public void EnableArtChose()
    {
        artChose.SetActive(true);
        brushController.canDraw = false;
    }

    public void DisableArtChose()
    {
        artChose.SetActive(false);
        brushController.canDraw = true;
    }

    private void AssignSpriteTypes()
    {
        programmableObject1SpriteType = GameManager.Instance.GameData.programmableObject1SpriteType ?? 
            new ProgrammableObjectSpriteTypeReference(ProgrammableObjectSpriteType.JumpPad);

        programmableObject2SpriteType = GameManager.Instance.GameData.programmableObject2SpriteType ?? 
            new ProgrammableObjectSpriteTypeReference(ProgrammableObjectSpriteType.BuzzSaw);

        SetChoseObject1();

        foreach (RectTransform child in spriteTypeOptions)
        {
            child.GetComponent<Button>().onClick.AddListener(() =>
            {
                currentProgrammableObjectSpriteType.programmableObjectSpriteType = buttonToSpriteType[child];

                if (currentSelectionOutline != null) { currentSelectionOutline.enabled = false; }
                currentSelectionOutline = child.GetComponent<Outline>();
                currentSelectionOutline.enabled = true;
            });
        }
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

        SetPlayerDraw();
    }
    public void SetPlayerDraw()
    {
        brushController.SetPixelGrid(playerPixelGrid);
    }

    public void SetEnemyDraw()
    {
        brushController.SetPixelGrid(enemyPixelGrid);
    }

    public void SetFinishDraw()
    {
        brushController.SetPixelGrid(finishPixelGrid);
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
        GameManager.Instance.SetProgrammableObject1SpriteType(programmableObject1SpriteType);
        GameManager.Instance.SetProgrammableObject2SpriteType(programmableObject2SpriteType);

        GameManager.Instance.SetPlayerSprite(SpriteFromPixelGrid(playerPixelGrid));
        GameManager.Instance.SetEnemySprite(SpriteFromPixelGrid(enemyPixelGrid));
        GameManager.Instance.SetFinishSprite(SpriteFromPixelGrid(finishPixelGrid));
        
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
        return Sprite.Create(texture, rect, new Vector2(.5f, .5f), canvasSize / 2);
    }
}

