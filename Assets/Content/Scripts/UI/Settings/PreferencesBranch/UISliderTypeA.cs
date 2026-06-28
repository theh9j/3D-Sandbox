using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISliderTypeA : UIOptions
{
    [SerializeField] private TMP_Text valueText;
    [SerializeField] private Slider slider;

    public Slider Slider => slider;

    public override void Init(SettingsOption data) {
        base.Init(data);
        RefreshText();
        slider.onValueChanged.AddListener(_ => RefreshText());
    }

    private void RefreshText() {
        valueText.text = $"{slider.value * 100f:F0}%";
    }
}