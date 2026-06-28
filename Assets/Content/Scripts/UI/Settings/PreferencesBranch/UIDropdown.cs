using System;
using TMPro;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.UI;

public class UIDropdown : UIOptions
{
    [SerializeField] private TMP_Dropdown dropdown;

    public TMP_Dropdown Dropdown => dropdown;

    public override void Init(SettingsOption option) {
        base.Init(option);

        dropdown.ClearOptions();
        dropdown.AddOptions(option.dropdownOptions);
    }
}