using System;
using UnityEngine;

public enum UIState {
    Gameplay,
    SemiPause,
    Pause,
}

public class UIManager : MonoBehaviour
{
    [SerializeField] private PlayerInteractor interact;
    [SerializeField] private GameObject settings;
    [SerializeField] private Material backgroundBlur;

    public bool SettingOpen {  get; private set; }
    public UIState CurrentState { get; private set; }

    void Awake() {
        interact.openSettings += ToggleSettings;
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
        backgroundBlur.SetFloat("_Darken", active ? .7f : 0f);
    }

    public void ToggleSettings() {
        if (SettingOpen) {
            CloseSettings();
        } else {
            OpenSettings();
        }
    }

    public void OpenSettings() {
        SettingOpen = true;
        settings?.SetActive(true);
        CurrentState = UIState.SemiPause;
    }

    public void CloseSettings() {
        SettingOpen = false;
        settings?.SetActive(false);
        CurrentState = UIState.Gameplay;
    }
}
