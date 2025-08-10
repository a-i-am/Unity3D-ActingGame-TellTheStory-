using NUnit.Framework.Internal;
using System.Collections;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class ActingLineTriggerManager : MonoBehaviour
{
    public static ActingLineTriggerManager instance;

    [SerializeField] RecordManager recordManager;
    [SerializeField] ScoreManager scoreManager;
    [SerializeField] ActingLineData actingLineData;  // ActingLineData 참조
    [SerializeField] ActingLineUI actingLineUI;      // UI 관리
    [SerializeField] STTManager sttManager;          // STT 관리 (STTManager로 대체)
    [SerializeField] ActingLineSynchronizer synchronizer;
    [SerializeField] AudioSource audioSource;
    public Role currentRole;   // 현재 턴의 역할 (NPC/Player)
    public int playerLineIndex = -1;       // 현재 대사 인덱스
    public int npcLineIndex = -1;       // 현재 대사 인덱스

    private float time_Max = 10f;            // 타이머 최대 시간

    private Coroutine npcClipCoroutine;
    private Coroutine remainTimeCoroutine;

    private bool isPlayingRecorded;
    private bool isActiveMic;
    private string currentLine;
    private string otherLine;

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
        // STT 결과 콜백 연결
        recordManager.onSttResult += OnSttResult;
    }
    // STT 결과를 처리하여 대사 진행
    public void OnSttResult(string sttResult, AudioClip clip)
    {
        // STT 결과를 UI에 타이핑 효과로 출력
        float score = GameManager.Instance.CompareTwoDialogue(currentLine, sttResult);
        actingLineUI.UpdateSTTResult(sttResult);
        //Clip
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


    //녹음 버튼을 눌렀을 때 호출할 메서드
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
            actingLineUI.UpdateTimerUI(time_Max); // 초기 타이머 값 UI로 전달
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
            // UI 업데이트
            actingLineUI.UpdateTimerUI(remainingTime);

            // 한 프레임 대기
            yield return null;

            // 경과 시간 계산
            remainingTime -= Time.deltaTime;
        }

        // 시간이 다 지나면 0으로 업데이트
        actingLineUI.UpdateTimerUI(0);

        // 코루틴 종료
    }

    // 대사 진행 (다음 대사로 넘어가기)
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