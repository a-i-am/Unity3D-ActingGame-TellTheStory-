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
        EpisodeData currentEpisode = episodeDataArr[GameManager.instance.currentNPC];
        titleText.text = currentEpisode.storyInfo.episodes[GameManager.instance.currentAct].episodeTitle;
        summaryText.text = currentEpisode.storyInfo.episodes[GameManager.instance.currentAct].summary;
    }
    public void OnStartButtonClick()
    {
        gameObject.SetActive(false);
    }
}
