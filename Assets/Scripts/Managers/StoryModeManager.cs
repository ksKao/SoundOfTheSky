using System.IO;
using UnityEngine;

public class StoryModeManager : Singleton<StoryModeManager>
{
    public static string SaveFilePath
    {
        get
        {
            int index = PlayerPrefs.GetInt(SaveMenu.PLAYER_PREFS_SAVE_FILE_TO_LOAD_KEY, -1);
            return GetSaveFilePath(index);
        }
    }

    protected override void Awake()
    {
        Application.runInBackground = true;
        InputManager.Instance.InputAction.Disable();
        InputManager.Instance.InputAction.StoryMode.A.performed += ctx =>
            UiManager.Instance.StoryModeScreen.RhythmGameScene.RhythmGameGameplay.PressLane(
                RhythmGameLane.A
            );
        InputManager.Instance.InputAction.StoryMode.S.performed += ctx =>
            UiManager.Instance.StoryModeScreen.RhythmGameScene.RhythmGameGameplay.PressLane(
                RhythmGameLane.S
            );
        InputManager.Instance.InputAction.StoryMode.D.performed += ctx =>
            UiManager.Instance.StoryModeScreen.RhythmGameScene.RhythmGameGameplay.PressLane(
                RhythmGameLane.D
            );
        InputManager.Instance.InputAction.StoryMode.F.performed += ctx =>
            UiManager.Instance.StoryModeScreen.RhythmGameScene.RhythmGameGameplay.PressLane(
                RhythmGameLane.F
            );
        InputManager.Instance.InputAction.StoryMode.A.canceled += ctx =>
            UiManager.Instance.StoryModeScreen.RhythmGameScene.RhythmGameGameplay.ReleaseLane(
                RhythmGameLane.A
            );
        InputManager.Instance.InputAction.StoryMode.S.canceled += ctx =>
            UiManager.Instance.StoryModeScreen.RhythmGameScene.RhythmGameGameplay.ReleaseLane(
                RhythmGameLane.S
            );
        InputManager.Instance.InputAction.StoryMode.D.canceled += ctx =>
            UiManager.Instance.StoryModeScreen.RhythmGameScene.RhythmGameGameplay.ReleaseLane(
                RhythmGameLane.D
            );
        InputManager.Instance.InputAction.StoryMode.F.canceled += ctx =>
            UiManager.Instance.StoryModeScreen.RhythmGameScene.RhythmGameGameplay.ReleaseLane(
                RhythmGameLane.F
            );
        InputManager.Instance.InputAction.StoryMode.OpenMenu.performed += ctx =>
            UiManager.Instance.Modal.Show(UiManager.Instance.StoryModeScreen.twoButtonMenu);
    }

    private void OnEnable()
    {
        InputManager.Instance.InputAction.StoryMode.Enable();
    }

    private void Start()
    {
        TextAsset storyTextAsset = Resources.Load<TextAsset>($"Stories/Main");
        UiManager.Instance.StoryModeScreen.Play(storyTextAsset, LoadGame());
    }

    private void OnDisable()
    {
        InputManager.Instance.InputAction.StoryMode.Disable();
    }

    public static string GetSaveFilePath(int index)
    {
        return Path.Combine(Application.persistentDataPath, $"story_mode_{index}.json");
    }

    private string LoadGame()
    {
        if (!File.Exists(SaveFilePath))
            return "";

        try
        {
            string serialized = "";

            using (FileStream stream = new(SaveFilePath, FileMode.Open))
            {
                using (StreamReader reader = new(stream))
                {
                    serialized = reader.ReadToEnd();
                }
            }

            return serialized;
        }
        catch
        {
            UiUtils.ShowError("Failed to load save file. Starting a new game.");
            return "";
        }
    }
}
