using UnityEngine;
using System.IO;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ActingLineData", menuName = "Acting/ActingLineData", order = 1)]
public class ActingLineData : ScriptableObject
{
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
    public List<string> npcActingLines = new List<string>();
    public List<string> npcPrompts = new List<string>();



    // 대사 파일을 파싱하여 데이터를 설정하는 함수
    public void ParseActingLineFile()
    {
        if (string.IsNullOrEmpty(actingLineFilePath) || !File.Exists(actingLineFilePath))
        {
            Debug.LogError($"대사 파일이 존재하지 않습니다: {actingLineFilePath}");
            return;
        }

        // 파일 내용을 읽어오기
        rawData = File.ReadAllLines(actingLineFilePath);
        UseAllLine();
    }

    private void UseAllLine()
    {
        foreach (string line in rawData)
        {
            // 빈 줄은 무시
            if (string.IsNullOrWhiteSpace(line))
                continue;

            allActingLines.Add(line);
        }
        LineSeparator();
    }

    public void LineSeparator()
    {
        foreach (var line in allActingLines)
        {
            string[] parts = line.Split(':');
            if (parts.Length < 2)
            {
                Debug.LogWarning($"잘못된 형식의 라인 발견: {line}");
                continue;
            }

            string role = parts[0].Trim();  // NPC 또는 Player
            string dialogue = parts[1].Trim();  // 대사

            // 지시문과 대사를 구분
            string linePrompts = null;
            int promptsStartIndex = dialogue.IndexOf('('); // '('의 위치
            int promptsEndIndex = dialogue.IndexOf(')');   // ')'의 위치

            if (promptsStartIndex != -1 && promptsEndIndex != -1 && promptsEndIndex > promptsStartIndex)
            {
                // 지시문 추출: '('부터 ')'까지의 내용 포함
                linePrompts = dialogue.Substring(promptsStartIndex, promptsEndIndex - promptsStartIndex + 1).Trim();

                // 대사 내용 갱신: '(' 이전과 ')' 이후를 결합
                string beforePrompt = dialogue.Substring(0, promptsStartIndex).Trim();
                string afterPrompt = dialogue.Substring(promptsEndIndex + 1).Trim();
                dialogue = string.IsNullOrEmpty(beforePrompt) ? afterPrompt : $"{beforePrompt} {afterPrompt}".Trim();
            }

            // 역할에 따른 순열 저장
            if (role == "NPC")
            {
                npcActingLines.Add(dialogue);
                npcPrompts.Add(linePrompts ?? "");  // 지시문이 없는 경우 빈 문자열
            }
            else if (role == "Player")
            {
                playerActingLines.Add(dialogue);
                playerPrompts.Add(linePrompts ?? "");  // 지시문이 없는 경우 빈 문자열
            }
            else
            {
                Debug.LogWarning($"알 수 없는 역할: {role} - {dialogue}");
            }
        }
    }
}
