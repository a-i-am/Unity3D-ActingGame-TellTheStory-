using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;
using System.Linq;

[CreateAssetMenu(fileName = "ActingLineData", menuName = "Acting/ActingLineData", order = 1)]
public class ActingLineData : ScriptableObject
{




    public string actingLineFilePath;

    private string[] rawData;


    public string[] playerActingLines;
    public string[] playerPrompts;


    public LineSet[] npcActingLines;
    public string[] npcPrompts;


    public void ParseActingLineFile()
    {
        if (string.IsNullOrEmpty(actingLineFilePath) || !File.Exists(actingLineFilePath))
        {
            Debug.LogError($"대사 파일이 존재하지 않습니다: {actingLineFilePath}");
            return;
        }


        string[] rawData = File.ReadAllLines(actingLineFilePath);


        List<string> npcActingLinesList = new();
        List<string> playerActingLinesList = new();
        List<string> npcPromptsList = new();
        List<string> playerPromptsList = new();

        for (int i = 0; i < rawData.Length; i++)
        {
            string line = rawData[i];

            if (string.IsNullOrWhiteSpace(line))
                continue;

            string[] parts = line.Split(':');
            if (parts.Length < 2)
            {
                Debug.LogWarning($"잘못된 형식의 라인 발견: {line}");
                continue;
            }

            string role = parts[0].Trim();
            string dialogue = parts[1].Trim();

            if (i == 0)
            {
                if (Enum.TryParse(role, true, out Role roleEnum))
                {
                    ActingLineTriggerManager.instance.currentRole = roleEnum;
                }
            }


            string linePrompts = null;
            int promptsStartIndex = dialogue.IndexOf('(');
            int promptsEndIndex = dialogue.IndexOf(')');

            while (promptsStartIndex != -1 && promptsEndIndex != -1 && promptsEndIndex > promptsStartIndex)
            {

                string extractedPrompt = dialogue.Substring(promptsStartIndex, promptsEndIndex - promptsStartIndex + 1).Trim();
                linePrompts = string.IsNullOrEmpty(linePrompts)
                    ? extractedPrompt
                    : $"{linePrompts} / {extractedPrompt}";


                dialogue = dialogue.Remove(promptsStartIndex, promptsEndIndex - promptsStartIndex + 1).Trim();


                promptsStartIndex = dialogue.IndexOf('(');
                promptsEndIndex = dialogue.IndexOf(')');
            }


            if (role == "NPC")
            {
                npcActingLinesList.Add(dialogue);
                npcPromptsList.Add(linePrompts ?? "");
            }
            else if (role == "Player")
            {
                playerActingLinesList.Add(dialogue);
                playerPromptsList.Add(linePrompts ?? "");
            }
            else
            {
                Debug.LogWarning($"알 수 없는 역할: {role} - {line}");
            }
        }

        AudioClip[] clips = DataManager.instance.GetNPCClips(GameManager.instance.currentNPC, GameManager.instance.currentAct);
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
