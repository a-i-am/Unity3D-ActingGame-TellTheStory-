#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SongData))]
public class SongDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SongData songData = (SongData)target;

        if (GUILayout.Button("Load Lyrics"))
        {
            // LyricsParser를 호출하여 가사를 로드
            LyricsParser.LoadLyrics(songData);
        }
    }
}
#endif
