using System;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Episode", menuName = "Acting/Episode", order = 2)]
public class EpisodeData : ScriptableObject
{
    [Serializable]
    public class Episode
    {
        public string episodeTitle; // 에피소드 제목
        [TextArea]
        public string summary; // 줄거리
    }

    [Serializable]
    public class StoryInfo
    {
        public List<Episode> episodes = new List<Episode>(); // 에피소드 목록
    }

    public StoryInfo storyInfo = new StoryInfo();
}
