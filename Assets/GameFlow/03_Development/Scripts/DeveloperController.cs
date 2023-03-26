using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum ProgrammableEventType
{
    ON_START = 0,
    ON_PLAYER_COLLIDE = 1,
    ON_PLAYER_JUMP = 2,
    ON_PLAYER_WALK = 3,
    ON_PlAYER_STOP = 4,
    ON_PLAYER_IN_RANGE = 5,
    ON_COLLIDE = 6,
    EVERY_HALF_SECOND = 7,
    EVERY_3_SECONDS = 8,
    EVERY_10_SECONDS = 9,
}

public enum ProgrammableActionType
{
    RELOAD_SCENE = 0,
    PLAYER_BIG_JUMP = 1,
    PLAYER_PLUS_ONE_HP = 2,
    PLAYER_MINUS_ONE_HP = 3,
    OBJECT_MOVE_FORWARD = 4,
    OBJECT_TURN_AROUND = 6,
    OBJECT_STOP_MOVING = 7,
    OBJECT_PAUSE_MOVING_3_SECONDS = 8,
    OBJECT_MOVE_DOWN = 9,
    OBJECT_MOVE_UP = 10,
}

public class DeveloperController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject lineRendererPrefab;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Color defaultConnectorColor;
    [SerializeField] private Color selectedConnectorColor;
    [SerializeField] private AudioObject music;

    [Header("Lock References")]
    [SerializeField] private GameObject lockProgrammableObject1;
    [SerializeField] private GameObject lockProgrammableEnemy;
    [SerializeField] private GameObject lockProgrammableObject2;
    [Space(6)]
    [SerializeField] private GameObject programmableObject1UI;
    [SerializeField] private GameObject programmableEnemyUI;
    [SerializeField] private GameObject programmableObject2UI;
    
    [Header("Settings")]
    [SerializeField] private float bezierCurveInset;
    [SerializeField, Range(4, 50)] private int bezierCurveResolution;

    [Header("Event And Actino Connectors")]
    [SerializeField] private SerializableDictionary<Button, ProgrammableEventType> eventConnectors = new();
    [SerializeField] private SerializableDictionary<Button, ProgrammableActionType> actionConnectors = new();

    private Dictionary<ProgrammableEventType, Button> eventConnectorButtons = new();
    private Dictionary<ProgrammableActionType, Button> actionConnectorButtons = new();

    private Dictionary<ProgrammableEventType, ProgrammableActionType[]> localEnemyEventsActions;
    private Dictionary<ProgrammableEventType, ProgrammableActionType[]> localObject1EventsActions;
    private Dictionary<ProgrammableEventType, ProgrammableActionType[]> localObject2EventsActions;

    private Dictionary<ProgrammableEventType, ProgrammableActionType[]> currentEventsActions;

    private Button currentEventConnector = null;
    private Button currentActionConnector = null;

    private float timer;
    private bool hasEnded;

    private Dictionary<(ProgrammableEventType, ProgrammableActionType), GameObject> connectionTypeToLineRenderer = new();

    private void Awake()
    {
        music.Play();
        Cursor.visible = true;

        foreach (SerializableKeyValuePair<Button, ProgrammableEventType> keyValuePair in eventConnectors)
        {
            eventConnectorButtons.Add(keyValuePair.value, keyValuePair.key);
        }

        foreach (SerializableKeyValuePair<Button, ProgrammableActionType> keyValuePair in actionConnectors)
        {
            actionConnectorButtons.Add(keyValuePair.value, keyValuePair.key);
        }

        localEnemyEventsActions = GameManager.Instance.GameData.programmableEnemyEventsActions != null ?
            localEnemyEventsActions = GameManager.Instance.GameData.programmableEnemyEventsActions :
            localEnemyEventsActions = new Dictionary<ProgrammableEventType, ProgrammableActionType[]>();

        localObject1EventsActions = GameManager.Instance.GameData.programmableObject1EventsActions != null ?
            localObject1EventsActions = GameManager.Instance.GameData.programmableObject1EventsActions :
            localObject1EventsActions = new Dictionary<ProgrammableEventType, ProgrammableActionType[]>();

        localObject2EventsActions = GameManager.Instance.GameData.programmableObject2EventsActions != null ?
            localObject2EventsActions = GameManager.Instance.GameData.programmableObject2EventsActions :
            localObject2EventsActions = new Dictionary<ProgrammableEventType, ProgrammableActionType[]>();

        ApplyDeveloperLocks();
    }

    private void Update()
    {
        if (timer >= GameManager.Instance.CurrentTurnData.timer)
        {
            DeveloperTurnEnd();
        }
        else
        {
            timer += Time.deltaTime;
            timerText.text = ((int)GameManager.Instance.CurrentTurnData.timer - (int)timer).ToString();
        }
    }

    private void ApplyDeveloperLocks()
    {
        lockProgrammableObject1.SetActive(!GameManager.Instance.CurrentTurnData.programmableObject1Unlocked);
        lockProgrammableEnemy.SetActive(!GameManager.Instance.CurrentTurnData.programmableEnemyUnlocked);
        lockProgrammableObject2.SetActive(!GameManager.Instance.CurrentTurnData.programmableObject2Unlocked);

        switch (GameManager.Instance.CurrentTurnData.startingDeveloperTab)
        {
            case DeveloperTabs.ProgrammableObject1:
                SetCurrentEventsActions_ProgrammableObject1();
                programmableObject1UI.SetActive(true);
                programmableEnemyUI.SetActive(false);
                programmableObject2UI.SetActive(false);
                break;
            case DeveloperTabs.ProgrammableEnemy:
                SetCurrentEventsActions_ProgrammableEnemy();
                programmableObject1UI.SetActive(false);
                programmableEnemyUI.SetActive(true);
                programmableObject2UI.SetActive(false);
                break;
            case DeveloperTabs.ProgrammableObject2:
                SetCurrentEventsActions_ProgrammableObject2();
                programmableObject1UI.SetActive(false);
                programmableEnemyUI.SetActive(false);
                programmableObject2UI.SetActive(true);
                break;
        }
    }

    public void SetCurrentEventConnector(Button button)
    {
        if (currentEventConnector != button)
        {
            if (currentEventConnector != null) { currentEventConnector.GetComponent<Image>().color = defaultConnectorColor; }
            currentEventConnector = button;
            currentEventConnector.GetComponent<Image>().color = selectedConnectorColor;
        }
        else
        {
            currentEventConnector.GetComponent<Image>().color = defaultConnectorColor;
            currentEventConnector = null;
        }

        if (currentEventConnector != null && currentActionConnector != null)
        {
            ConnectEventToAction();
        }
    }

    public void SetCurrentActionConnector(Button button)
    {
        if (currentActionConnector != button)
        {
            if (currentActionConnector != null) { currentActionConnector.GetComponent<Image>().color = defaultConnectorColor; }
            currentActionConnector = button;
            currentActionConnector.GetComponent<Image>().color = selectedConnectorColor;
        }
        else
        {
            currentActionConnector.GetComponent<Image>().color = defaultConnectorColor;
            currentActionConnector = null;
        }

        if (currentEventConnector != null && currentActionConnector != null)
        {
            ConnectEventToAction();
        }
    }

    private void ConnectEventToAction()
    {
        currentActionConnector.GetComponent<Image>().color = defaultConnectorColor;
        currentEventConnector.GetComponent<Image>().color = defaultConnectorColor;

        ProgrammableEventType programmableEventType = eventConnectors[currentEventConnector];
        ProgrammableActionType programmableActionType = actionConnectors[currentActionConnector];

        if (currentEventsActions.ContainsKey(programmableEventType))
        {
            if (currentEventsActions[programmableEventType].Contains(programmableActionType)) 
            {
                GameObject lineToBeRemoved = connectionTypeToLineRenderer[(programmableEventType, programmableActionType)];
                connectionTypeToLineRenderer.Remove((programmableEventType, programmableActionType));
                Destroy(lineToBeRemoved);

                if (currentEventsActions[programmableEventType].Length <= 1)
                {
                    currentEventsActions.Remove(programmableEventType);
                }
                else
                {
                    List<ProgrammableActionType> actions = currentEventsActions[programmableEventType].ToList();
                    actions.Remove(programmableActionType);
                    currentEventsActions[programmableEventType] = actions.ToArray();
                }
            }
            else
            {
                List<ProgrammableActionType> actions = currentEventsActions[programmableEventType].ToList();
                actions.Add(programmableActionType);
                currentEventsActions[programmableEventType] = actions.ToArray();
                AddLineBetweenSelectedConnectors(programmableEventType, programmableActionType);
            }
        }
        else
        {
            currentEventsActions.Add(programmableEventType, new ProgrammableActionType[1] { programmableActionType });
            AddLineBetweenSelectedConnectors(programmableEventType, programmableActionType);
        }

        currentActionConnector = null;
        currentEventConnector = null;
    }

    private void AddLineBetweenSelectedConnectors(ProgrammableEventType programmableEventType, ProgrammableActionType programmableActionType)
    {
        Vector3[] lineVertices = new Vector3[4]
        {
            currentEventConnector.transform.position - new Vector3(0, 0, 90),
            currentEventConnector.transform.position + Vector3.right * bezierCurveInset - new Vector3(0, 0, 90),
            currentActionConnector.transform.position + Vector3.left * bezierCurveInset - new Vector3(0, 0, 90),
            currentActionConnector.transform.position - new Vector3(0, 0, 90)
        };

        GameObject newLine = Instantiate(lineRendererPrefab, currentEventConnector.transform.position, Quaternion.identity);
        connectionTypeToLineRenderer.Add((programmableEventType, programmableActionType), newLine);
        LineRenderer newLineRenderer = newLine.GetComponent<LineRenderer>();


        Color color = new(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        newLineRenderer.startColor = color;
        newLineRenderer.endColor = color;

        newLineRenderer.positionCount = 4;
        newLineRenderer.SetPositions(lineVertices);
    }

    public void SetCurrentEventsActions_ProgrammableObject1()
    {
        currentEventsActions = localObject1EventsActions;
        ReConnectOldConncections();
    }
    public void SetCurrentEventsActions_ProgrammableObject2()
    {
        currentEventsActions = localObject2EventsActions;
        ReConnectOldConncections();
    }
    public void SetCurrentEventsActions_ProgrammableEnemy() 
    {
        currentEventsActions = localEnemyEventsActions;
        ReConnectOldConncections();
    }

    private void ReConnectOldConncections()
    {
        foreach (KeyValuePair<(ProgrammableEventType, ProgrammableActionType), GameObject> keyValuePair in connectionTypeToLineRenderer)
        {
            Destroy(keyValuePair.Value);
        }
        connectionTypeToLineRenderer.Clear();

        foreach (KeyValuePair<ProgrammableEventType, ProgrammableActionType[]> keyValuePair in currentEventsActions)
        {
            foreach (ProgrammableActionType actionType in keyValuePair.Value)
            {
                currentEventConnector = eventConnectorButtons[keyValuePair.Key];
                currentActionConnector = actionConnectorButtons[actionType];

                AddLineBetweenSelectedConnectors(keyValuePair.Key, actionType);
            }
        }

        currentEventConnector = null;
        currentActionConnector = null;
    }

    public void ClearAllLines()
    {
        foreach (KeyValuePair<(ProgrammableEventType, ProgrammableActionType), GameObject> keyValuePair in connectionTypeToLineRenderer)
        {
            Destroy(keyValuePair.Value);
        }
        connectionTypeToLineRenderer.Clear();

        currentEventsActions.Clear();
    }

    public void DeveloperTurnEnd()
    {
        if (hasEnded) { return; }
        hasEnded = true;

        music.Stop();

        GameManager.Instance.SetProgrammableEnemyEventsActions(localEnemyEventsActions);
        GameManager.Instance.SetProgrammableObject1EventsActions(localObject1EventsActions);
        GameManager.Instance.SetProgrammableObject2EventsActions(localObject2EventsActions);
        GameManager.Instance.NextTurn();
    }
}

