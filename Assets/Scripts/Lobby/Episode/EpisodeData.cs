using System;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Episode", menuName = "Acting/Episode", order = 2)]
public class EpisodeData : ScriptableObject
{
    [Serializable]
    public class Episode
    {
        public string episodeTitle;
        [TextArea]
        public string summary;
    }

    [Serializable]
    public class StoryInfo
    {
        public List<Episode> episodes = new List<Episode>();
    }

    public StoryInfo storyInfo = new StoryInfo();
}
