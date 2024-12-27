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

    // Player
    public string[] playerActingLines;
    public string[] playerPrompts;

    // NPC
    public LineSet[] npcActingLines;
    public string[] npcPrompts;

    // 대사 파일을 파싱하여 데이터를 설정하는 함수
    public void ParseActingLineFile()
    {
        if (string.IsNullOrEmpty(actingLineFilePath) || !File.Exists(actingLineFilePath))
        {
            Debug.LogError($"대사 파일이 존재하지 않습니다: {actingLineFilePath}");
            return;
        }

        // 파일 내용을 읽어오기
        string[] rawData = File.ReadAllLines(actingLineFilePath);

        // NPC 대사와 Player 대사를 담을 리스트
        List<string> npcActingLinesList = new();
        List<string> playerActingLinesList = new();
        List<string> npcPromptsList = new();
        List<string> playerPromptsList = new();

        for (int i = 0; i < rawData.Length; i++)
        {
            string line = rawData[i];
            // 빈 줄은 무시
            if (string.IsNullOrWhiteSpace(line))
                continue;

            string[] parts = line.Split(':');
            if (parts.Length < 2)
            {
                Debug.LogWarning($"잘못된 형식의 라인 발견: {line}");
                continue;
            }

            string role = parts[0].Trim();  // NPC 또는 Player
            string dialogue = parts[1].Trim();  // 대사

            if (i == 0)
            {
                if (Enum.TryParse(role, true, out Role roleEnum))
                {
                    ActingLineTriggerManager.instance.currentRole = roleEnum;
                }
            }

            // 지시문과 대사를 구분
            string linePrompts = null;
            int promptsStartIndex = dialogue.IndexOf('('); // '('의 위치
            int promptsEndIndex = dialogue.IndexOf(')');   // ')'의 위치

            while (promptsStartIndex != -1 && promptsEndIndex != -1 && promptsEndIndex > promptsStartIndex)
            {
                // 지시문 추출: '('부터 ')'까지의 내용 포함
                string extractedPrompt = dialogue.Substring(promptsStartIndex, promptsEndIndex - promptsStartIndex + 1).Trim();
                linePrompts = string.IsNullOrEmpty(linePrompts)
                    ? extractedPrompt
                    : $"{linePrompts} / {extractedPrompt}";

                // 대사 내용에서 지시문 제거
                dialogue = dialogue.Remove(promptsStartIndex, promptsEndIndex - promptsStartIndex + 1).Trim();

                // 다시 지시문 탐색
                promptsStartIndex = dialogue.IndexOf('(');
                promptsEndIndex = dialogue.IndexOf(')');
            }

            // 역할에 따른 대사 처리
            if (role == "NPC")
            {
                npcActingLinesList.Add(dialogue); // 슬래시가 포함된 대사를 그대로 저장
                npcPromptsList.Add(linePrompts ?? "");  // 지시문이 없는 경우 빈 문자열
            }
            else if (role == "Player")
            {
                playerActingLinesList.Add(dialogue); // 슬래시가 포함된 대사를 그대로 저장
                playerPromptsList.Add(linePrompts ?? "");  // 지시문이 없는 경우 빈 문자열
            }
            else
            {
                Debug.LogWarning($"알 수 없는 역할: {role} - {line}");
            }
        }

        AudioClip[] clips = GetClips();
        npcActingLines = new LineSet[npcActingLinesList.Count];
        for (int i = 0; i < npcActingLinesList.Count; i++)
        {
            npcActingLines[i] = new(npcActingLinesList[i], clips[i]);
        }
        playerActingLines = playerActingLinesList.ToArray();
        npcPrompts = npcPromptsList.ToArray();
        playerPrompts = playerPromptsList.ToArray();

        Debug.Log("대사 파일 파싱 완료!");
    }



    private AudioClip[] GetClips()
    {
        AudioClip[] clips = Resources.LoadAll<AudioClip>($"NPC{GameManager.Instance.currentNPC}/Act{GameManager.Instance.currentAct}");
        return clips;
    }
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