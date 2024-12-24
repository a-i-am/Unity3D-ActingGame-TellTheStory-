using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] songs;  // 여러 곡을 저장할 배열

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        PlaySong(0);  // 첫 번째 곡을 재생
    }

    public void PlaySong(int songIndex)
    {
        audioSource.clip = songs[songIndex];
        audioSource.Play();
    }
}