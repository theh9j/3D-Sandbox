using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISliderTypeB : UIOptions
{
    [SerializeField] private TMP_InputField input;
    [SerializeField] private Slider slider;

    public Slider Slider => slider;
    public TMP_InputField Input => input;

    public override void Init(SettingsOption data) {
        base.Init(data);

        slider.onValueChanged.AddListener(v => {
            input.SetTextWithoutNotify(v.ToString("F0"));
        });
    }
}
