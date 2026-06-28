using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public partial class UISettings : MonoBehaviour
{
    [Header("General Settings")]
    [SerializeField] private UIManager ui;
    [SerializeField] private List<Button> settings = new(4);
    [SerializeField] private RectTransform canvas;
    [SerializeField] private float startOffset;
    private List<UnityAction> actionList;
    private List<Transform> settingButtonsTransforms;

    public bool Pref { get; private set; }
    public bool Ctrl { get; private set; }

    void Awake() {
        preferencesTrans = preferenceSettings.transform;

        ui.SettingToggles += ToggleSettings;

        optionList = Build();
        OnCreateSliderFunc();
    }

    void Start() {
        settingButtonsTransforms = new(settings.Count);

        actionList = new(settings.Count) {
            Continue,
            Preferences,
            Controls,
            Quit
        };

        for (int i = 0; i < settings.Count; i++) {
            settingButtonsTransforms.Add(settings[i].transform);
            settings[i].onClick.AddListener(actionList[i]);
        }

    }

    private bool ToggleSettings() {
        if (!Pref && !Ctrl) return false;
        Preferences();

        return true;
    }

    private void Continue() {
        ui.ToggleSettings();
    }

    private void Preferences() {
        if (!Pref) OpenPreferences();
        else ClosePreferences();
    }

    private void Controls() {

    }

    private void Quit() {
        ui.ToggleSettings();
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit(); 
    #endif
    }

    //SETTINGS GLOBAL HELPERS

    private void ResetScroll(ScrollRect rect) {
        Canvas.ForceUpdateCanvases();
        rect.verticalNormalizedPosition = 1f;
    }
}
