using NUnit.Framework.Internal;
using System.Collections;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class ActingLineTriggerManager : MonoBehaviour
{
    public static ActingLineTriggerManager instance;

    [SerializeField] RecordManager recordManager;
    [SerializeField] ScoreManager scoreManager;
    [SerializeField] ActingLineData actingLineData;
    [SerializeField] ActingLineUI actingLineUI;
    [SerializeField] STTManager sttManager;
    [SerializeField] ActingLineSynchronizer synchronizer;
    [SerializeField] AudioSource audioSource;
    public Role currentRole;
    public int playerLineIndex = -1;
    public int npcLineIndex = -1;

    private float time_Max = 10f;

    private Coroutine npcClipCoroutine;
    private Coroutine remainTimeCoroutine;

    private bool isPlayingRecorded;
    private bool isActiveMic;
    private string currentLine;
    private string otherLine;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        instance = null;
    }

    private void Awake()
    {
        if (instance)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }
    void Start()
    {
        synchronizer.InitData();
        if (actingLineData == null)
        {
            Debug.LogError("ActingLineData가 할당되지 않았습니다!");
            return;
        }
        scoreManager.InitAll(actingLineData.playerActingLines.Length);

        recordManager.onSttResult += OnSttResult;
    }

    public void OnSttResult(string sttResult, AudioClip clip)
    {

        float score = GameManager.instance.CompareTwoDialogue(currentLine, sttResult);
        actingLineUI.UpdateSTTResult(sttResult);

        isPlayingRecorded = true;
        audioSource.PlayOneShot(clip);
        currentRole = Role.NPC;
        StartCoroutine(NextStep(clip.length, score, sttResult));
    }
    public IEnumerator NextStep(float second, float score, string sttResult)
    {
        yield return new WaitForSeconds(1f + second);
        recordManager.meshRenderer.enabled = false;
        isPlayingRecorded = false;

        if (score >= 0.3f)
        {
            scoreManager.ChangeScore(score, sttResult);
            ProceedToNextLine();
        }
        else
        {
            OnInaccurateSimilarity();
        }
    }



    public void OnRecordButtonClick()
    {
        SoundManager.instance.PlayButton(1);
        if (isActiveMic)
        {
            RecordManager.instance.StopRecording();
            StopCoroutine(remainTimeCoroutine);
            actingLineUI.UpdateTimerUI(0);
            isActiveMic = false;
        }
        else
        {
            RecordManager.instance.StartRecording(time_Max);
            actingLineUI.UpdateTimerUI(time_Max);
            remainTimeCoroutine = StartCoroutine(ShowRemainTimeCoroutine(time_Max));
            actingLineUI.sttText.text = string.Empty;
            isActiveMic = true;
        }
    }
    private IEnumerator ShowRemainTimeCoroutine(float time)
    {
        float remainingTime = time;

        while (remainingTime > 0)
        {

            actingLineUI.UpdateTimerUI(remainingTime);


            yield return null;


            remainingTime -= Time.deltaTime;
        }


        actingLineUI.UpdateTimerUI(0);


    }


    public void ProceedToNextLine()
    {
        if (actingLineData.npcActingLines.Length - 1 == npcLineIndex && actingLineData.playerActingLines.Length - 1 == playerLineIndex)
        {
            EndConversation();
            return;
        }
        actingLineUI.SetActiveByRole(currentRole);
        switch (currentRole)
        {
            case Role.NPC:
                NPCCase();
                break;
            case Role.Player:
                PlayerCase();
                break;
        }
    }
    private void EndConversation()
    {
        actingLineUI.ShowResultPanel();
        SoundManager.instance.PlayResult();
    }
    private void PlayerCase()
    {
        actingLineUI.UpdateTimerUI(time_Max);
        DataManager.instance.SaveCurrentData();
        playerLineIndex++;
        isActiveMic = false;
        currentLine = actingLineData.playerActingLines[playerLineIndex];
        if (currentLine.Contains('/'))
        {
            actingLineUI.timerPanel.SetActive(false);
            actingLineUI.UpdateUI(string.Empty, string.Empty);
            string[] splitted = currentLine.Split('/');
            currentLine = splitted[0];
            otherLine = splitted[1];
            actingLineUI.ShowChoices(currentLine, otherLine);
        }
        else
        {
            actingLineUI.timerPanel.SetActive(true);
            actingLineUI.UpdateUI(currentLine, actingLineData.playerPrompts[playerLineIndex]);
        }
    }

    private void NPCCase()
    {
        DataManager.instance.SaveCurrentData();
        npcLineIndex++;
        actingLineUI.UpdateUI(actingLineData.npcActingLines[npcLineIndex].dialogue, actingLineData.npcPrompts[npcLineIndex]);
        StartCoroutine(NPCClipCoroutine(actingLineData.npcActingLines[npcLineIndex].clip));
    }

    private IEnumerator NPCClipCoroutine(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
        yield return new WaitForSeconds(clip.length);
        currentRole = Role.Player;
        ProceedToNextLine();
    }
    public void SelectChoice(int choiceIndex)
    {
        actingLineUI.timerPanel.SetActive(true);
        string prompt;
        if (choiceIndex == 0)
        {
            otherLine = string.Empty;
            prompt = actingLineData.playerPrompts[playerLineIndex].Split('/')[0];
        }
        else
        {
            currentLine = otherLine;
            otherLine = string.Empty;
            prompt = actingLineData.playerPrompts[playerLineIndex].Split('/')[1];
        }
        actingLineUI.timerPanel.SetActive(true);
        actingLineUI.UpdateUI(currentLine, prompt);
        actingLineUI.choicePanel.SetActive(false);
    }
    private void OnInaccurateSimilarity()
    {
        recordManager.InitWaveformMesh();
        actingLineUI.UpdateTimerUI(time_Max);
        SoundManager.instance.PlayNeedRepeat();
    }
}
