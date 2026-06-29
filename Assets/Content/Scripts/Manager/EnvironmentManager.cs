using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    public static EnvironmentManager Instance { get; private set; }

    [Header("Weather")]
    [SerializeField] private Weather currentWeather = Weather.Clear;

    [Header("Clear Fog")]
    [SerializeField] private Color clearFogColor = new(.85f, .75f, .55f);
    [SerializeField] private float clearFogDensity = 1f;

    [Header("Sandstorm Fog")]
    [SerializeField] private Color stormFogColor = new(.75f, .58f, .35f);
    [SerializeField] private float stormFogDensity = .035f;

    void Awake() {
        Instance = this;
        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.Exponential;
    }

    void Start() {
        ApplyWeather(currentWeather);
    }




    private void ApplyWeather(Weather weather) {
        switch (weather) {
            case Weather.Clear:
                RenderSettings.fogColor = clearFogColor;
                RenderSettings.fogDensity = clearFogDensity;
                break;
            case Weather.Sandstorm:
                RenderSettings.fogColor = stormFogColor;
                RenderSettings.fogDensity = stormFogDensity;
                break;


            default:
                break;
        }
    }
}
