using System;
using TMPro;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.UI;

public class UIOptions : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown dropdown;
    [SerializeField] private OptionsType opt;
    public event Action<bool> OnVSyncLock;

    void Awake() {
        dropdown.onValueChanged.AddListener(OnValueChanged);
    }

    private void OnValueChanged(int type) {
        switch (opt) {
            case OptionsType.VSync:
                VSync(type);
                break;
        }
    }

    private void VSync(int type) { // 1 = Enabled
        bool select = type == 1;
        if (select) QualitySettings.vSyncCount = 1;
        else QualitySettings.vSyncCount = 0;
        OnVSyncLock?.Invoke(!select);

    }

    private void AntiAlias(int type) {
        AntiAliasing aa = (AntiAliasing)type;
        switch (aa) {
            case AntiAliasing.Disabled:

                break;
            case AntiAliasing.MSAA:

                break;

            case AntiAliasing.FXAA:

                break;
        }
    }
}
