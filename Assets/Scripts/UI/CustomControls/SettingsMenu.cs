using System;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;

[UxmlElement]
public partial class SettingsMenu : VisualElement
{
    public const string PLAYER_PREFS_FULLSCREEN_ENABLED_KEY = "fullscreenEnabled";
    public const string PLAYER_PREFS_AUTO_ENABLED_KEY = "autoEnabled";
    public const string PLAYER_PREFS_VOLUME_KEY = "volume";

    private const long VOLUME_HOLD_INITIAL_DELAY_MS = 400;
    private const long VOLUME_HOLD_REPEAT_INTERVAL_MS = 75;

    private int _volume = PlayerPrefs.GetInt(PLAYER_PREFS_VOLUME_KEY, 100);

    private readonly Button _fullscreenButton = new()
    {
        text = PlayerPrefs.GetInt(PLAYER_PREFS_FULLSCREEN_ENABLED_KEY, 1) == 0 ? "Enable Fullscreen" : "Enabled Windowed",
        style =
        {
            backgroundColor = Color.clear,
            paddingLeft = 0,
            marginLeft = 0
        }
    };

    private readonly Button _autoButton = new()
    {
        text = PlayerPrefs.GetInt(PLAYER_PREFS_AUTO_ENABLED_KEY, 1) == 0 ? "Enable Auto" : "Disable Auto",
        style =
        {
            backgroundColor = Color.clear,
            paddingLeft = 0,
            marginLeft = 0
        }
    };

    public SettingsMenu()
    {
        style.display = DisplayStyle.Flex;
        style.flexDirection = FlexDirection.Column;
        style.justifyContent = Justify.SpaceAround;
        style.alignItems = Align.FlexStart;
        style.unityFont = Resources.Load<Font>("Fonts/ronix");
        style.unityFontDefinition = new StyleFontDefinition(
            Resources.Load<FontAsset>("Fonts/ronix")
        );
        style.color = Color.white;
        style.backgroundColor = UiUtils.semiTransparentBlackColor;
        style.width = 400;
        style.paddingTop = 16;
        style.paddingBottom = 16;
        style.paddingLeft = 16;
        style.paddingRight = 16;
        style.borderTopLeftRadius = 8;
        style.borderTopRightRadius = 8;
        style.borderBottomLeftRadius = 8;
        style.borderBottomRightRadius = 8;

        UiUtils.ToggleBorder(_fullscreenButton, false);
        _fullscreenButton.clicked += () =>
        {
            int isFullscreen = PlayerPrefs.GetInt(PLAYER_PREFS_FULLSCREEN_ENABLED_KEY, 1);
            if (isFullscreen == 0)
            {
                PlayerPrefs.SetInt(PLAYER_PREFS_FULLSCREEN_ENABLED_KEY, 1);
                _fullscreenButton.text = "Enable Windowed";
            }
            else
            {
                PlayerPrefs.SetInt(PLAYER_PREFS_FULLSCREEN_ENABLED_KEY, 0);
                _fullscreenButton.text = "Enable Fullscreen";
            }
        };

        UiUtils.ToggleBorder(_autoButton, false);
        _autoButton.clicked += () =>
        {
            int autoEnabled = PlayerPrefs.GetInt(PLAYER_PREFS_AUTO_ENABLED_KEY, 1);
            if (autoEnabled == 0)
            {
                PlayerPrefs.SetInt(PLAYER_PREFS_AUTO_ENABLED_KEY, 1);
                _autoButton.text = "Disable Auto";
            }
            else
            {
                PlayerPrefs.SetInt(PLAYER_PREFS_AUTO_ENABLED_KEY, 0);
                _autoButton.text = "Enable Auto";
            }
        };

        Add(_fullscreenButton);
        Add(_autoButton);

        InitializeVolumeSettings();
    }

    private void InitializeVolumeSettings()
    {
        VisualElement container = new()
        {
            style =
            {
                display = DisplayStyle.Flex,
                flexDirection = FlexDirection.Row,
                alignItems = Align.Center
            }
        };

        Label volumeLabel = new($"{_volume}%");

        Button minus = new()
        {
            text = "-",
            style =
            {
                backgroundColor = Color.clear
            }
        };
        UiUtils.ToggleBorder(minus, false);

        void DecreaseVolume()
        {
            _volume = Math.Max(_volume - 1, 0);
            PlayerPrefs.SetInt(PLAYER_PREFS_VOLUME_KEY, _volume);
            volumeLabel.text = $"{_volume}%";
        }

        RegisterHoldButton(minus, DecreaseVolume);

        Button plus = new()
        {
            text = "+",
            style =
            {
                backgroundColor = Color.clear
            }
        };
        UiUtils.ToggleBorder(plus, false);

        void IncreaseVolume()
        {
            _volume = Math.Min(_volume + 1, 100);
            PlayerPrefs.SetInt(PLAYER_PREFS_VOLUME_KEY, _volume);
            volumeLabel.text = $"{_volume}%";
        }

        RegisterHoldButton(plus, IncreaseVolume);

        container.Add(new Label("Volume"));
        container.Add(minus);
        container.Add(volumeLabel);
        container.Add(plus);

        Add(container);
    }

    private static void RegisterHoldButton(Button button, Action step)
    {
        IVisualElementScheduledItem repeatTask = null;

        void StopRepeating()
        {
            repeatTask?.Pause();
            repeatTask = null;
        }

        button.RegisterCallback<PointerDownEvent>(
            _ =>
            {
                step();
                repeatTask = button.schedule.Execute(step)
                    .StartingIn(VOLUME_HOLD_INITIAL_DELAY_MS)
                    .Every(VOLUME_HOLD_REPEAT_INTERVAL_MS);
            },
            TrickleDown.TrickleDown
        );

        button.RegisterCallback<PointerUpEvent>(_ => StopRepeating(), TrickleDown.TrickleDown);
        button.RegisterCallback<PointerLeaveEvent>(_ => StopRepeating(), TrickleDown.TrickleDown);
        button.RegisterCallback<PointerCaptureOutEvent>(
            _ => StopRepeating(),
            TrickleDown.TrickleDown
        );
    }
}
