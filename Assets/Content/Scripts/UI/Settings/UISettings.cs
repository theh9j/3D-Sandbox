using DG.Tweening;
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
    [SerializeField] private Button[] backButton;

    [Header("Preferences")]
    [SerializeField] private GameObject preferenceSettings;
    [SerializeField] private ScrollRect prefScroll;

    private Transform preferencesTrans;

    [Header("Controls")]
    [SerializeField] private UIControls ctrl;
    [SerializeField] private GameObject controlSettings;
    [SerializeField] private ScrollRect ctrlScroll;

    private Transform controlsTrans;
    private Tween prefTween;

    private List<UnityAction> actionList;
    private List<Transform> settingButtonsTransforms;

    public bool Pref { get; private set; }
    public bool Ctrl { get; private set; }

    void Awake() {
        preferencesTrans = preferenceSettings.transform;
        controlsTrans = controlSettings.transform;

        ui.SettingToggles += ToggleSettings;
    }

    void Start() {
        settingButtonsTransforms = new(settings.Count);

        actionList = new(settings.Count) {
            Continue,
            Preferences,
            Controls,
            Quit
        };

        foreach (Button button in backButton) {
            button.onClick.AddListener(() => {
                if (Pref) ClosePreferences();
                if (Ctrl) CloseControls();
            });
        }

        for (int i = 0; i < settings.Count; i++) {
            settingButtonsTransforms.Add(settings[i].transform);
            settings[i].onClick.AddListener(actionList[i]);
        }

    }

    private bool ToggleSettings() {
        if (!Pref && !Ctrl) return false;
        if (Pref) ClosePreferences();
        if (Ctrl) CloseControls();
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
        if (!Ctrl) OpenControls();
        else CloseControls();
    }

    private void Quit() {
        ui.ToggleSettings();
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit(); 
    #endif
    }

    //SETTINGS CONTROLS

    private void OpenControls() {
        if (controlSettings.activeSelf) return;
        OpenNClosePanel(true, controlsTrans, false);
    }

    private void CloseControls() {
        if (!controlSettings.activeSelf || ctrl.WaitingForKey) return;
        OpenNClosePanel(false, controlsTrans, false);
    }

    private void SetActivityCtrl(bool active) {
        controlSettings.SetActive(active);
        Ctrl = active;
        if (active) ResetScroll(ctrlScroll); 
    }

    //SETTINGS PREFERENCES
    

    private void OpenPreferences() {
        if (preferenceSettings.activeSelf) return;
        OpenNClosePanel(true, preferencesTrans, true);
    }

    private void ClosePreferences() {
        if (!preferenceSettings.activeSelf) return;
        OpenNClosePanel(false, preferencesTrans, true);
    }

    private void SetActivityPref(bool active) {
        preferenceSettings.SetActive(active);
        Pref = active;
        if (active) ResetScroll(prefScroll);
    }


    // PREFERENCES HELPERS


    private Vector2 GetBottomPos() {
        float canvasHeight = canvas.rect.height;
        float panelHeight = (preferencesTrans as RectTransform).rect.height;

        return new Vector2(
            0f, -(canvasHeight / 2f + panelHeight / 2f + startOffset));
    }

    private bool IsPrefAnimating() {
        return prefTween != null && prefTween.IsActive() && prefTween.IsPlaying();
    }

    //SETTINGS GLOBAL HELPERS

    private void OpenNClosePanel(bool open, Transform movingPanel, bool type) {
        if (IsPrefAnimating()) return;
        if (open) {
            prefTween = movingPanel.DOLocalMove(Vector3.zero, .15f)
            .From(GetBottomPos())
            .SetEase(Ease.OutBack, 2f)
            .OnStart(() => {
                if (type) SetActivityPref(true);
                else SetActivityCtrl(true);
            })
            .OnKill(() => { prefTween = null; });
        } else {
            prefTween = movingPanel.DOLocalMove(GetBottomPos(), .15f)
            .OnComplete(() => {
                if (type) SetActivityPref(false);
                else SetActivityCtrl(false);
            })
            .OnKill(() => { prefTween = null; });
        }
    }

    private void ResetScroll(ScrollRect rect) {
        Canvas.ForceUpdateCanvases();
        rect.verticalNormalizedPosition = 1f;
    }
}
