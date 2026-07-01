using System;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private PlayerInteractor interact;
    [SerializeField] private PlayerHealth health;
    [SerializeField] private GameObject settingsGameObject;
    public event Func<bool> SettingToggles;
    public bool SettingOpen {  get; private set; }
    public bool PlayerDown { get; private set; }
    public UIState CurrentState { get; private set; }

    void Awake() {
        interact.openSettings += ToggleSettings;
        health.OnDown += PlayerIsDown;
    }

    void Update() {
        UIStateDefine(CurrentState);
    }

    private void UIStateDefine(UIState state) {
        switch (state) {
            case UIState.Gameplay:
                interact?.SetInputLock(false);
                SetBlur();
                Time.timeScale = 1f;
                break;
            case UIState.SemiPause:
                interact?.SetInputLock(true);
                interact?.FreeMouse(true);
                SetBlur();
                Time.timeScale = 1f;
                break;
            case UIState:
                interact?.SetInputLock(true);
                interact?.FreeMouse(true);
                SetBlur();
                Time.timeScale = 0f;
                break;
        }
    }

    private void PlayerIsDown(bool down) {
        if (down) PlayerDown = true;
        else PlayerDown = false;
    }

    private void SetBlur() {    
        if (BlurShader.Instance == null) return;
        BlurShader.Instance.Enabled = PlayerDown || SettingOpen;
        if (PlayerDown) {
            BlurShader.Instance.BlurSize = 6f;
            BlurShader.Instance.Darken = .8f;
        } else {
            BlurShader.Instance.BlurSize = 2f;
            BlurShader.Instance.Darken = .15f;
        }
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
