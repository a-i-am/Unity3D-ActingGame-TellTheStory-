using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    const int MAXSCORE = 100;
    public int playLineNum;
    private float totalScore;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] Image progressBarFill;

    public void InitAll(int playerLineNum)
    {
        this.playLineNum = playerLineNum;
        totalScore = 0;
        scoreText.text = "0";
        progressBarFill.fillAmount = 0;
    }
    public void ChangeScore(float currentSimilarity)
    {
        float score = currentSimilarity / playLineNum * 100;
        totalScore += score;
        scoreText.text = $"{Mathf.FloorToInt(totalScore)} / {MAXSCORE}";
        progressBarFill.fillAmount = totalScore / MAXSCORE;
    }
}
