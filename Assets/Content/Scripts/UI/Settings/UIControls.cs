using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIControls : ASettingsInterfaces
{

    void Awake() {
        optionList = Build();
        StartCoroutine(OnLaunch());
    }

    private IEnumerator OnLaunch() {
        yield return new WaitUntil(() =>
            SaveManager.Instance != null && SaveManager.Instance.init &&
            SettingsManager.Instance != null
        );

        GenerateControlFuncs();
    }

    //Functionalities

    private void GenerateControlFuncs() {
        SliderTypeB(OptionIDs.FOV,
            SaveManager.Instance.fov,
            SettingsManager.Instance.minFov, SettingsManager.Instance.maxFov, true,
            value => value.ToString(),
            value => {
                SettingsManager.Instance.UpdateFOV(Mathf.RoundToInt(value));
            });

        SliderTypeB(OptionIDs.Sensitivity,
            SaveManager.Instance.sensitivity,
            SettingsManager.Instance.minSens, SettingsManager.Instance.maxSens, false,
            value => (value).ToString("F2"),
            value => {
                SettingsManager.Instance.UpdateSens(value);
            });

        Keybind(OptionIDs.MoveFW,
            SaveManager.Instance.moveFW,
            key => SaveManager.Instance.moveFW = key
            );

        Keybind(OptionIDs.MoveBW,
            SaveManager.Instance.moveBW,
            key => SaveManager.Instance.moveBW = key
            );

        Keybind(OptionIDs.MoveL,
            SaveManager.Instance.moveL,
            key => SaveManager.Instance.moveL = key
            );

        Keybind(OptionIDs.MoveR,
            SaveManager.Instance.moveR,
            key => SaveManager.Instance.moveR = key
            );

        Keybind(OptionIDs.Interact,
            SaveManager.Instance.interact,
            key => SaveManager.Instance.interact = key
            );

        Keybind(OptionIDs.Sprint,
            SaveManager.Instance.sprint,
            key => SaveManager.Instance.sprint = key
            );

        Dropdown(OptionIDs.SprintToggle,
            SaveManager.Instance.sprintToggle ? 1 : 0,
            value => {
                SaveManager.Instance.sprintToggle = value == 1;
            });
    }
}
