using System.Collections.Generic;
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

    [SerializeField] private SerializableDictionary<Button, ProgrammableEventType> eventConnectors = new();
    [SerializeField] private SerializableDictionary<Button, ProgrammableActionType> actionConnectors = new();

    private Dictionary<ProgrammableEventType, ProgrammableActionType[]> localEnemyEventsActions;
    private Dictionary<ProgrammableEventType, ProgrammableActionType[]> localObject1EventsActions;
    private Dictionary<ProgrammableEventType, ProgrammableActionType[]> localObject2EventsActions;

    private Button currentEventConnector;
    private Button currentActionConnector;

    private void Awake()
    {
        localEnemyEventsActions = new Dictionary<ProgrammableEventType, ProgrammableActionType[]>();
        localObject1EventsActions = new Dictionary<ProgrammableEventType, ProgrammableActionType[]>();
        localObject2EventsActions = new Dictionary<ProgrammableEventType, ProgrammableActionType[]>();
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

        if (currentEventConnector != null && currentEventConnector != null)
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

        if (currentEventConnector != null && currentEventConnector != null)
        {
            ConnectEventToAction();
        }
    }

    private void ConnectEventToAction()
    {
        //Instantiate();
    }

    public void DeveloperTurnEnd()
    {
        GameManager.Instance.SetProgrammableEnemyEventsActions(localEnemyEventsActions);
        GameManager.Instance.SetProgrammableObject1EventsActions(localObject1EventsActions);
        GameManager.Instance.SetProgrammableObject2EventsActions(localObject2EventsActions);
        GameManager.Instance.NextTurn();
    }
}

