using System;
using UnityEngine;



public class UIManager : MonoBehaviour
{
    [SerializeField] private PlayerInteractor interact;
    [SerializeField] private GameObject settingsGameObject;
    [SerializeField] private Material backgroundBlur;
    public event Func<bool> SettingToggles;
    public bool SettingOpen {  get; private set; }
    public UIState CurrentState { get; private set; }

    void Awake() {
        interact.openSettings += ToggleSettings;
        SetBlur(false);
    }

    void Update() {
        UIStateDefine(CurrentState);
    }

    private void UIStateDefine(UIState state) {
        switch (state) {
            case UIState.Gameplay:
                interact?.SetInputLock(false);
                SetBlur(false);
                Time.timeScale = 1f;
                break;
            case UIState.SemiPause:
                interact?.SetInputLock(true);
                interact?.FreeMouse(true);
                SetBlur(true);
                Time.timeScale = 1f;
                break;
            case UIState:
                interact?.SetInputLock(true);
                interact?.FreeMouse(true);
                SetBlur(true);
                Time.timeScale = 0f;
                break;
        }
    }

    private void SetBlur(bool active) {
        backgroundBlur.SetFloat("_BlurStrength", active ? 1.4f : 0f);
        backgroundBlur.SetFloat("_Darken", active ? .5f : 0f);
    }

    public void ToggleSettings() {
        if (!SettingOpen) {
            OpenSettings();
        } else {
            if (SettingToggles.Invoke()) return;
            CloseSettings();
        }
    }

    private void OpenSettings() {
        SettingOpen = true;
        settingsGameObject?.SetActive(true);
        CurrentState = UIState.SemiPause;
    }

    private void CloseSettings() {
        SettingOpen = false;
        settingsGameObject?.SetActive(false);
        CurrentState = UIState.Gameplay;
    }
}
