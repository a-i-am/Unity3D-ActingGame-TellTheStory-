using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayStoryManager : MonoBehaviour
{
    public static PlayStoryManager instance;
    private AudioSource audioSource;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        instance = null; // 씬 시작 전에 항상 초기화
    }

    private void Awake()
    {
        instance = this;
        audioSource = GetComponent<AudioSource>();
    }



    [ContextMenu("PlayStory")]
    public void PlayStoryTest()
    {
        PlayStory(0, 0);
    }
    public void PlayStory(int npcId, int actId)
    {
        SoundManager.instance.TurnOffBGM();
        AudioClip[] recordedClips = DataManager.instance.GetRecoredClips(npcId, actId);
        AudioClip[] npcClips = DataManager.instance.GetNPCClips(npcId, actId);
        Role startRole = GetStartRole(npcId, actId);
        StartCoroutine(PlayStoryCoroutine(recordedClips, npcClips, startRole));
    }
    private IEnumerator PlayStoryCoroutine(AudioClip[] recordedClips, AudioClip[] npcClips, Role startRole)
    {
        // 배열 인덱스 초기화
        int recordedIndex = 0;
        int npcIndex = 0;

        // 현재 재생 순서 결정
        bool isPlayerTurn = startRole == Role.Player;

        while (recordedIndex < recordedClips.Length || npcIndex < npcClips.Length)
        {
            if (isPlayerTurn && recordedIndex < recordedClips.Length)
            {
                // Player 역할의 클립 재생
                audioSource.clip = recordedClips[recordedIndex];
                audioSource.Play();
                yield return new WaitForSeconds(recordedClips[recordedIndex].length);
                recordedIndex++;
            }
            else if (!isPlayerTurn && npcIndex < npcClips.Length)
            {
                // NPC 역할의 클립 재생
                audioSource.clip = npcClips[npcIndex];
                audioSource.Play();
                yield return new WaitForSeconds(npcClips[npcIndex].length);
                npcIndex++;
            }

            // 턴을 변경
            isPlayerTurn = !isPlayerTurn;
        }

        Debug.Log("All clips have been played.");
    }

    private Role GetStartRole(int npcId, int actId)
    {
        // 파일 경로 설정
        string actingLineFilePath = $"Assets/Data/NPC{npcId}/Act{actId}.txt";

        // 파일 존재 여부 확인
        if (!File.Exists(actingLineFilePath))
        {
            Debug.LogError($"File not found: {actingLineFilePath}");
            return Role.Player; // 기본값으로 Role.Player 반환
        }

        // 첫 번째 줄 읽기
        string firstLine = File.ReadLines(actingLineFilePath).FirstOrDefault();

        if (string.IsNullOrEmpty(firstLine))
        {
            Debug.LogError("The file is empty or the first line is invalid.");
            return Role.Player; // 기본값으로 Role.Player 반환
        }

        // 첫 번째 줄을 ':' 기준으로 분리
        string[] parts = firstLine.Split(':');
        string roleStr = parts[0].Trim();

        // 문자열을 Role Enum으로 변환
        if (Enum.TryParse(roleStr, true, out Role role))
        {
            return role; // 성공적으로 변환되면 반환
        }
        else
        {
            Debug.LogError($"Invalid Role: {roleStr}. Defaulting to Role.Player.");
            return Role.Player; // 기본값으로 Role.Player 반환
        }
    }
    public void StopPlay()
    {
        SoundManager.instance.TurnOnBGM();
        audioSource.Stop();
    }
}
