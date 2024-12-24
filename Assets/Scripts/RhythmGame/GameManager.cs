using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public SongData SelectedSong; // 선택된 곡 데이터

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
