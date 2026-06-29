using System;
using System.Collections;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;

    public int minFps = 30;
    public int maxFps = 241;

    public int minFov = 60;
    public int maxFov = 120;

    public float minSens = 0.01f;
    public float maxSens = 5f;

    public event Action<int> FovChanged;
    public event Action<float> SensChanged;

    void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start() {
        StartCoroutine(OnLaunch());
    }

    private IEnumerator OnLaunch() {
        yield return new WaitUntil(() =>
            SaveManager.Instance != null && SaveManager.Instance.init
        );
        SetVSync(SaveManager.Instance.vsync);
        SetAntiAlias(SaveManager.Instance.antiAlias);
    }

    public void SetFPS(int fps) {
        if (QualitySettings.vSyncCount > 0) return;
        Application.targetFrameRate = fps;
        SaveManager.Instance.fps = fps;
    }

    public void SetVSync(bool active) {
        if (active) SetFPS(-1);
        else SetFPS(SaveManager.Instance.fps);
        QualitySettings.vSyncCount = active ? 1 : 0;
        SaveManager.Instance.vsync = active;
    }

    public void UpdateFOV(int fov) {
        FovChanged.Invoke(fov);
    }

    public void UpdateSens(float sens) {
        SensChanged.Invoke(sens);
    }

    public void SetAntiAlias(int type) {
        UniversalAdditionalCameraData camData = Camera.main.GetComponent<UniversalAdditionalCameraData>();
        switch (type) {
            case 0:
                camData.antialiasing = AntialiasingMode.None;
                break;
            case 1:
                camData.antialiasing = AntialiasingMode.TemporalAntiAliasing;
                break;
            case 2:
                camData.antialiasing = AntialiasingMode.SubpixelMorphologicalAntiAliasing;
                break;
            default:
                return;
        }
        SaveManager.Instance.antiAlias = type;
    }
}
