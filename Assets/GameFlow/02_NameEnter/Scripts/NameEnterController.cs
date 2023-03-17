using UnityEngine;
using TMPro;

public class NameEnterController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI pleaseEnterAllFieldsText;
    [SerializeField] private TMP_InputField player1NameInput;
    [SerializeField] private TMP_InputField player2NameInput;
    [SerializeField] private TMP_InputField player3NameInput;

    private string player1Name = "";
    private string player2Name = "";
    private string player3Name = "";

    private void Awake()
    {
        player1NameInput.onValueChanged.AddListener((string newName) => player1Name = newName);
        player2NameInput.onValueChanged.AddListener((string newName) => player2Name = newName);
        player3NameInput.onValueChanged.AddListener((string newName) => player3Name = newName);
    }

    public void Submit()
    {
        if (player1Name == "" || player2Name == "" || player3Name == "")
        {
            pleaseEnterAllFieldsText.enabled = true;
            return;
        }

        GameManager.Instance.SetNamePlayer1(player1Name);
        GameManager.Instance.SetNamePlayer2(player2Name);
        GameManager.Instance.SetNamePlayer3(player3Name);
        GameManager.Instance.NextTurn();
    }
}

