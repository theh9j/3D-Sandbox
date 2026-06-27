using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class UISliderTypeA : UISlider
{
    [SerializeField] private AudioType type;

    void Awake() {
        slider.onValueChanged.AddListener(UpdateSave);
        StartCoroutine(OnLaunch());
    }

    private IEnumerator OnLaunch() {
        yield return new WaitUntil(() =>
            SaveManager.Instance != null && SaveManager.Instance.init &&
            AudioManager.Instance != null
        );

        slider.SetValueWithoutNotify(SetValue());
        UpdateSliderText();
    }

    private float SetValue() {
        return (type) switch {
            AudioType.Main => SaveManager.Instance.mainVolume,
            AudioType.Music => SaveManager.Instance.musicVolume,
            AudioType.SFX => SaveManager.Instance.sfxVolume,
            AudioType.Environment => SaveManager.Instance.enviromentVolume,
            _ => 1f
        };
    }

    protected override void UpdateSave(float value) {
        switch (type) {
            case AudioType.Main:
                AudioManager.Instance.SetMainVolume(value);
                break;
            case AudioType.Music:
                AudioManager.Instance.SetMusicVolume(value);
                break;
            case AudioType.SFX:
                AudioManager.Instance.SetSFXVolume(value);
                break;
            case AudioType.Environment:
                AudioManager.Instance.SetEnvironmentVolume(value);
                break;
            default:
                return;
        }
    }

    protected override string GetText(float value) {
        return (value * 100).ToString("F0") + "%"; 
    }
}
