using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultPanel : MonoBehaviour
{
    [SerializeField]TextMeshProUGUI similarityText;
    [SerializeField] TextMeshProUGUI accurateText;
    [SerializeField] TextMeshProUGUI inaccurateText;
    public void SetResult(float score, string accurateStr, string inaccurateStr)
    {
        similarityText.text = $"대사 일치도 : {score * 100:F0}%";
        accurateText.text = $"가장 정확한 문장 : {accurateText}";
        inaccurateText.text = $"가장 부정확한 문장 : {inaccurateText}";
    }
    public void GoToLobby()
    {
        SoundManager.instance.PlayButton(0);
        SceneManager.LoadScene("Lobby");
    }
}
