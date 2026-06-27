using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class UISlider : MonoBehaviour
{
    [SerializeField] protected Slider slider;
    [SerializeField] protected TMP_Text text;

    void Update() {
        UpdateSliderText();
    }

    protected virtual void UpdateSliderText() {
        text.text = GetText(slider.value);
    }


    protected abstract void UpdateSave(float value);

    protected abstract string GetText(float value);
}
