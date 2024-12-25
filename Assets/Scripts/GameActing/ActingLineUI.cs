using UnityEngine;
using TMPro;
public class ActingLineUI : MonoBehaviour
{
    public TextMeshProUGUI actingLineText;  // 대사 UI 텍스트
    public TextMeshProUGUI promptText;     // 행동 지시문 텍스트
    public GameObject choicePanel;          // 선택지 UI 패널
    public TextMeshProUGUI choice1Text;     // 선택지 1 텍스트
    public TextMeshProUGUI choice2Text;     // 선택지 2 텍스트

    // 대사와 감정 지시문을 UI에 업데이트하는 함수
    public void UpdateUI(string line, string prompts)
    {
        actingLineText.text = line;
        promptText.text = prompts;
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
}
