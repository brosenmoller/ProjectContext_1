using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<T>();
                DontDestroyOnLoad(instance.gameObject);
            }

            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = GetComponent<T>();
            DontDestroyOnLoad(instance.gameObject);
        }
        else if (instance.GetInstanceID() != GetInstanceID())
        {
            Destroy(gameObject);
        }

        SingletonAwake();
    }

    protected virtual void SingletonAwake() { }
}
