using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    private AudioSource audioSource;
    [Header("NPC0")]
    [SerializeField] AudioClip needRepeat_0;
    [SerializeField] AudioClip result_0;
    private void Awake()
    {
        if (instance)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public void PlayNeedRepeat()
    {
        audioSource.PlayOneShot(needRepeat_0);
    }
    public void PlayResult()
    {   
        audioSource.PlayOneShot(result_0);
    }
}
