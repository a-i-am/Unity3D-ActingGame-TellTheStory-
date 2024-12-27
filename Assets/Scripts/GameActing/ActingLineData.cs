using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;
using System.Linq;

[CreateAssetMenu(fileName = "ActingLineData", menuName = "Acting/ActingLineData", order = 1)]
public class ActingLineData : ScriptableObject
{
    // NPC 대사
    // 유저가 해야 할 대사
    // NPC와 유저 각각의 행동 지시문
    // 대사 문서 파일 경로
    public string actingLineFilePath;
    // 읽어온 파일 원본
    private string[] rawData;

    // 통합
    public List<string> allActingLines = new List<string>();
    
    // Player
    public List<string> playerActingLines = new List<string>();
    public List<string> playerPrompts = new List<string>();

    // NPC
    public List<LineSet> npcActingLines = new();
    public List<string> npcPrompts = new List<string>();

    // 대사 파일을 파싱하여 데이터를 설정하는 함수
    public void ParseActingLineFile()
    {
        if (string.IsNullOrEmpty(actingLineFilePath) || !File.Exists(actingLineFilePath))
        {
            Debug.LogError($"대사 파일이 존재하지 않습니다: {actingLineFilePath}");
            return;
        }
        
        InitLine(GetClips());



        Debug.Log("대사 파일 파싱 완료!");
    }
    private AudioClip[] GetClips()
    {
        AudioClip[] clips = Resources.LoadAll<AudioClip>($"NPC{GameManager.Instance.currentNPC}/Act{GameManager.Instance.currentAct}");
        return clips;
    }
    private void InitLine(AudioClip[] npcClips)
    {
        // 파일 내용을 읽어오기
        rawData = File.ReadAllLines(actingLineFilePath);
        UseAllLine(npcClips);
    }

    private void UseAllLine(AudioClip[] npcClips)
    {
        foreach (string line in rawData)
        {
            // 빈 줄은 무시
            if (string.IsNullOrWhiteSpace(line))
                continue;

            allActingLines.Add(line);
        }
    }




    // NPC, Player 대사 리스트
    //System.Collections.Generic.List<string> npcActingLinesList = new System.Collections.Generic.List<string>();
    //System.Collections.Generic.List<string> playerActingLinesList = new System.Collections.Generic.List<string>();
    //System.Collections.Generic.List<string> npcPromptsList = new System.Collections.Generic.List<string>();
    //System.Collections.Generic.List<string> playerPromptsList = new System.Collections.Generic.List<string>();




    //// 파싱된 데이터를 ActingLineData에 저장
    //allActingLines = allActingLinesList.ToArray();

    //playerActingLines = playerActingLinesList.ToArray();
    //playerPrompts = playerPromptsList.ToArray();

    //npcActingLines = npcActingLinesList.ToArray();
    //npcPrompts = npcPromptsList.ToArray();

    //Debug.Log("대사 파일 파싱 완료!");
}
[Serializable]
public class LineSet
{
    public string dialogue;
    public AudioClip clip;
    public LineSet(string dialogue, AudioClip clip)
    {
        this.dialogue = dialogue;
        this.clip = clip;
    }
}