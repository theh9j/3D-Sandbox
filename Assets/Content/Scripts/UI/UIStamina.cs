using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIStamina : MonoBehaviour
{
    [SerializeField] private PlayerHealth health;
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_Text percentile;

    void Start() {
        slider.maxValue = 1f;

        health.OnStaminaChanged += UpdateBar;
        UpdateBar(health.Stamina, health.MaxStamina);

    }

    private void UpdateBar(float value, float max) {
        float percent = value / max;
        UpdatePercentile(percent*100);
        slider.value = percent;
    }

    private void UpdatePercentile(float percentile) {
        this.percentile.text = percentile.ToString("F0") + "%";
    }
}
