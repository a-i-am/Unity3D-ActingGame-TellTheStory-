using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
    const float MAXSCORE = 100f;
    public int playLineNum;
    private float totalScore;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI changingText;
    [SerializeField] Image progressBarFill;

    private Coroutine showChangingScore;
    private Coroutine graduallyAscendScore;

    private string accurateStr;
    private float accurateSim;

    private string inaccurateStr;
    private float inaccurateSim;
    private void Awake()
    {
        instance = this;
    }
    public void InitAll(int playerLineNum)
    {
        this.playLineNum = playerLineNum;
        totalScore = 0;
        progressBarFill.fillAmount = 0;
        changingText.gameObject.SetActive(false);
        scoreText.text = "0.0";
    }

    public void ChangeScore(float currentSimilarity, string line)
    {
        if (currentSimilarity < inaccurateSim)
        {
            inaccurateSim = currentSimilarity;
            inaccurateStr = line;
        }
        else if (currentSimilarity>accurateSim)
        {
            accurateSim = currentSimilarity;
            accurateStr = line;
        }
        if (showChangingScore != null)
        {
            StopCoroutine(showChangingScore);
        }
        if (showChangingScore != null)
        {
            StopCoroutine(showChangingScore);
        }
        float changingScore = currentSimilarity / playLineNum * 100;
        StartCoroutine(ShowChangingScore(changingScore));
        StartCoroutine(GraduallyAscendScore(totalScore, totalScore + changingScore));
        StartCoroutine(GraduallyAscendProgressBar(totalScore / MAXSCORE, (totalScore + changingScore) / MAXSCORE));
        totalScore += changingScore;
    }

    private IEnumerator ShowChangingScore(float changingScore)
    {
        changingText.gameObject.SetActive(true);
        changingText.text = "+" +changingScore.ToString("F1");
        Color colorTemp = changingText.color;
        colorTemp.a = 1f;
        changingText.color = colorTemp;

        float duration = 1f; // 알파값을 변경하는 데 걸리는 시간
        float elapsedTime = 0f;
        yield return new WaitForSeconds(1f);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            colorTemp.a = Mathf.Lerp(1f, 0f, elapsedTime / duration);
            changingText.color = colorTemp;
            yield return null;
        }

        // 최종적으로 알파값이 0이 되도록 설정
        colorTemp.a = 0f;
        changingText.color = colorTemp;

        changingText.gameObject.SetActive(false);
        showChangingScore = null;
    }
    private IEnumerator GraduallyAscendScore(float from, float to)
    {
        float currentScore = from;
        float stepTime = 0.1f; // 숫자가 한 번 바뀔 때 걸리는 시간
        float stepAmount = 0.1f; // 점수 증가 단위

        while (currentScore < to)
        {
            currentScore += stepAmount;

            // 오버플로 방지
            if (currentScore > to)
            {
                currentScore = to;
            }

            scoreText.text = currentScore.ToString("F1"); // 소수점 한 자리까지 표시
            yield return new WaitForSeconds(stepTime);
        }

        // 마지막 점수 확인 (오차 방지)
        scoreText.text = to.ToString("F1");
        graduallyAscendScore = null;
    }


    private IEnumerator GraduallyAscendProgressBar(float from, float to)
    {
        float currentFill = from;
        float stepTime = 0.1f; // 업데이트 간격
        float stepAmount = 0.01f; // fillAmount 증가 단위

        while (currentFill < to)
        {
            currentFill += stepAmount;

            // 오버플로 방지
            if (currentFill > to)
            {
                currentFill = to;
            }

            progressBarFill.fillAmount = currentFill;
            yield return new WaitForSeconds(stepTime);
        }

        // 최종 값 보정
        progressBarFill.fillAmount = to;
        graduallyAscendScore = null;
    }
    public void GetResult(out float score, out string accurateStr, out string inaccurateStr)
    {
        score = this.totalScore;
        accurateStr = this.accurateStr;
        inaccurateStr = this.inaccurateStr;
    }
}
