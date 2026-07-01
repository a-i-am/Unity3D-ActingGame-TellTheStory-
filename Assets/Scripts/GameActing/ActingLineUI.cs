using System.Collections;
using TMPro;
using UnityEngine;

public class ActingLineUI : MonoBehaviour
{
    public TextMeshProUGUI actingLineText;
    public TextMeshProUGUI promptText;
    public TextMeshProUGUI sttText;
    public TextMeshProUGUI remainTimeText;
    public GameObject scorePanel;

    public GameObject choicePanel;
    public TextMeshProUGUI choice1Text;
    public TextMeshProUGUI choice2Text;

    public GameObject timerPanel;

    [SerializeField] EntrancePanel entrancePanel;
    [SerializeField] ResultPanel resultPanel;
    private Coroutine updateSTTResult;

    private void Start()
    {
        entrancePanel.gameObject.SetActive(true);
        resultPanel.gameObject.SetActive(false);
        actingLineText.gameObject.SetActive(false);
        promptText.gameObject.SetActive(false);
        sttText.gameObject.SetActive(false);
        remainTimeText.gameObject.SetActive(false);
        choicePanel.SetActive(false);
        timerPanel.SetActive(false);
        scorePanel.SetActive(false);
    }
    public void OnStartButtonClick()
    {
        actingLineText.gameObject.SetActive(true);
        promptText.gameObject.SetActive(true);
        scorePanel.SetActive(true);
        remainTimeText.gameObject.SetActive(true);
        SoundManager.instance.PlayButton(0);
    }

    public void UpdateUI(string line, string linePrompts)
    {
        actingLineText.text = line;
        promptText.text = linePrompts;
    }


    public void UpdateSTTResult(string sttResult)
    {
        if (updateSTTResult != null)
            StopCoroutine(updateSTTResult);
        updateSTTResult =  StartCoroutine(TypeText(sttResult, sttText));
    }


    private IEnumerator TypeText(string text, TextMeshProUGUI textComponent)
    {
        textComponent.text = "";
        foreach (char letter in text.ToCharArray())
        {
            textComponent.text += letter;
            yield return new WaitForSeconds(0.05f);
        }
        updateSTTResult = null;
    }


    public void ShowChoices(string choice1, string choice2)
    {
        choicePanel.SetActive(true);
        choice1Text.text = choice1;
        choice2Text.text = choice2;
    }


    public void HideChoices()
    {
        choicePanel.SetActive(false);
    }

    public void UpdateTimerUI(float time_current)
    {
        remainTimeText.text = $"{time_current:N0}";
    }
    public void SetActiveByRole(Role currentRole)
    {
        choicePanel.SetActive(false);
        switch (currentRole)
        {
            case Role.NPC:
                timerPanel.SetActive(false);
                promptText.gameObject.SetActive(false);
                sttText.gameObject.SetActive(false);
                break;
            case Role.Player:
                sttText.gameObject.SetActive(true);
                sttText.text = string.Empty;
                promptText.gameObject.SetActive(true);
                break;
        }
    }
    public void ShowResultPanel()
    {
        resultPanel.gameObject.SetActive(true);
        ScoreManager.instance.GetResult(out float score, out string accurateStr, out string inaccurateStr);
        resultPanel.SetResult(score, accurateStr, inaccurateStr);
    }
}
