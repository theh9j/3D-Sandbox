using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Options", menuName = "Scriptable Objects/Options")]
public class PreferencesType : ScriptableObject
{
    public List<SettingsCategory> categories = new();
}

[Serializable]
public class SettingsCategory {
    public string categoryName;
    public GameObject categoryPrefab;
    public List<SettingsOption> options = new();
}

[Serializable]
public class SettingsOption {
    public OptionIDs optionID;
    public string optionName;
    public OptionsType type;

    public List<string> dropdownOptions;
}