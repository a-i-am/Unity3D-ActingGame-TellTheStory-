using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActingLineSynchronizer : MonoBehaviour
{
    public ActingLineData actingLineData;  // ActingLineData 참조
    private int currentActingLineIndex = 0;  // 대사 인덱스

    void Start()
    {
        if (actingLineData != null)
        {
            actingLineData.ParseActingLineFile();  // 대사 파일을 런타임에 파싱
            Debug.Log("Acting Line File Parsed at Runtime!");
        }
        else
        {
            Debug.LogError("ActingLineData is not assigned!");
        }

        // 대사와 행동지문 동기화 로직
        //SyncActingLinesWithSTT();
    }

    //// 대사와 행동지문 동기화
    public void SyncActingLinesWithSTT()
    {
        // 대사와 행동지문 동기화 로직
        //string npcActingLine = actingLineData.npcActingLines[currentActingLineIndex];
        //string playerActingLine = actingLineData.playerActingLines[currentActingLineIndex];
        //string npcPrompts = actingLineData.npcPrompts[currentActingLineIndex];
        //string playerPrompts = actingLineData.playerPrompts[currentActingLineIndex];
    }

    // STT 결과를 처리하는 함수
    public void OnSTTRecognized(string sttResult)
    {
        // STT 결과에 따른 대사 동기화 처리
        //string currentPlayerActingLine = actingLineData.playerActingLines[currentActingLineIndex];
    }
}

