using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class UIOptions : MonoBehaviour
{
    public OptionIDs OptionID { get; private set; }

    [SerializeField] protected TMP_Text optionNameF;
    [SerializeField] protected TMP_Text optionNameB;

    public virtual void Init(SettingsOption option) {
        optionNameF.text = option.optionName;
        optionNameB.text = option.optionName;
        OptionID = option.optionID;
    }
}
