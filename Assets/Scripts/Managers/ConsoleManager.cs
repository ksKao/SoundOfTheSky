using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class ConsoleManager : Singleton<ConsoleManager>
{
    private ConsoleUi _ui;
    private readonly List<string> _commandHistory = new();
    private int _currentNavigationIndex = 0;

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

        InputManager.Instance.InputAction.CityMode.OpenConsole.performed += ctx => OpenConsole();
        InputManager.Instance.InputAction.Console.Close.performed += ctx => CloseConsole();
        InputManager.Instance.InputAction.Console.Submit.performed += ctx => SubmitCommand();
        InputManager.Instance.InputAction.Console.PreviousCommand.performed += ctx =>
            CycleCommand(true);
        InputManager.Instance.InputAction.Console.NextCommand.performed += ctx =>
            CycleCommand(false);
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
        InputManager.Instance.InputAction.CityMode.Disable();
        InputManager.Instance.InputAction.Console.Enable();
        UiManager.Instance.CityModeScreen.Add(_ui);
        StartCoroutine(FocusTextField());
    }

    private void CloseConsole()
    {
        InputManager.Instance.InputAction.CityMode.Enable();
        InputManager.Instance.InputAction.Console.Disable();
        UiManager.Instance.CityModeScreen.Remove(_ui);
    }

    private void SubmitCommand()
    {
        try
        {
            string input = _ui.textField.value;

            _commandHistory.Add(input);

            Output(input);

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
            _currentNavigationIndex = 0;
            StartCoroutine(FocusTextField());
            StartCoroutine(ScrollToBottom());
        }
    }

    private void CycleCommand(bool isUp)
    {
        _currentNavigationIndex += isUp ? 1 : -1;

        // Ensure we are not trying to access an out-of-bounds index
        _currentNavigationIndex = Math.Clamp(_currentNavigationIndex, 1, _commandHistory.Count);

        _ui.textField.value = _commandHistory[^_currentNavigationIndex];
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
