using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [SerializeField] AudioSource bgmSource;
    [SerializeField] AudioSource sfxSource;

    [Header("BGM")]
    [SerializeField] AudioClip titleBgm;
    [SerializeField] AudioClip lobbyBgm;
    [SerializeField] AudioClip actingGameBgm;

    [Header("NPC0")]
    [SerializeField] AudioClip needRepeat_0;
    [SerializeField] AudioClip result_0;
    [SerializeField] AudioClip[] dialogue_0;

    [Header("UI")]
    [SerializeField] AudioClip[] buttonSounds;
    [SerializeField] AudioClip clearSound;
    [SerializeField] AudioClip hoverSound;
    [SerializeField] AudioClip getScoreSound;
    [SerializeField] AudioClip selectSound;

    [Header("Walk")]
    [SerializeField] AudioClip walkSound;

    private readonly float walkInterval = 0.8f;
    private float lastWalkSoundTime;
    private bool isWalking = false;
    private float currentVolume;
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
        bgmSource = GetComponent<AudioSource>();
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
    }

    private void Start()
    {
        bgmSource = GetComponent<AudioSource>();
    }

    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        currentVolume = 0.5f;
        switch (arg0.name)
        {
            case "Title":
                bgmSource.clip = titleBgm;
                break;
            case "Lobby":
                bgmSource.clip = lobbyBgm;
                break;
            case "GameActing":
                bgmSource.clip = actingGameBgm;
                currentVolume = 0.1f;
                break;
        }
        bgmSource.volume = currentVolume;
        bgmSource.Play();
    }

    public void PlayNeedRepeat()
    {
        sfxSource.PlayOneShot(needRepeat_0);
    }

    public void PlayResult()
    {
        sfxSource.PlayOneShot(result_0);
    }

    public void PlayButton(int index)
    {
        if (index < buttonSounds.Length)
            sfxSource.PlayOneShot(buttonSounds[index]);
    }

    public void PlayClear()
    {
        sfxSource.PlayOneShot(clearSound);
    }

    public void PlayHover()
    {
        sfxSource.PlayOneShot(hoverSound);
    }

    public void PlayGetScore()
    {
        sfxSource.PlayOneShot(getScoreSound);
    }
    public void PlaySelect()
    {
        sfxSource.PlayOneShot(selectSound);
    }

    private void PlayWalkSound()
    {
        if (isWalking && Time.time - lastWalkSoundTime >= walkInterval)
        {
            sfxSource.PlayOneShot(walkSound);
            lastWalkSoundTime = Time.time;
        }
    }

    public void StartWalking()
    {
        isWalking = true;
    }

    public void StopWalking()
    {
        isWalking = false;
    }

    public void TurnOffBGM()
    {
        bgmSource.volume = 0f;
    }

    public void TurnOnBGM()
    {
        bgmSource.volume = currentVolume;
    }

    private void Update()
    {
        if (isWalking)
        {
            PlayWalkSound();
        }
    }

    internal void PlayNPC0_Dialogue(int index)
    {
        sfxSource.PlayOneShot(dialogue_0[index]);
    }
}
