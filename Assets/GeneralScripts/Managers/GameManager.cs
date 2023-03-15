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

    private readonly int[] GameFlowSceneIndexArray = new int[]
    {
        0, // Start Menu
        1, // Select Theme
        2, // Enter Names

        3, // First Development Turn
        4, // First Artist Turn
        5, // First Design Turn
        6, // First PlayTest Turn

        3, // Second Development Turn
        4, // Second Artist Turn
        5, // Second Design Turn
        6, // Second PlayTest Turn

        3, // Third Development Turn
        4, // Third Artist Turn
        5, // Third Design Turn
        6, // Third PlayTest Turn
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
            Destroy(Instance);
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
            SceneManager.LoadScene(GameFlowSceneIndexArray[currentGameFlowFase]);
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

