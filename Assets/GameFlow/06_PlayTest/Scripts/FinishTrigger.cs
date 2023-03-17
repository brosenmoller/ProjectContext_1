using UnityEngine;

public class FinishTrigger : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float reloadDelay;

    private PlayTestController controller;

    private void Awake()
    {
        controller = FindObjectOfType<PlayTestController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Invoke(nameof(ReloadScene), reloadDelay);
        }
    }

    private void ReloadScene()
    {
        controller.ReloadScene();
    }
}

