using UnityEngine;

public class ActingLineTriggerManager : MonoBehaviour
{
    public ActingLineData actingLineData;  // ActingLineData 참조
    public ActingLineUI actingLineUI;      // UI 관리
    public STTManager sttManager;          // STT 관리
    public ScoreManager scoreManager;      // 점수 관리
    private Role currentRole = Role.NPC;   // 현재 턴의 역할 (NPC/Player)
    private int currentTurnIndex = 0;

    void Start()
    {
        if (actingLineData == null)
        {
            Debug.LogError("ActingLineData가 할당되지 않았습니다!");
            return;
        }

        if (actingLineData.npcActingLines.Length == 0 || actingLineData.playerActingLines.Length == 0)
        {
            Debug.LogError("ActingLineData가 비어있습니다. ParseActingLineFile을 호출했는지 확인하세요.");
            return;
        }

        UpdateTurn();
    }


    public void UpdateTurn()
    {
        // ActingLineData 유효성 검증
        if (actingLineData == null)
        {
            Debug.LogError("ActingLineData가 설정되지 않았습니다!");
            return;
        }

        // 인덱스 범위 검증
        if (currentTurnIndex >= actingLineData.npcActingLines.Length ||
            currentTurnIndex >= actingLineData.playerActingLines.Length)
        {
            Debug.LogError($"currentTurnIndex({currentTurnIndex})가 대사 데이터 범위를 초과했습니다!");
            return;
        }

        string line = "";
        string prompts = "";

        if (currentRole == Role.NPC)
        {
            line = actingLineData.npcActingLines[currentTurnIndex];
            prompts = actingLineData.npcPrompts[currentTurnIndex];
        }
        else if (currentRole == Role.Player)
        {
            line = actingLineData.playerActingLines[currentTurnIndex];
            prompts = actingLineData.playerPrompts[currentTurnIndex];
        }

        actingLineUI.UpdateUI(line, prompts);
    }



    public void HandlePlayerInput(string input)
    {
        // STT 결과 처리
        //sttManager.ProcessInput(input);

        // 다음 턴으로 이동
        currentTurnIndex++;

        if (currentTurnIndex >= actingLineData.npcActingLines.Length ||
            currentTurnIndex >= actingLineData.playerActingLines.Length)
        {
            Debug.Log("모든 대사를 완료했습니다!");
            return;
        }

        UpdateTurn();
    }

    public void ShowChoices(string choice1, string choice2)
    {
        // 선택지 표시
        actingLineUI.ShowChoices(choice1, choice2);
    }
}
