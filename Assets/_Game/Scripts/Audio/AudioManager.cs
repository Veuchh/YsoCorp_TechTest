using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    Queue<AudioSource> availableAudioSources = new Queue<AudioSource>();
    List<AudioSource> playingAudioSources = new List<AudioSource>();

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError($"Trying to initialize an Intance of {nameof(AudioManager)}, but one already existed. Aborting.", this);
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Update()
    {
        if (playingAudioSources.Count == 0)
            return;

        for (int i = playingAudioSources.Count - 1; i >= 0; i--)
        {
            AudioSource audioSource = playingAudioSources[i];

            if (!audioSource.isPlaying)
            {
                playingAudioSources.Remove(audioSource);
                availableAudioSources.Enqueue(audioSource);
            }
        }
    }

    public void PlayAudioData(AudioData audioData)
    {
        AudioSource audioSource = GetAudioSource();

        audioSource.clip = audioData.GetClip;
        audioSource.pitch = audioData.GetPitch;
        audioSource.volume = audioData.GetVolume;

        audioSource.Play();

        playingAudioSources.Add(audioSource);
    }

    AudioSource GetAudioSource()
    {
        if (availableAudioSources.Count == 0)
        {
            availableAudioSources.Enqueue(gameObject.AddComponent<AudioSource>());
        }

        return availableAudioSources.Dequeue();
    }
}
