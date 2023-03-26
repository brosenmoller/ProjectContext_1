using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ThemeSelectionController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI pleaseChoseATheme;
    [SerializeField] private Button forestThemeButton;
    [SerializeField] private Button castleThemeButton;
    [SerializeField] private Button sciFiThemeButton;

    private Outline forestThemeButtonOutline;
    private Outline castleThemeButtonOutline;
    private Outline sciFiThemeButtonOutline;

    private GameTheme currentlySelectedGameTheme;
    private bool gameThemeAsigned = false;

    private void Awake()
    {
        forestThemeButton.onClick.AddListener(() => SetGameTheme(GameTheme.Forest));
        castleThemeButton.onClick.AddListener(() => SetGameTheme(GameTheme.Castle));
        sciFiThemeButton.onClick.AddListener(() => SetGameTheme(GameTheme.SciFi));

        forestThemeButtonOutline = forestThemeButton.GetComponent<Outline>();
        castleThemeButtonOutline = castleThemeButton.GetComponent<Outline>();
        sciFiThemeButtonOutline = sciFiThemeButton.GetComponent<Outline>();
    }

    public void SetGameTheme(GameTheme gameTheme)
    {
        currentlySelectedGameTheme = gameTheme;
        pleaseChoseATheme.enabled = false;
        gameThemeAsigned = true;
        
        switch (gameTheme)
        {
            case GameTheme.Forest:
                forestThemeButtonOutline.enabled = true;
                castleThemeButtonOutline.enabled = false;
                sciFiThemeButtonOutline.enabled = false;
                break;
            case GameTheme.Castle:
                forestThemeButtonOutline.enabled = false;
                castleThemeButtonOutline.enabled = true;
                sciFiThemeButtonOutline.enabled = false;
                break;
            case GameTheme.SciFi:
                forestThemeButtonOutline.enabled = false;
                castleThemeButtonOutline.enabled = false;
                sciFiThemeButtonOutline.enabled = true;
                break;
        }
    }

    public void Submit()
    {
        if (!gameThemeAsigned)
        {
            pleaseChoseATheme.enabled = true;
            return;
        }

        GameManager.Instance.SetGameTheme(currentlySelectedGameTheme);
        GameManager.Instance.NextTurn();
    }
}

