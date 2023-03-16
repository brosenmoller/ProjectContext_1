using System.Collections.Generic;
using System.Linq;
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
    OBJECT_JUMP = 5,
    OBJECT_TURN_AROUND = 6,
    OBJECT_STOP_MOVING = 7,
    OBJECT_PAUSE_MOVING_3_SECONDS = 8,
    OBJECT_MOVE_DOWN = 9,
    OBJECT_MOVE_UP = 10,
}

public class DeveloperController : MonoBehaviour
{
    #region Bezier Curves By BastianUrbach
    // https://answers.unity.com/questions/1835481/how-to-get-a-smooth-curved-line-between-two-points.html
    Vector2 Bezier(Vector2 a, Vector2 b, float t)
    {
        return Vector2.Lerp(a, b, t);
    }

    Vector2 Bezier(Vector2 a, Vector2 b, Vector2 c, float t)
    {
        return Vector2.Lerp(Bezier(a, b, t), Bezier(b, c, t), t);
    }

    Vector2 Bezier(Vector2 a, Vector2 b, Vector2 c, Vector2 d, float t)
    {
        return Vector2.Lerp(Bezier(a, b, c, t), Bezier(b, c, d, t), t);
    }
    #endregion

    #region SmoothCurve By CodeTastic
    // https://answers.unity.com/questions/392606/line-drawing-how-can-i-interpolate-between-points.html

    public static Vector3[] MakeSmoothCurve(Vector3[] arrayToCurve, int resolution)
    {
        List<Vector3> points;
        List<Vector3> curvedPoints;
        int pointsLength = 0;
        int curvedLength = 0;

        if (resolution < 1) resolution = 1;

        pointsLength = arrayToCurve.Length;

        curvedLength = (pointsLength * resolution) - 1;
        curvedPoints = new List<Vector3>(curvedLength);

        float t = 0.0f;
        for (int pointInTimeOnCurve = 0; pointInTimeOnCurve < curvedLength + 1; pointInTimeOnCurve++)
        {
            t = Mathf.InverseLerp(0, curvedLength, pointInTimeOnCurve);

            points = new List<Vector3>(arrayToCurve);

            for (int j = pointsLength - 1; j > 0; j--)
            {
                for (int i = 0; i < j; i++)
                {
                    points[i] = (1 - t) * points[i] + t * points[i + 1];
                }
            }

            curvedPoints.Add(points[0]);
        }

        return (curvedPoints.ToArray());
    }

    #endregion

    [Header("References")]
    [SerializeField] private GameObject lineRendererPrefab;
    
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

    private Dictionary<(ProgrammableEventType, ProgrammableActionType), GameObject> connectionTypeToLineRenderer = new();

    private void Awake()
    {
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

        SetCurrentEventsActions_ProgrammableObject1();
    }

    public void SetCurrentEventConnector(Button button)
    {
        if (currentEventConnector != button)
        {
            currentEventConnector = button;
        }
        else
        {
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
            currentActionConnector = button;
        }
        else
        {
            currentActionConnector = null;
        }

        if (currentEventConnector != null && currentActionConnector != null)
        {
            ConnectEventToAction();
        }
    }

    private void ConnectEventToAction()
    {
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

    public void DeveloperTurnEnd()
    {
        GameManager.Instance.SetProgrammableEnemyEventsActions(localEnemyEventsActions);
        GameManager.Instance.SetProgrammableObject1EventsActions(localObject1EventsActions);
        GameManager.Instance.SetProgrammableObject2EventsActions(localObject2EventsActions);
        GameManager.Instance.NextTurn();
    }
}

