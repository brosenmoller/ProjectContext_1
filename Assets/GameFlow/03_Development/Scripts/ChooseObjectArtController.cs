using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChooseObjectArtController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AudioObject music;
    [SerializeField] private RectTransform spriteTypeOptions;
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Lock References")]
    [SerializeField] private GameObject lockProgrammableObject1ArtChose;
    [SerializeField] private GameObject lockProgrammableObject2ArtChose;
    [Space(6)]
    [SerializeField] private GameObject programmableObject1ArtChoseUI;
    [SerializeField] private GameObject programmableObject2ArtChoseUI;

    [Header("RectTransform to SpriteType")]
    [SerializeField] private SerializableDictionary<RectTransform, ProgrammableObjectSpriteType> buttonToSpriteType = new();

    Dictionary<ProgrammableObjectSpriteType, RectTransform> spriteTypeToButton = new();

    private ProgrammableObjectSpriteTypeReference programmableObject1SpriteType;
    private ProgrammableObjectSpriteTypeReference programmableObject2SpriteType;

    private ProgrammableObjectSpriteTypeReference currentProgrammableObjectSpriteType;

    private Outline currentSelectionOutline;

    private float timer;
    private bool hasEnded;

    private void Start()
    {
        music.Play();

        foreach (SerializableKeyValuePair<RectTransform, ProgrammableObjectSpriteType> keyValuePair in buttonToSpriteType)
        {
            spriteTypeToButton.Add(keyValuePair.value, keyValuePair.key);
        }

        AssignSpriteTypes();
        ApplyArtLocks();
    }

    private void Update()
    {
        if (timer >= GameManager.Instance.CurrentTurnData.timer)
        {
            OnDevelopmentArtChooseTurnEnd();
        }
        else
        {
            timer += Time.deltaTime;
            timerText.text = ((int)GameManager.Instance.CurrentTurnData.timer - (int)timer).ToString();
        }
    }

    private void AssignSpriteTypes()
    {
        programmableObject1SpriteType = GameManager.Instance.GameData.programmableObject1SpriteType ??
            new ProgrammableObjectSpriteTypeReference(ProgrammableObjectSpriteType.JumpPad);

        programmableObject2SpriteType = GameManager.Instance.GameData.programmableObject2SpriteType ??
            new ProgrammableObjectSpriteTypeReference(ProgrammableObjectSpriteType.BuzzSaw);

        foreach (RectTransform child in spriteTypeOptions)
        {
            child.GetComponent<Button>().onClick.AddListener(() =>
            {
                SetProgrammableObjectSpriteType(buttonToSpriteType[child], child);
            });
        }
    }

    private void ApplyArtLocks()
    {
        lockProgrammableObject1ArtChose.SetActive(!GameManager.Instance.CurrentTurnData.programmableObject1Unlocked);
        lockProgrammableObject2ArtChose.SetActive(!GameManager.Instance.CurrentTurnData.programmableObject2Unlocked);

        switch (GameManager.Instance.CurrentTurnData.startingArtChoseTab)
        {
            case ArtChoseTabs.ProgrammableObject1:
                programmableObject1ArtChoseUI.SetActive(true);
                programmableObject2ArtChoseUI.SetActive(false);
                SetChoseObject1();
                break;
            case ArtChoseTabs.ProgrammableObject2:
                programmableObject1ArtChoseUI.SetActive(false);
                programmableObject2ArtChoseUI.SetActive(true);
                SetChoseObject2();
                break;
        }
    }

    public void SetChoseObject1()
    {
        currentProgrammableObjectSpriteType = programmableObject1SpriteType;
        RectTransform startButton = spriteTypeToButton[currentProgrammableObjectSpriteType.programmableObjectSpriteType];

        if (currentSelectionOutline != null) { currentSelectionOutline.enabled = false; }
        currentSelectionOutline = startButton.GetComponent<Outline>();
        currentSelectionOutline.enabled = true;
    }

    public void SetChoseObject2()
    {
        currentProgrammableObjectSpriteType = programmableObject2SpriteType;
        RectTransform startButton = spriteTypeToButton[currentProgrammableObjectSpriteType.programmableObjectSpriteType];

        if (currentSelectionOutline != null) { currentSelectionOutline.enabled = false; }
        currentSelectionOutline = startButton.GetComponent<Outline>();
        currentSelectionOutline.enabled = true;
    }

    private void SetProgrammableObjectSpriteType(ProgrammableObjectSpriteType spriteType, RectTransform rectTransform)
    {
        currentProgrammableObjectSpriteType.programmableObjectSpriteType = spriteType;

        if (currentSelectionOutline != null) { currentSelectionOutline.enabled = false; }
        currentSelectionOutline = rectTransform.GetComponent<Outline>();
        currentSelectionOutline.enabled = true;
    }

    public void OnDevelopmentArtChooseTurnEnd()
    {
        if (hasEnded) { return; }
        hasEnded = true;

        music.Stop();

        GameManager.Instance.SetProgrammableObject1SpriteType(programmableObject1SpriteType);
        GameManager.Instance.SetProgrammableObject2SpriteType(programmableObject2SpriteType);
        GameManager.Instance.NextTurn();
    }
}

