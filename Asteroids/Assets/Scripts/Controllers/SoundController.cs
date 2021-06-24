using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    static AudioSource audioSrc;

    [SerializeField] private List<AudioClip> musicAudios;
    private int musicIndex = 0;
    private float musicDelay = 1.0f;

    private bool isMuted = false;

    private GameController gameController;

    private void Awake()
    {
        audioSrc = GetComponent<AudioSource>();
    }

    private void Start()
    {
        gameController = GameController.Instance;

        isMuted = gameController.Data.isMuted;
        UpdateMuted();
    }

    public static void PlayOneShot(AudioClip audio)
    {
        audioSrc.PlayOneShot(audio);
    }

    public void MuteUnmute()
    {
        isMuted = !isMuted;
        UpdateMuted();
    }

    private void UpdateMuted()
    {
        gameController.Data.isMuted = isMuted;
        audioSrc.volume = isMuted ? 0.0f : 1.0f;
        gameController.Ui.UpdateMuteButtonText(isMuted);
    }

    public static bool IsMuted()
    {
        return audioSrc.mute;
    }

    private void PlayMusic()
    {
        if(GameController.Instance.OnTheGame)
        {
            PlayOneShot(musicAudios[musicIndex]);
            Invoke("PlayMusic", musicAudios[musicIndex].length + musicDelay);

            musicIndex++;
            musicIndex = musicIndex >= musicAudios.Count ? 0 : musicIndex;

            musicDelay -= 0.01f;
            musicDelay = musicDelay <= 0 ? 0 : musicDelay;
        }
    }

    public void StartGame()
    {
        musicDelay = 1.0f;
        PlayMusic();
    }
}
