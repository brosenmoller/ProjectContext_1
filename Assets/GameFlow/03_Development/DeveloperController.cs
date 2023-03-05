using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ProgrammableEventType
{
    ON_PLAYER_COLLIDE = 0,
    ON_PLAYER_JUMP = 1,
    ON_PLAYER_WALK = 2,

}

public enum ProgrammableActionType
{
    SCENE_RELOAD = 0,
    SET_PLAYER_JUMP_FORCE = 1,
    SET_PLAYER_MOVE_RIGHT_FORCE = 1,
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

    private Button currentEventConnector;
    private Button currentActionConnector;

    private void Awake()
    {

    }

}

