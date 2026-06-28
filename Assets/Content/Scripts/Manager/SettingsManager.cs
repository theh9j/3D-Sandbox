using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance; 

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

    public void SetAntiAlias(string type) {
        UniversalAdditionalCameraData camData = Camera.main.GetComponent<UniversalAdditionalCameraData>();
        switch (type) {
            case "Disabled":
                camData.antialiasing = AntialiasingMode.None;
                break;
            case "TAA":
                camData.antialiasing = AntialiasingMode.TemporalAntiAliasing;
                break;
            case "SMAA":
                camData.antialiasing = AntialiasingMode.SubpixelMorphologicalAntiAliasing;
                break;
            default:
                return;
        }
        SaveManager.Instance.antiAlias = type;
    }
}
