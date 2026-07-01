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
        instance = null;
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

        int recordedIndex = 0;
        int npcIndex = 0;


        bool isPlayerTurn = startRole == Role.Player;

        while (recordedIndex < recordedClips.Length || npcIndex < npcClips.Length)
        {
            if (isPlayerTurn && recordedIndex < recordedClips.Length)
            {

                audioSource.clip = recordedClips[recordedIndex];
                audioSource.Play();
                yield return new WaitForSeconds(recordedClips[recordedIndex].length);
                recordedIndex++;
            }
            else if (!isPlayerTurn && npcIndex < npcClips.Length)
            {

                audioSource.clip = npcClips[npcIndex];
                audioSource.Play();
                yield return new WaitForSeconds(npcClips[npcIndex].length);
                npcIndex++;
            }


            isPlayerTurn = !isPlayerTurn;
        }

        Debug.Log("All clips have been played.");
    }

    private Role GetStartRole(int npcId, int actId)
    {

        string actingLineFilePath = $"Assets/Data/NPC{npcId}/Act{actId}.txt";


        if (!File.Exists(actingLineFilePath))
        {
            Debug.LogError($"File not found: {actingLineFilePath}");
            return Role.Player;
        }


        string firstLine = File.ReadLines(actingLineFilePath).FirstOrDefault();

        if (string.IsNullOrEmpty(firstLine))
        {
            Debug.LogError("The file is empty or the first line is invalid.");
            return Role.Player;
        }


        string[] parts = firstLine.Split(':');
        string roleStr = parts[0].Trim();


        if (Enum.TryParse(roleStr, true, out Role role))
        {
            return role;
        }
        else
        {
            Debug.LogError($"Invalid Role: {roleStr}. Defaulting to Role.Player.");
            return Role.Player;
        }
    }
    public void StopPlay()
    {
        SoundManager.instance.TurnOnBGM();
        audioSource.Stop();
    }
}
