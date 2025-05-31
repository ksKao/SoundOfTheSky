using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;

[UxmlElement]
public partial class SaveMenu : VisualElement
{
    public const string PLAYER_PREFS_SAVE_FILE_TO_LOAD_KEY = "saveFileToLoad";

    private int _selectedIndex = -1;
    private readonly Action _onLoad = null;
    private readonly Action _onSave = null;
    private readonly Label _titleLabel = new();
    private readonly List<Label> _saveFileLabels = new(5);
    private readonly Button _loadButton = new()
    {
        text = "LOAD",
        style =
        {
            backgroundColor = Color.clear,
            borderTopWidth = 0,
            borderBottomWidth = 0,
            borderLeftWidth = 0,
            borderRightWidth = 0,
            display = DisplayStyle.None,
        },
    };
    private readonly Button _saveButton = new()
    {
        text = "SAVE",
        style =
        {
            backgroundColor = Color.clear,
            borderTopWidth = 0,
            borderBottomWidth = 0,
            borderLeftWidth = 0,
            borderRightWidth = 0,
            display = DisplayStyle.None,
        },
    };
    private readonly Button _deleteButton = new()
    {
        text = "DELETE",
        style =
        {
            backgroundColor = Color.clear,
            borderTopWidth = 0,
            borderBottomWidth = 0,
            borderLeftWidth = 0,
            borderRightWidth = 0,
            display = DisplayStyle.None,
        },
    };

    public int SelectedIndex
    {
        get => _selectedIndex;
        set
        {
            for (int i = 0; i < _saveFileLabels.Count; i++)
            {
                _saveFileLabels[i].text = value == i ? $"<u>SAVE {i + 1}</u>" : $"SAVE {i + 1}";
            }

            _selectedIndex = value;

            if (value < 0 || value > 4)
            {
                _loadButton.style.display = DisplayStyle.None;
                _saveButton.style.display = DisplayStyle.None;
                _deleteButton.style.display = DisplayStyle.None;
            }
            else
            {
                _loadButton.style.display = _onLoad is null ? DisplayStyle.None : DisplayStyle.Flex;
                _saveButton.style.display = _onSave is null ? DisplayStyle.None : DisplayStyle.Flex;
                _deleteButton.style.display = DisplayStyle.Flex;
            }
        }
    }

    public SaveMenu()
    {
        Debug.LogWarning($"Detected calling default constructor of {nameof(SaveMenu)}");
    }

    public SaveMenu(
        string title,
        Action onCancel = null,
        Action onNew = null,
        Action onLoad = null,
        Action onSave = null
    )
    {
        _onLoad = onLoad;
        _onSave = onSave;

        style.display = DisplayStyle.Flex;
        style.flexDirection = FlexDirection.Column;
        style.justifyContent = Justify.SpaceAround;
        style.unityFont = Resources.Load<Font>("Fonts/ronix");
        style.unityFontDefinition = new StyleFontDefinition(
            Resources.Load<FontAsset>("Fonts/ronix")
        );
        style.color = Color.white;
        style.backgroundColor = UiUtils.semiTransparentBlackColor;
        style.minHeight = UiUtils.GetLengthPercentage(50);
        style.width = UiUtils.GetLengthPercentage(20);
        style.borderTopLeftRadius = 8;
        style.borderTopRightRadius = 8;
        style.borderBottomLeftRadius = 8;
        style.borderBottomRightRadius = 8;

        VisualElement topContainer = new()
        {
            style =
            {
                display = DisplayStyle.Flex,
                flexDirection = FlexDirection.Column,
                justifyContent = Justify.Center,
                alignItems = Align.Center,
                width = UiUtils.GetLengthPercentage(100),
                marginTop = 16,
                marginBottom = 24,
            },
        };

        // title
        _titleLabel.text = title.ToUpper();
        topContainer.Add(_titleLabel);

        if (onCancel is not null)
        {
            Button cancelButton = new()
            {
                text = "Cancel",
                style =
                {
                    backgroundColor = Color.clear,
                    borderTopWidth = 0,
                    borderBottomWidth = 0,
                    borderLeftWidth = 0,
                    borderRightWidth = 0,
                },
            };

            cancelButton.clicked += () =>
            {
                onCancel();
            };

            topContainer.Add(cancelButton);
        }

        if (onNew is not null)
        {
            Button newSaveFileButton = new()
            {
                text = "New",
                style =
                {
                    backgroundColor = Color.clear,
                    borderTopWidth = 0,
                    borderBottomWidth = 0,
                    borderLeftWidth = 0,
                    borderRightWidth = 0,
                },
            };

            newSaveFileButton.clicked += () =>
            {
                PlayerPrefs.SetInt(PLAYER_PREFS_SAVE_FILE_TO_LOAD_KEY, -1);
                PlayerPrefs.Save();
                onNew();
            };

            topContainer.Add(newSaveFileButton);
        }

        _deleteButton.clicked += () =>
        {
            UiManager.Instance.ShowConfirmationModal(
                "Are you sure you want to delete this save file? This action cannot be reverted.",
                () =>
                {
                    UiManager.Instance.ShowModal(new SaveMenu(title, onCancel, onNew));
                },
                () =>
                {
                    UiManager.Instance.ShowModal(new SaveMenu(title, onCancel, onNew));
                }
            );
        };

        if (_onLoad is not null)
            _loadButton.clicked += () =>
            {
                PlayerPrefs.SetInt(PLAYER_PREFS_SAVE_FILE_TO_LOAD_KEY, SelectedIndex);
                _onLoad?.Invoke();
            };

        if (_onSave is not null)
            _saveButton.clicked += () =>
            {
                PlayerPrefs.SetInt(PLAYER_PREFS_SAVE_FILE_TO_LOAD_KEY, SelectedIndex);
                _onSave?.Invoke();
            };

        topContainer.Add(_loadButton);
        topContainer.Add(_saveButton);
        topContainer.Add(_deleteButton);

        VisualElement bottomContainer = new()
        {
            style =
            {
                display = DisplayStyle.Flex,
                flexDirection = FlexDirection.Column,
                justifyContent = Justify.Center,
                alignItems = Align.Center,
                width = UiUtils.GetLengthPercentage(100),
            },
        };

        for (int i = 0; i < 5; i++)
        {
            Button saveFileButton = new()
            {
                style =
                {
                    marginBottom = 8,
                    backgroundColor = Color.clear,
                    borderTopWidth = 0,
                    borderBottomWidth = 0,
                    borderLeftWidth = 0,
                    borderRightWidth = 0,
                },
            };
            saveFileButton.SetEnabled(
                (File.Exists(CityModeManager.GetSaveFilePath(i)) && _onLoad is not null)
                    || _onSave is not null
            );
            Label saveFileButtonLabel = new($"SAVE {i + 1}");
            saveFileButton.Add(saveFileButtonLabel);
            int thisButtonIndex = i;

            saveFileButton.clicked += () =>
            {
                SelectedIndex = thisButtonIndex;
            };

            _saveFileLabels.Add(saveFileButtonLabel);
            bottomContainer.Add(saveFileButton);
        }

        Add(topContainer);
        Add(bottomContainer);
    }
}
