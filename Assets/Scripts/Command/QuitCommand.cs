using System.Collections.Generic;

public class QuitCommand : Command
{
    public override string Id => "quit";

    public override string HelpText => "Quit the game.";

    public override Dictionary<string, string> Usage => new() { { "quit", "Quit the game." } };

    public override void Execute(string[] args)
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
