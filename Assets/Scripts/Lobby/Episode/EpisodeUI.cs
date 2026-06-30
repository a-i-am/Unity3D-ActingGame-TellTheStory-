using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class EpisodeUI : MonoBehaviour
{
    [SerializeField] EpisodeData episodeData;
    [SerializeField] private Transform buttonParent;
    [SerializeField] private Button episodeButtonPrefab;
    [SerializeField] private GameObject episodeWindow;
    [SerializeField] private GameObject episodeInfoPanel;
    [SerializeField] private GameObject storyInfoArea;
    [SerializeField] private Button gameStartButton;

    [SerializeField] private TextMeshProUGUI episodeTitleText;
    [SerializeField] private TextMeshProUGUI summaryText;
    private EpisodeData.Episode selectedEpisode;

    void Start()
    {

        if (episodeData != null && episodeData.storyInfo.episodes.Count > 0)
        {
            GenerateButtons();
        }


        gameStartButton.onClick.AddListener(OnGameStartButton);
        episodeWindow.SetActive(false);
    }

    void GenerateButtons()
    {

        foreach (Transform child in buttonParent)
        {
            Destroy(child.gameObject);
        }


        foreach (var episode in episodeData.storyInfo.episodes)
        {

            Button newButton = Instantiate(episodeButtonPrefab, buttonParent);
            newButton.GetComponentInChildren<TextMeshProUGUI>().text = episode.episodeTitle;


            newButton.onClick.AddListener(() => OnEpisodeButtonClicked(episode));
        }
    }


    void OnEpisodeButtonClicked(EpisodeData.Episode episode)
    {
        SoundManager.instance.PlaySelect();
        selectedEpisode = episode;
        storyInfoArea.SetActive(true);

        episodeTitleText.text = episode.episodeTitle;
        summaryText.text = episode.summary;
    }

    void OnGameStartButton()
    {

        if (selectedEpisode != null)
        {

            GameManager.instance.currentAct = episodeData.storyInfo.episodes.IndexOf(selectedEpisode);
            GameManager.instance.currentNPC = 0;


            SceneManager.LoadScene("GameActing");
        }
        else
        {
            Debug.LogWarning("에피소드를 선택해주세요.");
        }
    }

    public void ExitEpisodeWindow()
    {
        episodeWindow.SetActive(false);
    }

}
