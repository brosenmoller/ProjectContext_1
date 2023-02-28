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
            { ProgrammableEventType.ON_PLAYER_COLLIDE, new ProgrammableActionType[] { ProgrammableActionType.SCENE_RELOAD } }
        };

        SetUpProgrammableEvents(tempDict);
    }

    public enum ProgrammableEventType
    {
        ON_PLAYER_COLLIDE = 0,
    }

    public enum ProgrammableActionType
    {
        SCENE_RELOAD = 0,
    }

    private readonly Dictionary<ProgrammableActionType, Action> actionTypeToAction = new()
    {
        { 
            ProgrammableActionType.SCENE_RELOAD, () => 
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    };


    private readonly Dictionary<ProgrammableEventType, Action> localEventDictionary = new()
    {
        { ProgrammableEventType.ON_PLAYER_COLLIDE, null },
    };

    private static readonly Dictionary<ProgrammableEventType, Action> staticEventDictionary = new()
    {

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

