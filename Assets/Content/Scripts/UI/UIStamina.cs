using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIStamina : MonoBehaviour
{
    [SerializeField] private PlayerHealth health;

    [Header("Energy")]
    [SerializeField] private Slider energySlider;
    [SerializeField] private TMP_Text energyPerc;

    [Header("Thirst")]
    [SerializeField] private Slider thirstSlider;
    [SerializeField] private TMP_Text thirstPerc;

    void Start() {
        energySlider.maxValue = 1f;

        health.OnEnergyChanged += OnEnergyChange;
        health.OnThirstChanged += OnThirstChange;
        OnEnergyChange(health.Energy, health.MaxEnergy);
        OnThirstChange(health.Thirst, health.MaxThirst);
    }

    //Energy

    private void OnEnergyChange(float value, float max) {
        float percent = value / max;
        EnergyPercentage(percent*100, CheckForTextColor(percent));
        energySlider.value = percent;
    }

    private void EnergyPercentage(float percentile, Color c) {
        energyPerc.color = c;
        energyPerc.text = percentile.ToString("F0") + "%";
    }


    //Thirst

    private void OnThirstChange(float value, float max) {
        float percent = value / max;
        ThirstPercentage(percent * 100, CheckForTextColor(percent));
        thirstSlider.value = percent;
    }

    private void ThirstPercentage(float percentile, Color c) {
        thirstPerc.color = c;
        thirstPerc.text = percentile.ToString("F0") + "%";
    }

    //Helper

    private Color CheckForTextColor(float value) {
        if (Mathf.Approximately(value, 0f)) return Color.red;
        if (value < .1f) return Color.orange;
        if (value < .4f) return Color.yellow;
        return Color.white;
    }
}
