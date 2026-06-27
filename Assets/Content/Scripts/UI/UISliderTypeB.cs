using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class UISliderTypeB : UISlider
{
    [SerializeField] private UIOptions vsync;
    [SerializeField] private TMP_InputField fpsInput;
    [SerializeField] private int minFPS = 30;
    [SerializeField] private int maxFPS = 240; //-1 ~ No cap

    private void Awake() {
        slider.minValue = minFPS;
        slider.maxValue = maxFPS;
        slider.wholeNumbers = true;

        vsync.OnVSyncLock += OnLock;
        slider.onValueChanged.AddListener(UpdateSave);
        fpsInput.onEndEdit.AddListener(OnInput);
        StartCoroutine(OnLaunch());
    }

    private IEnumerator OnLaunch() {
        yield return new WaitUntil(() =>
            SaveManager.Instance != null && SaveManager.Instance.init &&
            AudioManager.Instance != null
        );
        int fps = SaveManager.Instance.fps;
        ApplyFPS(fps);
        slider.SetValueWithoutNotify(fps);
        fpsInput.SetTextWithoutNotify(GetText(fps));
        text.text = GetText(fps);
    }

    protected override void UpdateSave(float value) {
        int fps = Mathf.RoundToInt(value);

        SaveManager.Instance.fps = fps;
        ApplyFPS(fps);

        string display = GetText(fps);
        fpsInput.SetTextWithoutNotify(display);
        text.text = display;
    }

    private void OnInput(string value) {
        if (!int.TryParse(value, out int fps))
            fps = SaveManager.Instance.fps;

        fps = Mathf.Clamp(fps, minFPS, maxFPS);

        slider.SetValueWithoutNotify(fps);
        UpdateSave(fps);
    }

    protected override string GetText(float fps) {
        return Mathf.Approximately(fps, maxFPS) ? "Unlimited" : fps.ToString("F0");
    }

    private void ApplyFPS(int fps) {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = fps >= maxFPS ? -1 : fps;
    }

    private void OnLock(bool unlock) {
        slider.interactable = unlock;
        fpsInput.interactable = unlock;
    }

}
