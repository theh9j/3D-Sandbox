using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIDropdown : UIOptions
{
    [SerializeField] private TMP_Dropdown dropdown;
    [SerializeField] private RectTransform item;
    [SerializeField] private float maxHeight = 200f;
    public RectTransform template;

    public RectTransform ViewPort { get; private set; }

    public TMP_Dropdown Dropdown => dropdown;

    public override void Init(SettingsOption option) {
        base.Init(option);

        dropdown.ClearOptions();
        dropdown.AddOptions(option.dropdownOptions);
        ResizeTemplate(option.dropdownOptions.Count);
    }

    private void ResizeTemplate(int optionCount) {
        float itemHeight = item.rect.height;

        float desiredHeight = itemHeight * optionCount + 10f;

        template.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Vertical,
            Mathf.Min(desiredHeight, maxHeight)
        );
    }

    public void AssignViewport(RectTransform viewport) {
        this.ViewPort = viewport;
    }
}