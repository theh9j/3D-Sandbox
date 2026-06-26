using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UISettings : MonoBehaviour
{
    [SerializeField] private UIManager ui;
    [SerializeField] private List<Button> settings = new(4);
    private List<UnityAction> actionList;
    private List<Transform> settingButtonsTransforms;

    void Start() {
        settingButtonsTransforms = new(settings.Count);

        actionList = new(settings.Count) {
            Continue,
            Preferences,
            Keybinds,
            Quit
        };

        for (int i = 0; i < settings.Count; i++) {
            settingButtonsTransforms.Add(settings[i].transform);
            settings[i].onClick.AddListener(actionList[i]);
        }

    }

    private void Continue() {
        ui.ToggleSettings();
    }

    private void Preferences() {

    }

    private void Keybinds() {

    }

    private void Quit() {
        ui.ToggleSettings();
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit(); 
    #endif
    }
}
