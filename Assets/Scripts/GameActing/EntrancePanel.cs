using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EntrancePanel : MonoBehaviour
{
    [SerializeField] EpisodeData[] episodeDataArr;

    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] TextMeshProUGUI summaryText;
    private void Start()
    {
        EpisodeData currentEpisode = episodeDataArr[GameManager.Instance.currentNPC];
        titleText.text = currentEpisode.storyInfo.episodes[GameManager.Instance.currentAct].episodeTitle;
        summaryText.text = currentEpisode.storyInfo.episodes[GameManager.Instance.currentAct].summary;
    }
    public void OnStartButtonClick()
    {
        gameObject.SetActive(false);
    }
}
