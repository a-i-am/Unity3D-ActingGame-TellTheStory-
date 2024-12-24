using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SongPreview : MonoBehaviour
{

    // 노래 목록과 정보 표시
    public SongData[] songList;     // 등록된 SongData 목록
    private int selectedIndex = 0;  // 현재 선택된 곡 인덱스

    // 곡 정보 텍스트
    public TextMeshProUGUI songNameText;       // 곡명 표시용 텍스트
    public TextMeshProUGUI artistText;         // 아티스트 이름 표시용 텍스트
    public TextMeshProUGUI lyricsContent;      // 가사 표시용 텍스트

    public AudioSource audioSource; // 오디오 소스


    // Start is called before the first frame update
    void Start()
    {
        //ShowPreview();
    }

    // 이전 곡 선택
    public void SelectPrevious()
    {
        selectedIndex = (selectedIndex - 1 + songList.Length) % songList.Length;
        ShowPreview();
    }

    // 다음 곡 선택
    public void SelectNext()
    {
        selectedIndex = (selectedIndex + 1) % songList.Length;
        ShowPreview();
    }

    // 선택된 곡 정보 업데이트
    public void ShowPreview()
    {
        SongData selectedSong = songList[selectedIndex];

        // 곡 정보 표시
        songNameText.text = $"곡명: {selectedSong.songName}";
        artistText.text = $"아티스트: {selectedSong.artist}";

        // 가사 전체 표시
        lyricsContent.text = ""; // 초기화
        for (int i = 0; i < selectedSong.lyrics.Length; i++)
        {
            lyricsContent.text += selectedSong.lyrics[i] + "\n";
        }

        // 오디오 소스에 곡 설정
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
        audioSource.clip = selectedSong.audioClip;
        audioSource.Play();

    }

    // 선택된 곡으로 게임 시작
    public void StartGame()
    {
        // GameManager나 DontDestroyOnLoad를 사용해 선택된 곡 데이터 전달
        GameManager.Instance.SelectedSong = songList[selectedIndex];
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }
}
