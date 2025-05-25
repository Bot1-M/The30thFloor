using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Audio Clips")]
    public AudioClip[] explorationClips;
    public AudioClip[] menuClips;
    public AudioClip fightingClips;
    public AudioClip[] sfxClips;

    Dictionary<string, AudioClip> sfxDict;

    private void Awake()
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

        BuildDictionaries();
    }

    //private void Start()
    //{
    //    PlayMusic(explorationClips[Random.Range(0, explorationClips.Length)]);
    //}

    private void BuildDictionaries()
    {

        sfxDict = new Dictionary<string, AudioClip>();
        foreach (var clip in sfxClips)
        {
            if (clip != null && !sfxDict.ContainsKey(clip.name))
                sfxDict.Add(clip.name, clip);
        }
    }


    public void PlayWalkingSound()
    {
        if (!sfxSource.isPlaying)
        {

            sfxSource.loop = true;

            sfxSource.clip = sfxDict["walkingSound"];
            if (Random.Range(0, 2) == 0)
                sfxSource.pitch = Random.Range(0.8f, 1.2f);
            sfxSource.Play();
        }
    }

    public void StopWalkingSound()
    {
        sfxSource.loop = false;
        if (sfxSource.isPlaying)
            sfxSource.Stop();
    }

    public void PlaySFX(string name)
    {
        if (sfxDict.TryGetValue(name, out var clip))
        {
            sfxSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"SFX '{name}' no encontrado.");
        }
    }

    public void PlayMusic(AudioClip music)
    {
        musicSource.clip = music;
        musicSource.Play();
    }

    public void StopMusic()
    {
        if (musicSource.isPlaying)
            musicSource.Stop();
    }
}
