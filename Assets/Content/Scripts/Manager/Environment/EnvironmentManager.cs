using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnvironmentManager : MonoBehaviour
{
    public static EnvironmentManager Instance { get; private set; }

    [SerializeField] private LightManager lightManager;

    [Header("Weather")]
    [SerializeField] private WeatherTypes[] weatherTypes;
    [SerializeField] private Material skyBox;
    [SerializeField] private ParticleSystem effects;

    [Header("Configuration")]
    [SerializeField] private float delay = 2.5f;
    [SerializeField] private int resetChance = 5; //prevent weather reset when a new weather occurs

    [Header("Randomness")]
    [SerializeField] private float minSeverity = .7f;
    [SerializeField] private float maxSeverity = 1.5f;
    [SerializeField] private bool randSeverity = true;

    private WeatherTypes currentWeather;
    private bool weatherGauge;

    private Coroutine weatherTransition;
    private ParticleSystem.EmissionModule particleEmission;
    private ParticleSystem.MainModule particleMain;

    public event Action<float> visibilityFluct;

    void Awake() {
        Instance = this;
        particleEmission = effects.emission;
        particleMain = effects.main;


        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.ExponentialSquared;
    }

    void Start() {
        ChangeWeather(Weather.Clear);
    }

    public void ChangeWeather(Weather weather = Weather.Randomize) {
        if (weather == Weather.Randomize) {
            if (weatherGauge && Random.Range(0, resetChance) != 1 && 
                (currentWeather.minWindow > lightManager.TimeOfDay || currentWeather.maxWindow < lightManager.TimeOfDay))
                return;

            weather = RandomWeather();
            if (weather == currentWeather.weatherType) return;
            weatherGauge = IsSpecialWeather(weather);
        }

        if (!effects.isPlaying)
            effects.Play();

        foreach (var weatherType in weatherTypes) {
            if (weatherType.weatherType == weather) {
                WeatherEffectChanges(weatherType);
                break;
            }
        }
    }

    private bool IsSpecialWeather(Weather weather) {
        return weather != Weather.Clear && weather != Weather.Night;
    }

    private Weather RandomWeather() {
        float time = lightManager.TimeOfDay;

        int totalChances = 0;

        foreach (var weatherType in weatherTypes) {
            if (time < weatherType.minWindow || time > weatherType.maxWindow)
                continue;

            totalChances += Mathf.Max(0, weatherType.weatherWeight);
        }

        if (totalChances == 0)
            return Weather.Clear; //fallback

        int roll = Random.Range(0, totalChances);

        foreach (var weatherType in weatherTypes) {
            if (time < weatherType.minWindow || time > weatherType.maxWindow)
                continue;

            int weight = Mathf.Max(0, weatherType.weatherWeight);

            if (roll < weight)
                return weatherType.weatherType;

            roll -= weight;
        }

        return Weather.Clear; //fallback
    }

    private void WeatherEffectChanges(WeatherTypes weather) {
        if (weatherTransition != null)
            StopCoroutine(weatherTransition);

        float severity = randSeverity ? Random.Range(minSeverity, maxSeverity) : 1f;

        weatherTransition = StartCoroutine(TransitionWeather(weather, severity));
        ChangeWindDirection(weather.envSFX.windStrength);
        Debug.Log("Weather changed to " + weather.weatherType.ToString());
    }

    private void ChangeWindDirection(float windStrength) {
        var velocity = effects.velocityOverLifetime;
        velocity.enabled = true;

        Vector3 windDirection = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;
        if (windDirection == Vector3.zero) windDirection = Vector3.forward;

        velocity.space = ParticleSystemSimulationSpace.World;
        velocity.x = windDirection.x * windStrength;
        velocity.y = windDirection.y * windStrength;
        velocity.z = windDirection.z * windStrength;
    }

    private IEnumerator TransitionWeather(WeatherTypes weather, float severity) {
        Color startFogColor = RenderSettings.fogColor;
        Color targetFogColor = weather.fog.fogColor;

        float startFogDensity = RenderSettings.fogDensity;
        float targetFogDensity = weather.fog.fogDensity * severity;

        float startSunSize = skyBox.GetFloat("_SunSize");
        float targetSunSize = weather.lighting.sunSize;

        float startSunConvergenceSize = skyBox.GetFloat("_SunSizeConvergence");
        float targetSunConvergenceSize = weather.lighting.sunConvergenceSize;

        float startExposure = skyBox.GetFloat("_Exposure");
        float endExposure = weather.lighting.exposure;

        float startParticleRate = particleEmission.rateOverTime.constant;
        float targetParticleRate = weather.envSFX.dustEmissionRate * severity;

        float startMaxParticle = particleMain.maxParticles;
        float targetMaxParticle = weather.envSFX.maxDust * severity;

        float startWindSpeed = particleMain.simulationSpeed;
        float targetWindSpeed = weather.envSFX.windStrength * severity;

        float currentVisibility = currentWeather == null ? 1_000f : currentWeather.envSFX.visbility;
        float targetVisibility = weather.envSFX.visbility * severity;

        AudioManager.Instance?.PlayEnvironment(weather.envSFX.ambientSFX, weather.envSFX.ambientVolume);

        float timer = 0f;

        while (timer < delay) {
            timer += Time.deltaTime;

            float t = timer / delay;
            t = Mathf.SmoothStep(0f, 1f, t);

            RenderSettings.fogColor = Color.Lerp(startFogColor, targetFogColor, t);
            RenderSettings.fogDensity = Mathf.Lerp(startFogDensity, targetFogDensity, t);

            skyBox.SetFloat("_SunSize", Mathf.Lerp(startSunSize, targetSunSize, t));
            skyBox.SetFloat("_SunSizeConvergence", Mathf.Lerp(startSunConvergenceSize, targetSunConvergenceSize, t));
            skyBox.SetFloat("_Exposure", Mathf.Lerp(startExposure, endExposure, t));

            visibilityFluct.Invoke(Mathf.Lerp(currentVisibility, targetVisibility, t));
            particleEmission.rateOverTime = Mathf.Lerp(startParticleRate, targetParticleRate, t);
            particleMain.maxParticles = Mathf.RoundToInt(Mathf.Lerp(startMaxParticle, targetMaxParticle, t));
            particleMain.simulationSpeed = Mathf.Lerp(startWindSpeed, targetWindSpeed, t);

            yield return null;
        }

        RenderSettings.fogColor = targetFogColor;
        RenderSettings.fogDensity = targetFogDensity;

        skyBox.SetFloat("_SunSize", targetSunSize);
        skyBox.SetFloat("_SunSizeConvergence", targetSunConvergenceSize);
        skyBox.SetFloat("_Exposure", endExposure);

        visibilityFluct.Invoke(targetVisibility);
        particleEmission.rateOverTime = targetParticleRate;
        particleMain.simulationSpeed = targetWindSpeed;

        currentWeather = weather;
        weatherTransition = null;
    }

}