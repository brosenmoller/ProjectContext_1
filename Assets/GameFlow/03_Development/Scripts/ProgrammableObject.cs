using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProgrammableObject : MonoBehaviour
{
    [SerializeField] private LayerMask playerLayer;

    private GameObject player;
    private Rigidbody2D playerRB;
    private Rigidbody2D rb;

    private bool movingForward = false;
    private bool flipped = false;
    private bool playerInRange = false;

    private void Start()
    {
        player = FindObjectOfType<PlatformerMovement>().gameObject;
        playerRB = player.GetComponent<Rigidbody2D>();
        rb = GetComponent<Rigidbody2D>();

        InvokeEvent(ProgrammableEventType.ON_START);
    }

    private void Update()
    {
        if (movingForward)
        {
            float xVelocity = flipped ? -300f : 300f;
            rb.velocity = new Vector2(xVelocity * Time.deltaTime, rb.velocity.y);
        }

        RaycastHit2D hit = Physics2D.Raycast(transform.position, player.transform.position - transform.position, 5f, playerLayer);
        if (hit)
        {
            if (!playerInRange)
            {
                InvokeEvent(ProgrammableEventType.ON_PLAYER_IN_RANGE);
                playerInRange = true;
            }
        }
        else
        {
            playerInRange = false;
        }
    }

    private readonly Dictionary<ProgrammableActionType, Action<ProgrammableObject>> actionTypeToAction = new()
    {
        {
            ProgrammableActionType.RELOAD_SCENE, (ProgrammableObject thisObject) =>
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        },
        {
            ProgrammableActionType.PLAYER_BIG_JUMP, (ProgrammableObject thisObject) =>
            {
                thisObject.playerRB.velocity = new Vector2(thisObject.playerRB.velocity.x, 25f);
            }
        },
        {
            ProgrammableActionType.PLAYER_PLUS_ONE_HP, (ProgrammableObject thisObject) =>
            {
                
            }
        },
        {
            ProgrammableActionType.PLAYER_MINUS_ONE_HP, (ProgrammableObject thisObject) =>
            {
                
            }
        },
        {
            ProgrammableActionType.OBJECT_MOVE_FORWARD, (ProgrammableObject thisObject) =>
            {
                thisObject.movingForward = true;
            }
        },
        {
            ProgrammableActionType.OBJECT_JUMP, (ProgrammableObject thisObject) =>
            {
                thisObject.rb.velocity = new Vector2(thisObject.rb.velocity.x, 18f);
            }
        },
        {
            ProgrammableActionType.OBJECT_TURN_AROUND, (ProgrammableObject thisObject) =>
            {
                if (thisObject.flipped)
                {
                    thisObject.transform.rotation = Quaternion.Euler(0, 0, 0);
                }
                else
                {
                    thisObject.transform.rotation = Quaternion.Euler(0, 180, 0);
                }

                thisObject.flipped = !thisObject.flipped;
            }
        },
        {
            ProgrammableActionType.OBJECT_STOP_MOVING, (ProgrammableObject thisObject)    =>
            {
                thisObject.movingForward = false;
            }
        },
        {
            ProgrammableActionType.OBJECT_PAUSE_MOVING_3_SECONDS, (ProgrammableObject thisObject) =>
            {
                thisObject.movingForward = false;
                new Timer(3, () => thisObject.movingForward = true);
            }
        },
        {
            ProgrammableActionType.OBJECT_MOVE_DOWN, (ProgrammableObject thisObject) =>
            {
                
            }
        },
        {
            ProgrammableActionType.OBJECT_MOVE_UP, (ProgrammableObject thisObject) =>
            {
                
            }
        },
    };


    private readonly Dictionary<ProgrammableEventType, Action<ProgrammableObject>> eventDictionary = new()
    {
        // Local
        { ProgrammableEventType.ON_PLAYER_COLLIDE, null },
        { ProgrammableEventType.ON_PLAYER_IN_RANGE, null },
        { ProgrammableEventType.ON_COLLIDE, null },

        // Global
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
        foreach (KeyValuePair<ProgrammableEventType, ProgrammableActionType[]> developerAction in developerActions)
        {
            foreach (ProgrammableActionType actionType in developerAction.Value)
            {
                if (eventDictionary.ContainsKey(developerAction.Key))
                {
                    if (GetActionFromActionType(actionType, out Action<ProgrammableObject> action))
                    {
                        eventDictionary[developerAction.Key] -= action;
                        eventDictionary[developerAction.Key] += action;
                    }
                }
                else
                {
                    Debug.LogWarning(developerAction.Key.ToString() + " is undefined in the event dictionary");
                }
            }
        }
    }

    private bool GetActionFromActionType(ProgrammableActionType actionType, out Action<ProgrammableObject> action)
    {
        if (actionTypeToAction.ContainsKey(actionType))
        {
            action = actionTypeToAction[actionType];
            return true;
        }
        else
        {
            Debug.LogWarning(actionType.ToString() + " is undefined in actionTypeToAction Dictionary");
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
        else
        {
            InvokeEvent(ProgrammableEventType.ON_COLLIDE);
        }
    }

    public void InvokeEvent(ProgrammableEventType eventType)
    {
        if (eventDictionary.ContainsKey(eventType))
        {
            eventDictionary[eventType]?.Invoke(this);
        }
        else
        {
            Debug.LogWarning(eventType.ToString() + " is undefined in the event dictionary");
        }
    }
}

