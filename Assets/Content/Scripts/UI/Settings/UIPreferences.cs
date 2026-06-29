using System.Collections;
using UnityEngine;

public class UIPreferences : ASettingsInterfaces
{
    private float minFps = 30;
    private float maxFps = 241;

    void Awake() {
        optionList = Build();
        StartCoroutine(OnLaunch());
    }

    private IEnumerator OnLaunch() {
        yield return new WaitUntil(() =>
            SaveManager.Instance != null && SaveManager.Instance.init &&
            SettingsManager.Instance != null
        );
        minFps = SettingsManager.Instance.minFps;
        maxFps = SettingsManager.Instance.maxFps;
        GeneratePreferencesFuncs();
    }

    private void GeneratePreferencesFuncs() {
        SliderTypeA(OptionIDs.MainVolume,
            SaveManager.Instance.mainVolume,
            AudioManager.Instance.SetMainVolume
            );

        SliderTypeA(OptionIDs.MusicVolume,
            SaveManager.Instance.musicVolume,
            AudioManager.Instance.SetMusicVolume
            );

        SliderTypeA(OptionIDs.SFXVolume,
            SaveManager.Instance.sfxVolume,
            AudioManager.Instance.SetSFXVolume
            );

        SliderTypeA(OptionIDs.EnvironmentVolume,
            SaveManager.Instance.enviromentVolume,
            AudioManager.Instance.SetEnvironmentVolume
            );

        SliderTypeB(OptionIDs.FPSLimit,
            SaveManager.Instance.fps == -1 ? maxFps : SaveManager.Instance.fps,
            minFps, maxFps, true,
            value => Mathf.Approximately(value, maxFps) ? "Unlimited" : value.ToString(),
            value => {
                int fps = Mathf.Approximately(value, maxFps) ? -1 : Mathf.RoundToInt(value);
                if (QualitySettings.vSyncCount == 0) SettingsManager.Instance.SetFPS(fps);
            });

        Dropdown(OptionIDs.VSync, SaveManager.Instance.vsync ? 1 : 0,
            index => {
                SettingsManager.Instance.SetVSync(index == 1);
                if (!optionList.TryGetValue(OptionIDs.FPSLimit, out UIOptions fps)) return;
                if (fps is not UISliderTypeB fpsSlider) return;
                fpsSlider.Slider.interactable = index == 0;
            });

        Dropdown(OptionIDs.AntiAliasing, SaveManager.Instance.antiAlias,
            index => {
                SettingsManager.Instance.SetAntiAlias(index);
            });
    }
    
}