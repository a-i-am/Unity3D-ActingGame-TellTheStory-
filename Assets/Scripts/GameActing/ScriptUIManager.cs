using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ScriptUIManager : MonoBehaviour
{
    // UI 요소
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI emotionPromptText;
    public TextMeshProUGUI scoreDisplayText;
    public Slider scoreProgressSlider;

    // 실시간 STT 대사 텍스트
    public TextMeshProUGUI realTimePlayerSpeechText;

    // 대사 데이터
    public ActingLineData actingLineData;
    private int currentLineIndex = 0;

    private float playerScore = 0;

    private string realTimeUserSpeech = "";

    //void Start()
    //{
    //    // 대사 타이밍 계산
    //    //lineData.SetDialogueTimings();
    //    StartCoroutine(DisplayDialogues());
    //}

    //// 대사를 시간에 맞춰 표시하는 코루틴
    //IEnumerator DisplayDialogues()
    //{
    //    for (int i = 0; i < actingLineData.npcActingLines.Length; i++)
    //    {
    //        // NPC 대사 표시
    //        UpdateDialogue(actingLineData.npcActingLines[i], actingLineData.npcEmotionPrompts[i]);

    //        // NPC 대사 표시 시간 대기
    //        yield return new WaitForSeconds(actingLineData.dialogueStartTimes[i]);

    //        // 플레이어 대사 표시 (지정된 대사)
    //        UpdateDialogue(actingLineData.playerActingLines[i], actingLineData.playerEmotionPrompts[i]);

    //        // 플레이어 STT 대사 수집 시간 대기
    //        yield return new WaitForSeconds(actingLineData.dialogueEndTimes[i] - actingLineData.dialogueStartTimes[i]);

    //        // 대사 종료 후 잠시 대기
    //        yield return new WaitForSeconds(1f);
    //    }
    //}

    //// 대사 및 감정 프롬프트 업데이트
    //public void UpdateDialogue(string dialogue, string emotionPrompt)
    //{
    //    dialogueText.text = dialogue;
    //    emotionPromptText.text = emotionPrompt;
    //}

    //// 실시간 STT 결과 받아오기
    //public void OnSTTRecognized(string sttResult)
    //{
    //    realTimeUserSpeech = sttResult;
    //    UpdateRealTimeSpeechDisplay(realTimeUserSpeech);
    //    AnalyzePlayerSpeech(realTimeUserSpeech);
    //}

    //// 실시간 STT 대사 UI 업데이트
    //public void UpdateRealTimeSpeechDisplay(string speech)
    //{
    //    realTimePlayerSpeechText.text = $"{speech}";
    //}

    //// 유저 대사 분석 (점수 계산)
    //public void AnalyzePlayerSpeech(string playerSpeech)
    //{
    //    float pronunciationScore = Random.Range(50, 100); // 발음 점수 예시
    //    float toneScore = Random.Range(50, 100);          // 톤 점수 예시
    //    float speedScore = Random.Range(50, 100);         // 속도 점수 예시

    //    float totalScore = (pronunciationScore * 0.4f) +
    //                       (toneScore * 0.3f) +
    //                       (speedScore * 0.3f);

    //    SetScore(totalScore);
    //}

    //// 점수 업데이트
    //public void SetScore(float score)
    //{
    //    playerScore = score;
    //    scoreDisplayText.text = $"Score: {playerScore:F1}";
    //    scoreProgressSlider.value = playerScore / 100f; // 0 ~ 1 슬라이더
    //}
}
