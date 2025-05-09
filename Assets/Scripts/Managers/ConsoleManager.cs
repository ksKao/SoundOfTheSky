using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

public class ConsoleManager : Singleton<ConsoleManager>
{
    private ConsoleUi _ui;

    public List<Command> Commands { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        _ui = new();
        Commands = new()
        {
            new HelpCommand(),
            new ClearCommand(),
            new GiveCommand(),
            new UnlockCommand(),
            new SetCommand(),
            new QuitCommand(),
        };

        InputManager.Instance.InputAction.Main.OpenConsole.performed += ctx => OpenConsole();
        InputManager.Instance.InputAction.Console.Close.performed += ctx => CloseConsole();
        InputManager.Instance.InputAction.Console.Submit.performed += ctx => SubmitCommand();
    }

    public void Output(
        string message,
        ConsoleOutputLevel consoleOutputLevel = ConsoleOutputLevel.Info
    )
    {
        _ui.AddOutput(message, consoleOutputLevel);
    }

    private void OpenConsole()
    {
        InputManager.Instance.InputAction.Main.Disable();
        InputManager.Instance.InputAction.Console.Enable();
        UiManager.Instance.GameplayScreen.Add(_ui);
        StartCoroutine(FocusTextField());
    }

    private void CloseConsole()
    {
        InputManager.Instance.InputAction.Main.Enable();
        InputManager.Instance.InputAction.Console.Disable();
        UiManager.Instance.GameplayScreen.Remove(_ui);
    }

    private void SubmitCommand()
    {
        try
        {
            string input = _ui.textField.value;

            string[] split = input.Split(' ');

            if (string.IsNullOrWhiteSpace(input) || split.Length == 0)
                return;

            Command found =
                Commands.FirstOrDefault(c => c.Id == split[0])
                ?? throw new Exception($"\"{split[0]}\" is not a valid command.");

            found.Execute(split.Skip(1).ToArray());
        }
        catch (Exception e)
        {
            _ui.AddOutput(e.Message, ConsoleOutputLevel.Error);
        }
        finally
        {
            _ui.textField.value = "";
            StartCoroutine(FocusTextField());
            StartCoroutine(ScrollToBottom());
        }
    }

    public void Clear()
    {
        _ui.outputContainer.Clear();
    }

    private IEnumerator FocusTextField()
    {
        yield return null;
        _ui.textField.Focus();
    }

    private IEnumerator ScrollToBottom()
    {
        yield return null;

        VisualElement lastElement = _ui.outputContainer.Children().LastOrDefault();

        if (lastElement is not null)
        {
            _ui.outputContainer.ScrollTo(lastElement);
        }
    }
}
