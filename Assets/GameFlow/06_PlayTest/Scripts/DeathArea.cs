using UnityEngine;

public class DeathArea : MonoBehaviour
{
    private PlayTestController controller;

    private void Awake()
    {
        controller = FindObjectOfType<PlayTestController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            controller.ReloadScene();
        }
    }
}

