using UnityEngine;

[CreateAssetMenu(fileName = "LineData", menuName = "Acting/LineData", order = 1)]
public class LineData : ScriptableObject
{
    // NPC의 여러 문장 대사
    public string[] npcDialogues;

    // 유저가 따라 할 여러 문장 대사
    public string[] playerDialogues;

    // NPC와 유저 각각의 감정 표현 지시문
    public string[] npcEmotionPrompts;  // NPC 대사의 감정 표현 지시문
    public string[] playerEmotionPrompts; // 유저 대사의 감정 표현 지시문

    // 각 대사에 대한 점수 기준 (각 문장에 대한 점수를 설정할 수 있음)
    public float[] requiredPronunciationScores;
    public float[] requiredToneScores;
    public float[] requiredSpeedScores;
}
