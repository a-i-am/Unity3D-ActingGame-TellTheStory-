using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActingLineSynchronizer : MonoBehaviour
{
    public ActingLineData actingLineData;
    private int currentActingLineIndex = 0;

    public void InitData()
    {
        if (actingLineData != null)
        {
            actingLineData.actingLineFilePath = $"Assets/Data/NPC{GameManager.instance.currentNPC}/Act{GameManager.instance.currentAct}.txt";
            actingLineData.ParseActingLineFile();
            Debug.Log("Acting Line File Parsed at Runtime!");
        }
        else
        {
            Debug.LogError("ActingLineData is not assigned!");
        }

    }


    public void SyncActingLinesWithSTT()
    {

        string npcActingLine = actingLineData.npcActingLines[currentActingLineIndex].dialogue;
        string playerActingLine = actingLineData.playerActingLines[currentActingLineIndex];
        string npcPrompts = actingLineData.npcPrompts[currentActingLineIndex];
        string playerPrompts = actingLineData.playerPrompts[currentActingLineIndex];
    }


    public void OnSTTRecognized(string sttResult)
    {


    }
}

