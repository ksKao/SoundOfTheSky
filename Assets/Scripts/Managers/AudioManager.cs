using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    private readonly Dictionary<string, AudioSource> _audioSources = new();
    private AudioSource _voiceAudioSource;

    protected override void Awake()
    {
        base.Awake();

        _voiceAudioSource = gameObject.AddComponent<AudioSource>();
        _voiceAudioSource.loop = false;
    }

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

    public void PlayVoice(string name)
    {
        _voiceAudioSource.clip = Resources.Load<AudioClip>(
            $"Audio/Voices/Day {CampaignModeManager.Instance.CurrentTime.day}/{name}"
        );

        _voiceAudioSource.Play();
    }

    public void StopVoice()
    {
        _voiceAudioSource.Stop();
    }

    public void PlayAudioWithDuration(string name, float duration)
    {
        // check if audio exist in the list
        if (!_audioSources.TryGetValue(name, out AudioSource audioSource))
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            _audioSources.Add(name, audioSource);

            audioSource.clip = Resources.Load<AudioClip>($"Audio/{name}");
        }

        audioSource.Play();
        DOVirtual.DelayedCall(duration, () => audioSource.Stop());
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

    public void StopAllAudio()
    {
        foreach (KeyValuePair<string, AudioSource> audioSource in _audioSources)
        {
            audioSource.Value.Stop();
        }
    }

    public float GetSongDuration(string name)
    {
        if (!_audioSources.TryGetValue(name, out AudioSource audioSource))
        {
            return 0;
        }

        return audioSource.clip.length;
    }
}
