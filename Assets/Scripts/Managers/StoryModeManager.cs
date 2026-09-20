using UnityEngine;

public class StoryModeManager : Singleton<StoryModeManager>
{
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
    }

    private void OnEnable()
    {
        InputManager.Instance.InputAction.StoryMode.Enable();
    }

    private void Start()
    {
        TextAsset storyTextAsset = Resources.Load<TextAsset>($"Stories/Main");
        UiManager.Instance.StoryModeScreen.Play(storyTextAsset);
    }

    private void OnDisable()
    {
        InputManager.Instance.InputAction.StoryMode.Disable();
    }
}
