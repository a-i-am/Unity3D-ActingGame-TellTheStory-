using NUnit.Framework;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int currentNPC;
    public int currentAct;
    public int[] npcCurrentLine;
    public int[] npcCurrentRole;
    public int[] npcFinished;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        instance = null;
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
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

        string[] words1 = Regex.Replace(dialogue1, @"[^가-힣a-zA-Z0-9\s]", "")
                                .ToLower()
                                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        string[] words2 = Regex.Replace(dialogue2, @"[^가-힣a-zA-Z0-9\s]", "")
                                .ToLower()
                                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);


        HashSet<string> uniqueWords = new HashSet<string>(words1);
        int matchingCount = 0;

        foreach (string word in words2)
        {
            if (uniqueWords.Contains(word))
            {
                matchingCount++;
            }
        }


        int maxWordCount = Mathf.Max(words1.Length, words2.Length);
        if (maxWordCount == 0) return 1.0f;


        float similarity = (float)matchingCount / maxWordCount;
        Debug.Log("Similarity : " + similarity);
        return similarity;
    }

    public void GoToHome()
    {
        SceneManager.LoadScene("Lobby");
    }

}
