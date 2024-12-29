using NUnit.Framework;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public int currentNPC;
    public int currentAct;//NPC와 Act는 게임 씬에서 들어갔을 때 어떤 데이터를 불러올지 결정한다.
    public int[] npcCurrentLine;
    public int[] npcCurrentRole;
    public int[] npcFinished;
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
        npcCurrentLine = new int[4];
        npcCurrentRole = new int[4];
        npcFinished = new int[4];
    }
    public float CompareTwoDialogue(string dialogue1, string dialogue2)
    {
        // 1. 특수 문자 제거 및 공백으로 분리
        string[] words1 = Regex.Replace(dialogue1, @"[^가-힣a-zA-Z0-9\s]", "")
                                .ToLower()
                                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        string[] words2 = Regex.Replace(dialogue2, @"[^가-힣a-zA-Z0-9\s]", "")
                                .ToLower()
                                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        // 2. 단어 리스트 생성
        HashSet<string> uniqueWords = new HashSet<string>(words1);
        int matchingCount = 0;

        foreach (string word in words2)
        {
            if (uniqueWords.Contains(word))
            {
                matchingCount++;
            }
        }

        // 3. 최대 단어 수 계산
        int maxWordCount = Mathf.Max(words1.Length, words2.Length);
        if (maxWordCount == 0) return 1.0f; // 둘 다 비어있다면 100% 일치

        // 4. 일치도 계산
        float similarity = (float)matchingCount / maxWordCount;
        Debug.Log("Similarity : " + similarity);
        return similarity;
    }

}