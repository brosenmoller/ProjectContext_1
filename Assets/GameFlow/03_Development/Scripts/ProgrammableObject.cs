using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProgrammableObject : MonoBehaviour
{
    [SerializeField] private LayerMask playerLayer;

    private GameObject player;
    private PlatformerMovement playerMovement;
    private Rigidbody2D playerRB;
    private Rigidbody2D rb;

    private bool movingForward = false;
    private bool flipped = false;
    private bool playerInRange = false;

    private bool movingVertical = false;
    private bool moveDown = false;
    //private bool gravityEnabled = false;

    //private float jumpTimer = 0;

    private void Start()
    {
        playerMovement = FindObjectOfType<PlatformerMovement>();
        player = playerMovement.gameObject;
        playerRB = player.GetComponent<Rigidbody2D>();
        rb = GetComponent<Rigidbody2D>();

        InvokeEvent(ProgrammableEventType.ON_START);
    }

    //private void EnableGravity()
    //{
    //    if (gravityEnabled) { return; }
    //    GetComponent<Collider2D>().isTrigger = false;
    //    rb.gravityScale = 4f;
    //    gravityEnabled = true;
    //}

    private void FixedUpdate()
    {
        if (movingForward)
        {
            float xVelocity = flipped ? -500f : 500f;
            rb.velocity = new Vector2(xVelocity * Time.deltaTime, rb.velocity.y);
        }

        if (movingVertical)
        {
            float yVelocity = moveDown ? -500f : 500f;
            rb.velocity = new Vector2(rb.velocity.x, yVelocity * Time.deltaTime);
        }

        CheckPlayerInRange();

        //if (GroundCheck() && jumpTimer < Time.time)
        //{
        //    rb.gravityScale = 0f;
        //    rb.velocity = new Vector2(rb.velocity.x, 0);
        //}
    }

    private void CheckPlayerInRange()
    {
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

    public bool GroundCheck()
    {
        if (Physics2D.Raycast(transform.position + playerMovement.colliderWidth, Vector2.down, playerMovement.groundDistance, playerMovement.groundLayer) ||
            Physics2D.Raycast(transform.position - playerMovement.colliderWidth, Vector2.down, playerMovement.groundDistance, playerMovement.groundLayer)) return true;
        else return false;
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
                thisObject.playerRB.velocity = new Vector2(thisObject.playerRB.velocity.x, 22f);
            }
        },
        {
            ProgrammableActionType.PLAYER_PLUS_ONE_HP, (ProgrammableObject thisObject) =>
            {
                // Give the Player plus one hp
            }
        },
        {
            ProgrammableActionType.PLAYER_MINUS_ONE_HP, (ProgrammableObject thisObject) =>
            {
                // Give the Player minus one hp
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
                //thisObject.rb.velocity = new Vector2(thisObject.rb.velocity.x, 18f);
                //thisObject.EnableGravity();
                ////thisObject.rb.gravityScale = 4f;
                ////thisObject.jumpTimer = Time.time + .15f;
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
                thisObject.movingVertical = false;
            }
        },
        {
            ProgrammableActionType.OBJECT_PAUSE_MOVING_3_SECONDS, (ProgrammableObject thisObject) =>
            {
                thisObject.PauseForThreeSeconds();
            }
        },
        {
            ProgrammableActionType.OBJECT_MOVE_DOWN, (ProgrammableObject thisObject) =>
            {
                thisObject.movingVertical = true;
                thisObject.moveDown = true;
            }
        },
        {
            ProgrammableActionType.OBJECT_MOVE_UP, (ProgrammableObject thisObject) =>
            {
                thisObject.movingVertical = true;
                thisObject.moveDown = false;
            }
        },
    };

    private void PauseForThreeSeconds()
    {
        movingForward = false;
        Invoke(nameof(ResumeMoving), 3f);
    }

    private void ResumeMoving()
    {
        movingForward = true;
    }


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
    //private void OnCollisionEnter2D(Collision2D other)
    //{
    //    if (other.gameObject.CompareTag("Player"))
    //    {
    //        InvokeEvent(ProgrammableEventType.ON_PLAYER_COLLIDE);
    //    }
    //    else
    //    {
    //        InvokeEvent(ProgrammableEventType.ON_COLLIDE);
    //    }
    //}

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

