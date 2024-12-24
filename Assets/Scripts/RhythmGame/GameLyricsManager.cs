using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameLyricsManager : MonoBehaviour
{
    public AudioSource audioSource;      // 오디오 소스
    public TextMeshProUGUI lyricsText;   // 가사 표시용 TextMeshPro

    private SongData songData;           // 선택된 곡 데이터
    private int currentLineIndex = 0;    // 현재 줄 인덱스
    private int currentWordIndex = 0;    // 현재 단어 인덱스

    void Start()
    {
        // GameManager에서 선택된 SongData를 가져옴
        songData = GameManager.Instance.SelectedSong;

        if (songData == null)
        {
            Debug.LogError("SongData가 설정되지 않았습니다. GameManager에서 데이터를 전달했는지 확인하세요.");
            return;
        }

        // 오디오 소스 설정
        audioSource.clip = songData.audioClip;
        audioSource.Play();

        // 초기화
        currentLineIndex = 0;
        currentWordIndex = 0;
        lyricsText.text = ""; // 가사 초기화
    }

    void Update()
    {
        if (audioSource.isPlaying && currentLineIndex < songData.lyrics.Length)
        {
            // 현재 줄의 단어와 타이밍 정보 가져오기
            string[] words = songData.wordSplitLyrics[currentLineIndex];
            float[] wordTimes = songData.wordTimes[currentLineIndex];

            // 단어 업데이트
            if (currentWordIndex < words.Length && audioSource.time >= wordTimes[currentWordIndex])
            {
                UpdateLyricsDisplay(words, currentWordIndex);
                currentWordIndex++;
            }

            // 줄의 모든 단어를 표시한 경우 다음 줄로 이동
            if (currentWordIndex >= words.Length)
            {
                currentLineIndex++;
                currentWordIndex = 0;
            }
        }
    }



    // 가사 텍스트 업데이트
    void UpdateLyricsDisplay(string[] words, int highlightIndex)
    {
        lyricsText.text = ""; // 초기화
        for (int i = 0; i < words.Length; i++)
        {
            if (i == highlightIndex)
            {
                // 현재 단어 강조
                lyricsText.text += $"<color=#FFD700>{words[i]}</color> ";
            }
            else
            {
                lyricsText.text += $"{words[i]} ";
            }
        }
    }
}
