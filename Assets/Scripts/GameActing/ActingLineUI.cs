using System.Collections;
using TMPro;
using UnityEngine;

public class ActingLineUI : MonoBehaviour
{
    public TextMeshProUGUI actingLineText;  // 대사 UI 텍스트
    public TextMeshProUGUI promptText;     // 행동 지시문 텍스트
    public TextMeshProUGUI sttText;
    public TextMeshProUGUI timeMaxText;

    public GameObject choicePanel;          // 선택지 UI 패널
    public TextMeshProUGUI choice1Text;     // 선택지 1 텍스트
    public TextMeshProUGUI choice2Text;     // 선택지 2 텍스트

    // 대사와 감정 지시문을 UI에 업데이트하는 함수
    public void UpdateUI(string line, string linePrompts)
    {
        actingLineText.text = line;
        promptText.text = linePrompts;
    }

    // STT 결과를 UI에 타이핑 효과로 업데이트하는 함수
    public void UpdateSTTResult(string sttResult)
    {
        StartCoroutine(TypeText(sttResult, sttText));
    }

    // STT 결과를 타이핑 효과로 출력하는 코루틴
    private IEnumerator TypeText(string text, TextMeshProUGUI textComponent)
    {
        textComponent.text = "";  // 텍스트 초기화
        foreach (char letter in text.ToCharArray())
        {
            textComponent.text += letter;
            yield return new WaitForSeconds(0.05f);  // 타이핑 속도 조절
        }
    }

    // 선택지 표시
    public void ShowChoices(string choice1, string choice2)
    {
        choicePanel.SetActive(true);  // 선택지 패널 활성화
        choice1Text.text = choice1;   // 첫 번째 선택지 텍스트
        choice2Text.text = choice2;   // 두 번째 선택지 텍스트
    }

    // 선택지 숨기기
    public void HideChoices()
    {
        choicePanel.SetActive(false);  // 선택지 패널 비활성화
    }

    public void UpdateTimerUI(float time_current)
    {
        if (timeMaxText != null)
            timeMaxText.text = $"{time_current:N1}";
    }

}
