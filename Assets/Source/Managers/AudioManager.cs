using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private Sound[] sounds;
    [SerializeField] private Sound[] music;

    private bool isCoroutineActive = false;

    private Sound playedMusic;

    void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);
        else
            Instance = this;

        for (int i = 0; i < sounds.Length; i++)
        {
            sounds[i].source = gameObject.AddComponent<AudioSource>();
            sounds[i].source.clip = sounds[i].clip;

            sounds[i].source.volume = sounds[i].volume;
            sounds[i].source.pitch = 1f;
        }

        for (int i = 0; i < music.Length; i++)
        {
            music[i].source = gameObject.AddComponent<AudioSource>();
            music[i].source.clip = music[i].clip;

            music[i].source.volume = music[i].volume;
            music[i].source.pitch = 1f;
        }

        isCoroutineActive = false;
    }

    void Update()
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            sounds[i].source.volume = sounds[i].volume * SerializeManager.GetSFXVolume() * SerializeManager.GetMasterVolume();
        }

        for (int i = 0; i < music.Length; i++)
        {
            music[i].source.volume = music[i].volume * SerializeManager.GetMusicVolume() * SerializeManager.GetMasterVolume();
        }

        if (!isCoroutineActive)
            StartCoroutine(PlayMusicCoroutine());
    }

    public void PlaySound(string name)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].name == name)
                sounds[i].source.Play();
        }
    }

    public void StopSound(string name)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].name == name)
                sounds[i].source.Stop();
        }
    }

    public IEnumerator PlayMusicCoroutine()
    {
        isCoroutineActive = true;

        int random;

        do random = Random.Range(0, music.Length);
        while (music[random] == playedMusic);

        music[random].source.Play();

        playedMusic = music[random];

        yield return new WaitForSecondsRealtime(music[random].clip.length);

        isCoroutineActive = false;
    }
}