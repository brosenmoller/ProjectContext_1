using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private StateMachine<GameManager> stateMachine;

    public Controls controls;
    
    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(instance);
        }
    }

    private void OnEnable()
    {
        controls = new Controls();
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    public void Start()
    {
        stateMachine = new StateMachine<GameManager>(
            this
            //new State<GameManager>(
        );
    }
}

