using UnityEngine;

public class ActingLineTriggerManager : MonoBehaviour
{
    public RecordManager recordManager;
    public ActingLineData actingLineData;  // ActingLineData 참조
    public ActingLineUI actingLineUI;      // UI 관리
    public STTManager sttManager;          // STT 관리 (STTManager로 대체)
    private Role currentRole;   // 현재 턴의 역할 (NPC/Player)
    public int currentTurnIndex = 0;       // 현재 대사 인덱스

    public float time_Max = 3f;            // 타이머 최대 시간
    private float time_current;            // 현재 타이머 시간
    private bool isWaiting = false;

    void Start()
    {
        if (actingLineData == null)
        {
            Debug.LogError("ActingLineData가 할당되지 않았습니다!");
            return;
        }

        if (actingLineData.allActingLines.Count == 0)
        {
            Debug.LogError("ActingLineData가 비어있습니다. ParseActingLineFile을 호출했는지 확인하세요.");
            return;
        }

        // STT 결과 콜백 연결
        sttManager.onSttResult += HandleSttResult;

        UpdateTurn();
    }

    void Update()
    {
        if (isWaiting)
        {
            Check_Timer();
        }
    }

    // 현재 턴에 맞는 대사와 프롬프트를 UI에 업데이트
    public void UpdateTurn()
    {
        // ActingLineData 유효성 검증
        if (actingLineData == null)
        {
            Debug.LogError("ActingLineData가 설정되지 않았습니다!");
            return;
        }

        // 인덱스 범위 검증
        if (currentTurnIndex >= actingLineData.allActingLines.Count)
        {
            Debug.LogError($"currentTurnIndex({currentTurnIndex})가 대사 데이터 범위를 초과했습니다!");
            return;
        }

        foreach (string line in actingLineData.allActingLines)
        {
            string[] parts = line.Split(':');
            if (parts.Length < 2)
            {
                Debug.LogWarning($"잘못된 형식의 라인 발견: {line}");
                continue;
            }

            string role = parts[0].Trim();  // NPC 또는 Player
            string dialogue = parts[1].Trim();  // 대사

            // 지시문과 대사를 구분
            string linePrompts = null;
            int promptsStartIndex = dialogue.IndexOf('('); // '('의 위치
            int promptsEndIndex = dialogue.IndexOf(')');   // ')'의 위치

            if (promptsStartIndex != -1 && promptsEndIndex != -1 && promptsEndIndex > promptsStartIndex)
            {
                // 지시문 추출: '('부터 ')'까지의 내용 포함
                linePrompts = dialogue.Substring(promptsStartIndex, promptsEndIndex - promptsStartIndex + 1).Trim();

                // 대사 내용 갱신: '(' 이전과 ')' 이후를 결합
                string beforePrompt = dialogue.Substring(0, promptsStartIndex).Trim();
                string afterPrompt = dialogue.Substring(promptsEndIndex + 1).Trim();
                dialogue = string.IsNullOrEmpty(beforePrompt) ? afterPrompt : $"{beforePrompt} {afterPrompt}".Trim();
            }
        }
    }

    // STT 결과를 처리하여 대사 진행
    public void HandleSttResult(string sttResult)
    {
        // STT 결과를 UI에 타이핑 효과로 출력
        actingLineUI.UpdateSTTResult(sttResult);  // 프롬프트 없이 STT 결과만 UI에 출력

        Start_Timer();
    }

    // 녹음 duration 후, 대기시간 타이머 로직
    // 대기시간 타이머 시작
    private void Start_Timer()
    {
        time_current = time_Max;         // 최대 시간으로 초기화
        isWaiting = true;                // 타이머 활성화
        actingLineUI.UpdateTimerUI(time_current); // 초기 타이머 값 UI로 전달
    }

    private void Check_Timer()
    {
        if (time_current > 0)
        {
            time_current -= Time.deltaTime; // 시간 감소
            actingLineUI.UpdateTimerUI(time_current); // 타이머 값 갱신
        }
        else
        {
            End_Timer();
        }
    }

    private void End_Timer()
    {
        Debug.Log("타이머 종료");
        isWaiting = false;    // 타이머 비활성화
        ProceedToNextLine();  // 다음 대사 진행
    }

    // 대사 진행 (다음 대사로 넘어가기)
    private void ProceedToNextLine()
    {
        // 대사 진행
        currentTurnIndex++;

        // 모든 대사를 완료하면 종료
        if (currentTurnIndex >= actingLineData.npcActingLines.Count ||
            currentTurnIndex >= actingLineData.playerActingLines.Count)
        {
            Debug.Log("모든 대사를 완료했습니다!");
            return;
        }

        // 다음 대사로 진행
        UpdateTurn();
    }

    // 선택지 표시 (필요한 경우 구현)
    public void ShowChoices(string choice1, string choice2)
    {
        actingLineUI.ShowChoices(choice1, choice2);
    }
}
