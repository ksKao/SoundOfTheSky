using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    private readonly Dictionary<string, AudioSource> _audioSources = new();

    public void PlayAudio(string name, bool loop)
    {
        // check if audio exist in the list
        if (!_audioSources.TryGetValue(name, out AudioSource audioSource))
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            _audioSources.Add(name, audioSource);

            audioSource.clip = Resources.Load<AudioClip>($"Audio/{name}");
        }

        audioSource.loop = loop;
        audioSource.Play();
    }

    public float GetVolume(string name)
    {
        if (!_audioSources.TryGetValue(name, out AudioSource audioSource))
        {
            Debug.LogWarning($"Audio {name} does not exist.");
            return 0;
        }

        return audioSource.volume;
    }

    public void SetVolume(string name, float volume)
    {
        if (!_audioSources.TryGetValue(name, out AudioSource audioSource))
        {
            Debug.LogWarning($"Audio {name} does not exist.");
            return;
        }

        audioSource.volume = volume;
    }

    public void StopAudio(string name)
    {
        if (!_audioSources.TryGetValue(name, out AudioSource audioSource))
        {
            Debug.LogWarning($"Audio {name} does not exist.");
            return;
        }

        audioSource.Stop();
        _audioSources.Remove(name);
    }
}
