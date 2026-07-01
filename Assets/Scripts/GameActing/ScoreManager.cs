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

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        instance = null;
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
        SoundManager.instance.PlayGetScore();
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

        float duration = 1f;
        float elapsedTime = 0f;
        yield return new WaitForSeconds(1f);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            colorTemp.a = Mathf.Lerp(1f, 0f, elapsedTime / duration);
            changingText.color = colorTemp;
            yield return null;
        }


        colorTemp.a = 0f;
        changingText.color = colorTemp;

        changingText.gameObject.SetActive(false);
        showChangingScore = null;
    }
    private IEnumerator GraduallyAscendScore(float from, float to)
    {
        float currentScore = from;
        float stepTime = 0.03f;
        float stepAmount = 0.1f;

        while (currentScore < to)
        {
            currentScore += stepAmount;


            if (currentScore > to)
            {
                currentScore = to;
            }

            scoreText.text = currentScore.ToString("F1");
            yield return new WaitForSeconds(stepTime);
        }


        scoreText.text = to.ToString("F1");
        graduallyAscendScore = null;
    }


    private IEnumerator GraduallyAscendProgressBar(float from, float to)
    {
        float currentFill = from;
        float stepTime = 0.1f;
        float stepAmount = 0.01f;

        while (currentFill < to)
        {
            currentFill += stepAmount;


            if (currentFill > to)
            {
                currentFill = to;
            }

            progressBarFill.fillAmount = currentFill;
            yield return new WaitForSeconds(stepTime);
        }


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
