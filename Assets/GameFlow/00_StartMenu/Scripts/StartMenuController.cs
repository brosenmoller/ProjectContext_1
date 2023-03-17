using UnityEngine;

public class StartMenuController : MonoBehaviour
{
    public void Quit()
    {
        Application.Quit();
    }

    public void NextTurn()
    {
        GameManager.Instance.NextTurn();
    }
}

