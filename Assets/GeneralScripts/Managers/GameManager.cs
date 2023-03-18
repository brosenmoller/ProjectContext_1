using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public static AudioManager AudioManager { get; private set; }
    public static TimerManager TimerManager { get; private set; }
    public static SaveManager SaveManager { get; private set; }
    public static EventManager EventManager { get; private set; }
    public static InputManager InputManager { get; private set; }

    private Manager[] activeManagers;

    public GameData GameData { get; private set; }

    public TurnData CurrentTurnData { get { return GameFlowSceneIndexArray[currentGameFlowFase]; } }

    private readonly TurnData[] GameFlowSceneIndexArray = new TurnData[]
    {
        new TurnData(0, 30f, RoomType.Other, Player.Unassigned), // Start Menu
        new TurnData(1, 30f, RoomType.Other, Player.Unassigned), // Select Theme
        new TurnData(2, 30f, RoomType.Other, Player.Unassigned), // Enter Names



        new TurnData(7, 5f, RoomType.Development, Player.Player1), // Transition
        new TurnData(3, 40f, RoomType.Development, Player.Player1, true, false, false, true, false, true, DeveloperTabs.ProgrammableObject1, ArtDrawTabs.Player, ArtChoseTabs.ProgrammableObject1), // First Development Turn
        
        new TurnData(7, 5f, RoomType.Art, Player.Player2), // Transition
        new TurnData(4, 40f, RoomType.Art, Player.Player2, true, false, false, true, false, true, DeveloperTabs.ProgrammableObject1, ArtDrawTabs.Player, ArtChoseTabs.ProgrammableObject1), // First Artist Turn
        
        new TurnData(7, 5f, RoomType.Design, Player.Player3), // Transition
        new TurnData(5, 40f, RoomType.Design, Player.Player3, true, false, false, true, false, true, DeveloperTabs.ProgrammableObject1, ArtDrawTabs.Player, ArtChoseTabs.ProgrammableObject1), // First Design Turn
        
        new TurnData(7, 5f, RoomType.PlayTest, Player.Player1), // Transition
        new TurnData(6, 40f, RoomType.PlayTest, Player.Player1, true, false, false, true, false, true, DeveloperTabs.ProgrammableObject1, ArtDrawTabs.Player, ArtChoseTabs.ProgrammableObject1), // First PlayTest Turn



        new TurnData(7, 5f, RoomType.Development, Player.Player2), // Transition
        new TurnData(3, 40f, RoomType.Development, Player.Player2, true, false, true, true, false, false, DeveloperTabs.ProgrammableEnemy, ArtDrawTabs.Enemy, ArtChoseTabs.ProgrammableObject1), // Second Development Turn
        
        new TurnData(7, 5f, RoomType.Art, Player.Player3), // Transition
        new TurnData(4, 40f, RoomType.Art, Player.Player3, true, false, true, true, false, false, DeveloperTabs.ProgrammableEnemy, ArtDrawTabs.Enemy, ArtChoseTabs.ProgrammableObject1), // Second Artist Turn
        
        new TurnData(7, 5f, RoomType.Design, Player.Player1), // Transition
        new TurnData(5, 40f, RoomType.Design, Player.Player1, true, false, true, true, false, false, DeveloperTabs.ProgrammableEnemy, ArtDrawTabs.Enemy, ArtChoseTabs.ProgrammableObject1), // Second Design Turn
        
        new TurnData(7, 5f, RoomType.PlayTest, Player.Player2), // Transition
        new TurnData(6, 40f, RoomType.PlayTest, Player.Player2, true, false, true, true, false, false, DeveloperTabs.ProgrammableEnemy, ArtDrawTabs.Enemy, ArtChoseTabs.ProgrammableObject1), // Second PlayTest Turn
       


        new TurnData(7, 5f, RoomType.Development, Player.Player3), // Transition
        new TurnData(3, 40f, RoomType.Development, Player.Player3, true, true, true, true, true, true, DeveloperTabs.ProgrammableObject2, ArtDrawTabs.Finish, ArtChoseTabs.ProgrammableObject2), // Third Development Turn
        
        new TurnData(7, 5f, RoomType.Art, Player.Player1), // Transition
        new TurnData(4, 40f, RoomType.Art, Player.Player1, true, true, true, true, true, true, DeveloperTabs.ProgrammableObject2, ArtDrawTabs.Finish, ArtChoseTabs.ProgrammableObject2), // Third Artist Turn
        
        new TurnData(7, 5f, RoomType.Design, Player.Player2), // Transition
        new TurnData(5, 40f, RoomType.Design, Player.Player2, true, true, true, true, true, true, DeveloperTabs.ProgrammableObject2, ArtDrawTabs.Finish, ArtChoseTabs.ProgrammableObject2), // Third Design Turn
        
        new TurnData(7, 5f, RoomType.PlayTest, Player.Player3), // Transition
        new TurnData(6, 40f, RoomType.PlayTest, Player.Player3, true, true, true, true, true, true, DeveloperTabs.ProgrammableObject2, ArtDrawTabs.Finish, ArtChoseTabs.ProgrammableObject2, true), // Third PlayTest Turn
    };

    private int currentGameFlowFase = 0;

    private void Awake()
    {
        GameData = new GameData();

        SingletonSetup();
        ManagerSetup();
    }

    private void SingletonSetup()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void ManagerSetup()
    {
        AudioManager = new AudioManager();
        TimerManager = new TimerManager();
        SaveManager = new SaveManager();
        EventManager = new EventManager();
        InputManager = new InputManager();

        activeManagers = new Manager[] {
            AudioManager,
            TimerManager,
            SaveManager,
            EventManager,
            InputManager
        };

        foreach (Manager manager in activeManagers)
        {
            manager.Setup();
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene loadedScene, LoadSceneMode loadSceneMode)
    {
        foreach (Manager manager in activeManagers)
        {
            manager.OnSceneLoad();
        }
    }
    
    private void FixedUpdate()
    {
        foreach (Manager manager in activeManagers)
        {
            manager.OnFixedUpdate();
        }
    }

    public void NextTurn()
    {
        currentGameFlowFase++;
        if (currentGameFlowFase < GameFlowSceneIndexArray.Length)
        {
            SceneManager.LoadScene(CurrentTurnData.sceneIndex);
        }
    }

    #region SaveManager ContextMenu
    [ContextMenu("SaveManager/Save")] public void Save() => SaveManager?.Save();
    [ContextMenu("SaveManager/Load")] public void Load() => SaveManager?.Load();
    [ContextMenu("SaveManager/DeleteSave")] public void DeleteSave() => SaveManager?.DeleteSave();

    #endregion

    #region Setting Gamedata

    public void SetGameTheme(GameTheme gameTheme) => GameData.gameTheme = gameTheme;

    public void SetNamePlayer1(string name) => GameData.namePlayer1 = name;
    public void SetNamePlayer2(string name) => GameData.namePlayer2 = name;
    public void SetNamePlayer3(string name) => GameData.namePlayer3 = name;

    public void SetPlayerSprite(Sprite sprite) => GameData.playerSprite = sprite;
    public void SetEnemySprite(Sprite sprite) => GameData.programmableEnemySprite = sprite;
    public void SetFinishSprite(Sprite sprite) => GameData.finishSprite = sprite;
    public void SetProgrammableObject1Sprite(Sprite sprite) => GameData.programmableObject1Sprite = sprite;
    public void SetProgrammableObject2Sprite(Sprite sprite) => GameData.programmableObject2Sprite = sprite;

    public void SetProgrammableObject1SpriteType(ProgrammableObjectSpriteTypeReference spriteType) => GameData.programmableObject1SpriteType = spriteType;
    public void SetProgrammableObject2SpriteType(ProgrammableObjectSpriteTypeReference spriteType) => GameData.programmableObject2SpriteType = spriteType;

    public void SetProgrammableObject1EventsActions
    (
        Dictionary<ProgrammableEventType, ProgrammableActionType[]> dictionary
    )
    => GameData.programmableObject1EventsActions = dictionary;

    public void SetProgrammableObject2EventsActions
    (
        Dictionary<ProgrammableEventType, ProgrammableActionType[]> dictionary
    )
    => GameData.programmableObject2EventsActions = dictionary;
    
    public void SetProgrammableEnemyEventsActions
    (
        Dictionary<ProgrammableEventType, ProgrammableActionType[]> dictionary
    )
    => GameData.programmableEnemyEventsActions = dictionary;

    public void SetLevelLayout(Dictionary<Vector3Int, GridCellContent> dictionary) => GameData.levelLayout = dictionary;

    #endregion
}

