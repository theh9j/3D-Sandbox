using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIInputKey : UIOptions
{
    [SerializeField] private Button inputKey;
    [SerializeField] private TMP_Text text;
    [SerializeField] private GameObject warning;

    public Button KeyInput => inputKey;

    public override void Init(SettingsOption option) {
        base.Init(option);
        DisplayWarning("");
    }

    public void ChangeKeyVisible(string key) {

        text.text = key;
    } 

    public Key TextToKey() {
        if (!Key.TryParse(text.text, out Key current)) current = Key.None;
        return current;
    }

    public void DisplayWarning(string message) {
        warning.GetComponent<TMP_Text>().text = message;
    }
}
