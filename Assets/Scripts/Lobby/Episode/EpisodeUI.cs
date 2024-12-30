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
    private EpisodeData.Episode selectedEpisode; // 선택된 에피소드 저장

    void Start()
    {
        // 에피소드 데이터가 비어있지 않으면 버튼을 생성
        if (episodeData != null && episodeData.storyInfo.episodes.Count > 0)
        {
            GenerateButtons();
        }

        // 게임 시작 버튼에 리스너 추가
        gameStartButton.onClick.AddListener(OnGameStartButton);
        episodeWindow.SetActive(false);
    }

    void GenerateButtons()
    {
        // 기존의 버튼들 삭제
        foreach (Transform child in buttonParent)
        {
            Destroy(child.gameObject);
        }

        // 에피소드 리스트 순서대로 버튼을 생성
        foreach (var episode in episodeData.storyInfo.episodes)
        {
            // 버튼 생성
            Button newButton = Instantiate(episodeButtonPrefab, buttonParent);
            newButton.GetComponentInChildren<TextMeshProUGUI>().text = episode.episodeTitle; // 버튼 텍스트에 에피소드 제목 설정

            // 버튼 클릭 시 동작 정의
            newButton.onClick.AddListener(() => OnEpisodeButtonClicked(episode));
        }
    }

    // 버튼 클릭 시 호출될 메서드
    void OnEpisodeButtonClicked(EpisodeData.Episode episode)
    {
        SoundManager.instance.PlaySelect();
        selectedEpisode = episode;  // 선택된 에피소드 저장
        storyInfoArea.SetActive(true);

        episodeTitleText.text = episode.episodeTitle; // 에피소드 제목
        summaryText.text = episode.summary; // 에피소드 줄거리
    }

    void OnGameStartButton()
    {
        // 게임 시작 시 선택된 에피소드 정보로 게임을 초기화
        if (selectedEpisode != null)
        {
            // 게임에 필요한 데이터 초기화
            GameManager.Instance.currentAct = episodeData.storyInfo.episodes.IndexOf(selectedEpisode);
            GameManager.Instance.currentNPC = 0;

            // 게임 씬으로 전환
            SceneManager.LoadScene("Real Copy"); 
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
