using UnityEngine;
using UnityEngine.InputSystem;

public class LightManager : MonoBehaviour
{
    [SerializeField] private Light sun;

    [Header("Time")]
    [SerializeField] private float dayLengthInSeconds = 900f;
    [SerializeField] private float envUpdateRate = .25f;


    [Header("Lighting")]
    [SerializeField] private Gradient ambientColor;
    [SerializeField] private Gradient sunColor;
    [SerializeField] private AnimationCurve sunIntensity;

    /* Midnight - 0f
     * Sunrise - 0.25f
     * Noon - 0.5f
     * Sunset 0.75f
     * Midnight 1f
    */

    public float TimeOfDay { get; private set; } = 0.25f;
    private float envTimer;
    private bool timeFlow = true;
    

    void Update() {
        DebugResetTimeTest();
        if (!timeFlow) return;

        TimeOfDay += Time.deltaTime / dayLengthInSeconds;

        if (TimeOfDay >= 1f) timeFlow = false;

        UpdateLighting();

        envTimer += Time.deltaTime;

        if (envTimer < envUpdateRate) return;

        envTimer = 0f;
        EnvironmentManager.Instance?.ChangeWeather();
        Debug.Log("Attempt Weather Change");
    }

    private void UpdateLighting() {
        float sunAngle = TimeOfDay * 360f - 90f;

        sun.transform.rotation = Quaternion.Euler(sunAngle, 170f, 0f);

        sun.color = sunColor.Evaluate(TimeOfDay);
        sun.intensity = sunIntensity.Evaluate(TimeOfDay);

        RenderSettings.ambientLight = ambientColor.Evaluate(TimeOfDay);
    }

    private void DebugResetTimeTest() {
        if (Keyboard.current != null && Keyboard.current.tabKey.wasPressedThisFrame) {
            TimeOfDay = .25f;
            timeFlow = true;
        }
    }
}
