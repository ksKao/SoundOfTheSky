using UnityEngine;
using UnityEngine.Video;

public class VideoManager : Singleton<VideoManager>
{
    private VideoPlayer _videoPlayer;
    private RenderTexture _renderTexture;

    public RenderTexture Texture => _renderTexture;

    protected override void Awake()
    {
        base.Awake();

        _renderTexture = new RenderTexture(1920, 1080, 0);

        _videoPlayer = gameObject.AddComponent<VideoPlayer>();
        _videoPlayer.playOnAwake = false;
        _videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        _videoPlayer.targetTexture = _renderTexture;

        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        _videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
        _videoPlayer.SetTargetAudioSource(0, audioSource);
    }

    // background filenames may match either a video or an image; callers check this first so videos take priority
    public bool TryGetVideoClip(string fileName, out VideoClip clip)
    {
        clip = Resources.Load<VideoClip>($"Videos/{fileName}");
        return clip != null;
    }

    public void PlayVideo(VideoClip clip, bool loop)
    {
        _videoPlayer.Stop();
        _videoPlayer.clip = clip;
        _videoPlayer.isLooping = loop;
        _videoPlayer.Play();
    }

    public void StopVideo()
    {
        _videoPlayer.Stop();
    }
}
