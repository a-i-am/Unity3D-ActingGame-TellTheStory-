using System;
using UnityEngine;

public class STTManager : MonoBehaviour
{
    public Action<string> onSttResult; // STT 결과 콜백

    public RecordManager recordManager; // RecordManager 참조
    public ActingLineTriggerManager actingLineTriggerManager; // ActingLineTriggerManager 참조

    // STT 결과를 처리하여 대사 진행
    private void Awake()
    {
        if (recordManager != null)
        {
            // RecordManager의 STT 결과 콜백을 STTManager의 HandleSttResult로 연결
            recordManager.onSttResult += HandleSttResult;
        }
    }

    // STT 결과를 처리하는 콜백 함수
    private void HandleSttResult(string sttResult)
    {
        // 대사 넘김 판정 ㅡ ActingLineTriggerManager
        if (actingLineTriggerManager != null)
        {
            // STT 결과를 ActingLineTriggerManager로 전달
            actingLineTriggerManager.HandleSttResult(sttResult);
        }
    }
}
