using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScriptUIManager : MonoBehaviour
{
    // UI 요소
    public TextMeshProUGUI npcLineText;        // NPC 대사 텍스트
    public TextMeshProUGUI playerLineText;     // 유저 대사 텍스트
    public TextMeshProUGUI npcEmotionPromptText;  // NPC 감정 표현 지시문
    public TextMeshProUGUI playerEmotionPromptText;  // 유저 감정 표현 지시문
    public TextMeshProUGUI scoreDisplayText;   // 점수 텍스트
    public Slider scoreProgressSlider;         // 점수 슬라이더

    // 유저의 STT 대사 실시간 표시
    public TextMeshProUGUI realTimePlayerSpeechText; // 실시간 STT 대사 텍스트

    // 대사 데이터
    public LineData lineData;                  // 여러 문장을 포함한 대사 데이터
    private int currentLineIndex = 0;           // 현재 문장의 인덱스

    private float playerScore = 0; // 유저의 현재 점수

    // 실시간 STT 대사
    private string realTimeUserSpeech = ""; // 실시간으로 유저가 입력한 대사

    void Start()
    {
        LoadLineData();
    }

    // 대사 데이터 로드 및 UI 설정
    public void LoadLineData()
    {
        if (lineData != null && lineData.npcDialogues.Length > 0)
        {
            SetNPCLine(lineData.npcDialogues[currentLineIndex]);
            SetPlayerLine(lineData.playerDialogues[currentLineIndex]);
            SetNpcEmotionPrompt(lineData.npcEmotionPrompts[currentLineIndex]);
            SetPlayerEmotionPrompt(lineData.playerEmotionPrompts[currentLineIndex]);
            SetScore(0);
        }
    }

    // NPC 대사를 업데이트하는 함수
    public void SetNPCLine(string line)
    {
        npcLineText.text = line;
    }

    // 유저 대사를 업데이트하는 함수
    public void SetPlayerLine(string playerSpeech)
    {
        playerLineText.text = $"You said: {playerSpeech}";
    }

    // NPC 감정 표현 지시문을 업데이트하는 함수
    public void SetNpcEmotionPrompt(string prompt)
    {
        npcEmotionPromptText.text = prompt;
    }

    // 유저 감정 표현 지시문을 업데이트하는 함수
    public void SetPlayerEmotionPrompt(string prompt)
    {
        playerEmotionPromptText.text = prompt;
    }

    // 점수를 업데이트하는 함수
    public void SetScore(float score)
    {
        playerScore = score;
        scoreDisplayText.text = $"Score: {playerScore:F1}";
        scoreProgressSlider.value = playerScore / 100f; // 슬라이더 값은 0~1로 정규화
    }

    // 유저 대사 분석 결과와 점수를 실시간으로 업데이트
    public void AnalyzePlayerSpeech(string playerSpeech)
    {
        // 예시: 점수 계산
        float pronunciationScore = Random.Range(50, 100); // 가상의 발음 점수
        float toneScore = Random.Range(50, 100);          // 가상의 톤 점수
        float speedScore = Random.Range(50, 100);         // 가상의 속도 점수

        float totalScore = (pronunciationScore * 0.4f) +
                           (toneScore * 0.3f) +
                           (speedScore * 0.3f);

        // UI 업데이트
        SetPlayerLine(playerSpeech);
        SetScore(totalScore);
    }

    // STT 대사 받아오기 (실시간 대사 처리)
    public void OnSTTRecognized(string sttResult)
    {
        realTimeUserSpeech = sttResult;
        UpdateRealTimeSpeechDisplay(realTimeUserSpeech);
        AnalyzePlayerSpeech(realTimeUserSpeech);
    }

    // 실시간으로 유저의 STT 대사를 UI에 표시
    public void UpdateRealTimeSpeechDisplay(string speech)
    {
        realTimePlayerSpeechText.text = $"Your Speech: {speech}";
    }

    // 대사 진행 후 다음 문장으로 이동
    public void NextLine()
    {
        if (currentLineIndex < lineData.npcDialogues.Length - 1)
        {
            currentLineIndex++;
            LoadLineData();
        }
    }

    // 녹음 시작 및 STT 처리
    public void StartRecording()
    {
        // 예시: STT 통합을 위한 녹음 처리
        string mockPlayerSpeech = lineData.playerDialogues[currentLineIndex]; // 테스트용 대사
        OnSTTRecognized(mockPlayerSpeech); // STT 결과를 바로 처리
    }
}
