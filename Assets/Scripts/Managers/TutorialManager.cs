using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TutorialManager : Singleton<TutorialManager>
{
    private int _stepIndex = -1;
    private readonly List<(
        Func<VisualElement> getEl,
        string msg,
        bool clickable,
        Action onClick
    )> _steps = new();

    protected override void Awake()
    {
        base.Awake();

        PlayerPrefs.SetInt(SaveMenu.PLAYER_PREFS_SAVE_FILE_TO_LOAD_KEY, -1); // do not load save file
    }

    private void Start()
    {
        CityModeManager.Instance.PendingMissions.Clear();
        CityModeManager.Instance.PendingMissions.Add(new RescueMission(true));

        CityModeManager.Instance.crews.Clear();

        CityModeManager.Instance.IncrementMaterialValue(MaterialType.Payments, 9900);
        CityModeManager.Instance.IncrementMaterialValue(MaterialType.Supplies, 9900);
        CityModeManager.Instance.IncrementMaterialValue(MaterialType.Resources, 9900);

        UiManager.Instance.CityModeScreen.RefreshMissionList(
            UiManager.Instance.CityModeScreen.missionTypeTab.ActiveTab
        );

        // _steps.Add(
        //     (
        //         () => UiManager.Instance.CityModeScreen.materialBar,
        //         "You can see the materials that you own here. Each mission type will require different materials to deploy.",
        //         false,
        //         null
        //     )
        // );

        // _steps.Add(
        //     (
        //         () => UiManager.Instance.CityModeScreen.pendingMissionList,
        //         "This is the pending mission list. You can deploy different types of mission in here. Let's try to deploy a rescue mission",
        //         false,
        //         null
        //     )
        // );

        // _steps.Add(
        //     (
        //         () => UiManager.Instance.CityModeScreen.missionTypeTab,
        //         "You can filter different types of mission to deploy here.",
        //         false,
        //         null
        //     )
        // );

        _steps.Add(
            (
                () => UiManager.Instance.CityModeScreen.bottomNavigationBar.trainButton,
                "Before we can deploy a rescue mission. We'll have to first get a train. Click here to view the list of trains available.",
                true,
                null
            )
        );

        _steps.Add(
            (
                () => UiManager.Instance.CityModeScreen.trainList.TrainCards[0],
                "Let's buy the first train",
                false,
                null
            )
        );

        _steps.Add(
            (
                () => UiManager.Instance.CityModeScreen.bottomNavigationBar.crewButton,
                "Click this to show the list of crews",
                true,
                null
            )
        );

        IncrementIndex();
    }

    private void IncrementIndex()
    {
        if (_stepIndex >= _steps.Count - 1)
            return;

        _stepIndex++;

        (Func<VisualElement> getEl, string msg, bool clickable, Action onClick) = _steps[
            _stepIndex
        ];

        UiManager.Instance.FocusElement(getEl(), msg, onClick ?? IncrementIndex, clickable);
    }
}
