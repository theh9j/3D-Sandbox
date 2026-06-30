using UnityEngine;

[CreateAssetMenu(fileName = "WeatherTypes", menuName = "Scriptable Objects/WeatherTypes")]
public class WeatherTypes : ScriptableObject
{
    [Header("General")]
    public Weather weatherType;
    public int weatherWeight;

    [Header("Occurence Window")]
    [Range(0f, 1f)]
    public float minWindow;
    [Range(0f, 1f)]
    public float maxWindow;

    public FogInfo fog;
    public LightingInfo lighting;
    public EnvironmentalEFX envSFX;

}

[System.Serializable]
public class FogInfo {
    public float fogDensity;
    public Color fogColor;
}

[System.Serializable]
public class LightingInfo {
    public float sunSize = .035f;
    public float sunConvergenceSize = 1f;
    public float exposure = 1f;
}

[System.Serializable]
public class EnvironmentalEFX {
    [Header("Visibility")]
    public float visbility;

    [Header("Wind")]
    public float windStrength;

    [Header("Particles")]
    public float dustEmissionRate;
    public float maxDust;

    [Header("Audio")]
    public AudioClip ambientSFX = null;
    public float ambientVolume;
}