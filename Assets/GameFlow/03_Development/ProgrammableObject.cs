using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProgrammableObject : MonoBehaviour
{
    private void Start()
    {
        Dictionary<ProgrammableEventType, ProgrammableActionType[]> tempDict = new()
        {
            { ProgrammableEventType.ON_PLAYER_COLLIDE, new ProgrammableActionType[] { ProgrammableActionType.RELOAD_SCENE } }
        };

        SetUpProgrammableEvents(tempDict);
    }

    private readonly Dictionary<ProgrammableActionType, Action> actionTypeToAction = new()
    {
        {
            ProgrammableActionType.RELOAD_SCENE, () =>
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        },
        {
            ProgrammableActionType.PLAYER_BIG_JUMP, () =>
            {
                
            }
        },
        {
            ProgrammableActionType.PLAYER_PLUS_ONE_HP, () =>
            {
                
            }
        },
        {
            ProgrammableActionType.PLAYER_MINUS_ONE_HP, () =>
            {
                
            }
        },
        {
            ProgrammableActionType.OBJECT_MOVE_FORWARD, () =>
            {
                
            }
        },
        {
            ProgrammableActionType.OBJECT_JUMP, () =>
            {
                
            }
        },
        {
            ProgrammableActionType.OBJECT_TURN_AROUND, () =>
            {
                
            }
        },
        {
            ProgrammableActionType.OBJECT_STOP_MOVING, () =>
            {
                
            }
        },
        {
            ProgrammableActionType.OBJECT_PAUSE_MOVING_3_SECONDS, () =>
            {
                
            }
        },
        {
            ProgrammableActionType.OBJECT_MOVE_DOWN, () =>
            {
                
            }
        },
        {
            ProgrammableActionType.OBJECT_MOVE_UP, () =>
            {
                
            }
        },
    };


    private readonly Dictionary<ProgrammableEventType, Action> localEventDictionary = new()
    {
        { ProgrammableEventType.ON_PLAYER_COLLIDE, null },
        { ProgrammableEventType.ON_PLAYER_IN_RANGE, null },
        { ProgrammableEventType.ON_COLLIDE, null },
    };

    private static readonly Dictionary<ProgrammableEventType, Action> staticEventDictionary = new()
    {
        { ProgrammableEventType.ON_START, null },
        { ProgrammableEventType.ON_PLAYER_JUMP, null },
        { ProgrammableEventType.ON_PLAYER_WALK, null },
        { ProgrammableEventType.ON_PlAYER_STOP, null },
        { ProgrammableEventType.EVERY_HALF_SECOND, null },
        { ProgrammableEventType.EVERY_3_SECONDS, null },
        { ProgrammableEventType.EVERY_10_SECONDS, null },
    };

    public void SetUpProgrammableEvents(Dictionary<ProgrammableEventType, ProgrammableActionType[]> developerActions)
    {
        foreach (ProgrammableEventType eventType in staticEventDictionary.Keys)
        {
            staticEventDictionary[eventType] = null;
        }

        foreach (KeyValuePair<ProgrammableEventType, ProgrammableActionType[]> developerAction in developerActions)
        {
            foreach (ProgrammableActionType actionType in developerAction.Value)
            {
                if (localEventDictionary.ContainsKey(developerAction.Key))
                {
                    if (GetActionFromActionType(actionType, out Action action))
                    {
                        localEventDictionary[developerAction.Key] -= action;
                        localEventDictionary[developerAction.Key] += action;
                    }
                }
                else if (staticEventDictionary.ContainsKey(developerAction.Key))
                {
                    if (GetActionFromActionType(actionType, out Action action))
                    {
                        staticEventDictionary[developerAction.Key] -= action;
                        staticEventDictionary[developerAction.Key] += action;
                    }
                }
                else
                {
                    Debug.LogWarning(developerAction.Key.ToString() + " is undefined in both of the event dictionaries");
                }
            }
        }
    }

    private bool GetActionFromActionType(ProgrammableActionType eventType, out Action action)
    {
        if (actionTypeToAction.ContainsKey(eventType))
        {
            action = actionTypeToAction[eventType];
            return true;
        }
        else
        {
            Debug.LogWarning(eventType.ToString() + " is undefined in actionTypeToAction Dictionary");
            action = null;
            return false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            InvokeEvent(ProgrammableEventType.ON_PLAYER_COLLIDE);
        }
    }

    public void InvokeEvent(ProgrammableEventType eventType)
    {
        if (localEventDictionary.ContainsKey(eventType))
        {
            localEventDictionary[eventType]?.Invoke();
        }
        else if (staticEventDictionary.ContainsKey(eventType))
        {
            staticEventDictionary[eventType]?.Invoke();
        }
        else
        { 
            Debug.LogWarning(eventType.ToString() + " is undefined in both of the event dictionaries");
        }
    }
}

