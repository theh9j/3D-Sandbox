using UnityEngine;

public class LightManager : MonoBehaviour
{
    [SerializeField] private Light sun;

    [Header("Time")]
    [SerializeField] private float dayLengthInSeconds = 900f;

    [Range(0f, 1f)]
    [SerializeField] private float timeOfDay = .25f;

    [Header("Lighting")]
    [SerializeField] private Gradient ambientColor;
    [SerializeField] private Gradient sunColor;
    [SerializeField] private AnimationCurve sunIntensity;
    [SerializeField] private Material skyBox;

    void Update() {
        timeOfDay += Time.deltaTime / dayLengthInSeconds;

        if (timeOfDay >= 1f)
            timeOfDay = -1f;

        UpdateLighting();
    }

    private void UpdateLighting() {
        float sunAngle = timeOfDay * 360f - 90f;

        sun.transform.rotation = Quaternion.Euler(sunAngle, 170f, 0f);

        sun.color = sunColor.Evaluate(timeOfDay);
        sun.intensity = sunIntensity.Evaluate(timeOfDay);

        RenderSettings.ambientLight = ambientColor.Evaluate(timeOfDay);
    }
}
