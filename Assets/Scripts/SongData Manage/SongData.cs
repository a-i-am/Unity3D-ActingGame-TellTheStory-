using UnityEngine;

[CreateAssetMenu(fileName = "SongData", menuName = "GameData/SongData")]
public class SongData : ScriptableObject
{
    public string songName;          // 노래 제목
    public string artist;            // 아티스트 이름
    public AudioClip audioClip;      // 오디오 파일
    public string filePath;          // 가사 txt 파일 경로 (예: "Assets/Data/lyrics.txt")
    public string[] lyrics;          // 가사 (한 줄씩)
    public float[] lyricsTimes;      // 각 줄의 시작 시간 (초 단위)
    public string[][] wordSplitLyrics;   // 각 줄을 단어별로 분리한 배열
    public float[][] wordTimes;          // 단어별 시작 시간 배열
}
