using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActSelectBtn : MonoBehaviour
{
    [SerializeField] GameObject playButton;
    [SerializeField] GameObject pauseButton;
    private void Start()
    {
        playButton.SetActive(true);
        pauseButton.SetActive(false);
    }
    public void OnPlayButtonClick()
    {
        playButton.SetActive(false);
        pauseButton.SetActive(true);
        PlayStoryManager.instance.PlayStory(0, 0);
    }
    public void OnPauseButtonClick()
    {
        playButton.SetActive(true);
        pauseButton.SetActive(false);
        PlayStoryManager.instance.StopPlay();
    }
}
