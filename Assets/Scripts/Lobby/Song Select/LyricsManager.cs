using UnityEngine;
using UnityEngine.UI;
using System.IO; // 파일 입출력을 위한 네임스페이스
using System;
using TMPro;

public class LyricsManager : MonoBehaviour
{
    public TextMeshProUGUI lyricsText; // UI TextMeshProUGUI 컴포넌트
    public AudioSource audioSource;  // 오디오 소스
    public GameObject previewPanel;  // 가사 전체를 보여주는 프리뷰 패널
    public TextMeshProUGUI previewText;         // 프리뷰 패널 내 텍스트
    public GameObject gamePanel;     // 본 게임에서 가사를 보여주는 패널

    private string[] lyrics;         // 가사 배열
    private float[] lyricsTimes;     // 각 가사 시간 배열 (초 단위)
    private int currentIndex = 0;

    void Start()
    {
        // 특정 경로에서 가사 파일 읽어오기
        string filePath = Application.dataPath + "/Data/test lyric.txt";
        LoadLyricsFromFile(filePath);

        // 프리뷰 모드 초기화
        ShowPreview();
    }

    void Update()
    {
        // 게임 모드일 때만 가사 업데이트
        if (gamePanel.activeSelf && audioSource.isPlaying && currentIndex < lyrics.Length)
        {
            if (audioSource.time >= lyricsTimes[currentIndex])
            {
                lyricsText.text = lyrics[currentIndex];
                currentIndex++;

                // 마지막 가사 후에는 텍스트를 비움
                if (currentIndex >= lyrics.Length)
                {
                    lyricsText.text = "";
                }
            }
        }
    }

    // 가사 파일을 읽어오는 함수
    void LoadLyricsFromFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            string[] lines = File.ReadAllLines(filePath); // 파일의 모든 줄을 읽음
            lyrics = new string[lines.Length];
            lyricsTimes = new float[lines.Length];

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].Trim();

                // 빈 줄 무시
                if (!string.IsNullOrEmpty(line))
                {
                    // 라인에서 시간과 가사를 분리
                    string[] parts = line.Split(']');
                    if (parts.Length == 2)
                    {
                        string timeStr = parts[0].TrimStart('['); // [ 제거
                        string lyric = parts[1].Trim();          // 가사 부분

                        // 시간을 초 단위로 변환 (예: 00:10.50 -> 10.5)
                        string[] timeParts = timeStr.Split(':');
                        if (timeParts.Length == 2)
                        {
                            float minutes = float.Parse(timeParts[0]);
                            float seconds = float.Parse(timeParts[1]);
                            lyricsTimes[i] = minutes * 60 + seconds;
                        }

                        lyrics[i] = lyric; // 가사 저장
                    }
                }
            }
        }
        else
        {
            Debug.LogError($"Lyrics file not found at: {filePath}");
        }
    }

    // 프리뷰 모드: 가사 전체를 표시
    void ShowPreview()
    {
        previewPanel.SetActive(true);
        gamePanel.SetActive(false);

        // 가사 전체를 합쳐서 표시
        previewText.text = "";
        for (int i = 0; i < lyrics.Length; i++)
        {
            previewText.text += lyrics[i] + "\n";
        }
    }

    // 게임 모드: 음악과 동기화하여 가사를 표시
    public void StartGame()
    {
        previewPanel.SetActive(false);
        gamePanel.SetActive(true);

        // 초기화
        currentIndex = 0;
        lyricsText.text = "";
        audioSource.Play();
    }
}
