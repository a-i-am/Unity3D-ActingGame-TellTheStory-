using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class LyricsParser
{
    public static void LoadLyrics(SongData songData)
    {
        if (songData == null)
        {
            Debug.LogError("SongData ScriptableObject가 null입니다.");
            return;
        }

        string filePath = songData.filePath;
        if (string.IsNullOrEmpty(filePath))
        {
            Debug.LogError("SongData의 파일 경로가 설정되지 않았습니다.");
            return;
        }

        try
        {
            // 파일 읽기
            string[] lines = File.ReadAllLines(filePath);

            // 데이터 저장을 위한 리스트
            List<string> lyricsList = new List<string>();
            List<float> timesList = new List<float>();

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                // 시간과 가사를 분리
                int timeStart = line.IndexOf('[');
                int timeEnd = line.IndexOf(']');
                if (timeStart == -1 || timeEnd == -1) continue;

                string timeStr = line.Substring(timeStart + 1, timeEnd - timeStart - 1);
                string lyric = line.Substring(timeEnd + 1).Trim();

                // 시간 파싱
                if (TimeSpan.TryParseExact(timeStr, @"mm\:ss\.ff", null, out TimeSpan time))
                {
                    float timeInSeconds = (float)time.TotalSeconds;
                    timesList.Add(timeInSeconds);
                    lyricsList.Add(lyric);
                }
            }

            // ScriptableObject에 데이터 저장
            songData.lyrics = lyricsList.ToArray();
            songData.lyricsTimes = timesList.ToArray();

            Debug.Log("가사가 성공적으로 로드되었습니다.");
        }
        catch (Exception e)
        {
            Debug.LogError($"가사를 로드하는 동안 오류가 발생했습니다: {e.Message}");
        }
    }
}
