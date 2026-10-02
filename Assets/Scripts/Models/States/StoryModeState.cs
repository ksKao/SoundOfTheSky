using System;
using System.Collections.Generic;

[Serializable]
public class StoryModeState
{
    public string inkJson;
    public DialogSceneType dialogSceneType;
    public string backgroundFileName;
    public string rhythmGameSong;
    public List<AudioTrackSerializable> activeAudioTracks = new();
}

[Serializable]
public class AudioTrackSerializable
{
    public string name;
    public bool loop;
    public float volume;
}
