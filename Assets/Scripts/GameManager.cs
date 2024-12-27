using NUnit.Framework;
using System.Text.RegularExpressions;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
<<<<<<< HEAD:Assets/Scripts/GameManager.cs
    public int currentNpc;
    public int currentDialogueIndex;
=======
    public int currentNPC;
    public int currentAct;//NPC와 Act는 게임 씬에서 들어갔을 때 어떤 데이터를 불러올지 결정한다.
    public int[] npcCurrentLine = new int[4];//각 npc의 이야기 진행 상황. 세이브, 로드 해야함.
>>>>>>> a0b00ea (오디오 클립 세팅):Assets/Scripts/RhythmGame/GameManager.cs
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public float CompareTwoDialogue(string dialogue1, string dialogue2)
    {
        // 1. 특수 문자와 공백 제거
        string cleanedDialogue1 = Regex.Replace(dialogue1, @"[^가-힣a-zA-Z0-9]", "").ToLower();
        string cleanedDialogue2 = Regex.Replace(dialogue2, @"[^가-힣a-zA-Z0-9]", "").ToLower();

        // 2. 길이 확인
        int maxLength = Mathf.Max(cleanedDialogue1.Length, cleanedDialogue2.Length);

        if (maxLength == 0) return 1.0f; // 둘 다 비어있다면 100% 일치

        // 3. 순서 기반 비교
        int matchingCount = 0;
        for (int i = 0; i < Mathf.Min(cleanedDialogue1.Length, cleanedDialogue2.Length); i++)
        {
            if (cleanedDialogue1[i] == cleanedDialogue2[i]) // 위치가 동일한 글자만 매칭
            {
                matchingCount++;
            }
        }

        // 4. 일치도 계산
        float similarity = (float)matchingCount / maxLength;
        return similarity;
    }
}