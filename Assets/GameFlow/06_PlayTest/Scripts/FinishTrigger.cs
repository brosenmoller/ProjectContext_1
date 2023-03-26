using UnityEngine;

public class FinishTrigger : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float reloadDelay;
    [SerializeField] private ParticleSystem winParticles;

    private PlayTestController controller;
    private PlatformerMovement movement;

    private void Awake()
    {
        controller = FindObjectOfType<PlayTestController>();
        movement = FindObjectOfType<PlatformerMovement>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            movement.enabled = false;
            winParticles.Play();
            Invoke(nameof(ReloadScene), reloadDelay);
        }
    }

    private void ReloadScene()
    {
        controller.ReloadScene();
    }
}

