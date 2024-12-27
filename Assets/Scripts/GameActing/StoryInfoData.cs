using System;
using UnityEngine;
[CreateAssetMenu(fileName = "ActingInfoData", menuName = "Acting/ActingInfoData", order = 2)]
public class StoryInfoData : ScriptableObject
{
    public string storyTitle;
    public ActInfo[] actInfos;
}
[Serializable]
public class ActInfo
{
    public string title;
    public string summary;
}