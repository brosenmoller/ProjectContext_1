using UnityEngine;

public class StartMenuController : MonoBehaviour
{
    [SerializeField] private AudioObject menuMuziek;

    private void Awake()
    {
        menuMuziek.Play();
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void NextTurn()
    {
        GameManager.Instance.NextTurn();
    }
}

