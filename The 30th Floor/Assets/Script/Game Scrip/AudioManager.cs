using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gestor de audio principal para el juego.
/// Controla la música de fondo, efectos de sonido (SFX) y sonidos de pasos.
/// Implementa patrón Singleton para acceso global.
/// </summary>
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

    private Coroutine walkingCoroutine;
    public float stepInterval = 0.4f;

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

    private void BuildDictionaries()
    {

        sfxDict = new Dictionary<string, AudioClip>();
        foreach (var clip in sfxClips)
        {
            if (clip != null && !sfxDict.ContainsKey(clip.name))
                sfxDict.Add(clip.name, clip);
        }
    }

    public void StartWalkingSound()
    {
        if (walkingCoroutine == null)
            walkingCoroutine = StartCoroutine(WalkingLoop());
    }

    public void StopWalkingSound()
    {
        if (walkingCoroutine != null)
        {
            StopCoroutine(walkingCoroutine);
            walkingCoroutine = null;
        }
    }

    private IEnumerator WalkingLoop()
    {
        if (!sfxDict.TryGetValue("walkingSound", out var clip))
        {
            Debug.LogWarning("SFX 'walkingSound' no encontrado en el diccionario.");
            yield break;
        }

        while (true)
        {
            sfxSource.pitch = Random.Range(0.9f, 1.1f);
            sfxSource.PlayOneShot(clip);
            yield return new WaitForSeconds(stepInterval);
        }
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
        musicSource.loop = true;
        musicSource.clip = music;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.loop = false;
        if (musicSource.isPlaying)
            musicSource.Stop();
    }
}
