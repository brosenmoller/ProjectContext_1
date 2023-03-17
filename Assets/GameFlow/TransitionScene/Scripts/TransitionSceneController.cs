using TMPro;
using UnityEngine;

public class TransitionSceneController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI nextTurnText;

    private float timer = 0;

    private void Awake()
    {
        timer = 0;
        Debug.Log("Transition Scene");
    }

    private void Start()
    {
        playerNameText.text = GameManager.Instance.CurrentTurnData.player switch
        {
            Player.Player1 => GameManager.Instance.GameData.namePlayer1,
            Player.Player2 => GameManager.Instance.GameData.namePlayer2,
            Player.Player3 => GameManager.Instance.GameData.namePlayer3,
            _ => GameManager.Instance.GameData.namePlayer1,
        };

        nextTurnText.text = GameManager.Instance.CurrentTurnData.roomType.ToString();
    }

    private void FixedUpdate()
    {
        if (timer > GameManager.Instance.CurrentTurnData.timer)
        {
            OnTransitionSceneEnd();
        }
        else
        {
            timer += Time.fixedDeltaTime;
            timerText.text = ((int)GameManager.Instance.CurrentTurnData.timer - (int)timer).ToString();
        }
    }

    public void OnTransitionSceneEnd()
    {
        GameManager.Instance.NextTurn();
    }
}

