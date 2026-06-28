using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public partial class UISettings : MonoBehaviour
{

    [Header("Preferences MinMax")]
    [SerializeField] private float minFps = 30;
    [SerializeField] private float maxFps = 241;

    private Dictionary<OptionIDs, UIOptions> optionList;

    private void OnCreateSliderFunc() {
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
                SaveManager.Instance.fps = fps;
                if (QualitySettings.vSyncCount == 0) Application.targetFrameRate = fps;
            });

        Dropdown(OptionIDs.VSync, SaveManager.Instance.vsync ? 1 : 0,
            index => {
                switch (index) {
                    case 0:
                        Application.targetFrameRate = SaveManager.Instance.fps;
                        QualitySettings.vSyncCount = 0;
                        break;
                    case 1:
                        Application.targetFrameRate = -1;
                        QualitySettings.vSyncCount = 1; 
                        break;
                    default:
                        return;
                }
                SaveManager.Instance.vsync = index == 1;
                if (!optionList.TryGetValue(OptionIDs.FPSLimit, out UIOptions fps)) return;
                if (fps is not UISliderTypeB fpsSlider) return;
                fpsSlider.Slider.interactable = index == 0;
            });
    }

    //HELPERS

    private void SliderTypeA(OptionIDs id, float value, UnityAction<float> onChanged) {
        if (!optionList.TryGetValue(id, out UIOptions option)) return;

        if (option is not UISliderTypeA sliderUI) return;

        sliderUI.Slider.onValueChanged.RemoveAllListeners();

        sliderUI.Slider.SetValueWithoutNotify(value);
        sliderUI.UpdateDisplay(value);

        sliderUI.Slider.onValueChanged.AddListener(value => {
            sliderUI.UpdateDisplay(value);
            onChanged.Invoke(value);
        });
    }

    private void SliderTypeB(OptionIDs id, float value, float min, float max,
        bool wholeNumbers, Func<float, string> textFormat, Action<float> apply) {

        if (!optionList.TryGetValue(id, out UIOptions option)) return;

        if (option is not UISliderTypeB sliderUI) return;

        sliderUI.Slider.onValueChanged.RemoveAllListeners();
        sliderUI.Input.onEndEdit.RemoveAllListeners();

        sliderUI.Slider.minValue = min;
        sliderUI.Slider.maxValue = max;
        sliderUI.Slider.wholeNumbers = wholeNumbers;

        SetSliderBValue(sliderUI, value, textFormat);

        sliderUI.Slider.onValueChanged.AddListener(value => {
            SetSliderBValue(sliderUI, value, textFormat);
            apply(value);
        });

        sliderUI.Input.onEndEdit.AddListener(text => {
            if (!float.TryParse(text, out float val)) val = value;
            
            val = Mathf.Clamp(val, min, max);

            if (wholeNumbers) val = Mathf.Round(val);

            SetSliderBValue(sliderUI, val, textFormat);
            apply(val);

        });
    }

    private void SetSliderBValue(UISliderTypeB sliderUI, float value, Func<float, string> textFormat) {
        sliderUI.Slider.SetValueWithoutNotify(value);
        sliderUI.Input.SetTextWithoutNotify(textFormat(value));
    }

    private void Dropdown(OptionIDs id, int value, Action<int> apply) {
        if (!optionList.TryGetValue(id, out UIOptions option)) return;
        if (option is not UIDropdown dropdown) return;

        dropdown.Dropdown.onValueChanged.RemoveAllListeners();
        dropdown.Dropdown.SetValueWithoutNotify(value);

        dropdown.Dropdown.onValueChanged.AddListener(text => {
            apply(text);
        });
    }
}