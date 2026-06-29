using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class ASettingsInterfaces : MonoBehaviour
{

    [Header("Settings Config")]
    [SerializeField] private RectTransform viewPort;
    [SerializeField] private Transform contentBox;

    [Header("Settings Layout")]
    [SerializeField] private PreferencesType database;
    [SerializeField] private PreferencesPrefabs registry;

    public bool WaitingForKey {  get; private set; }
    private OptionIDs waitingOption;

    private readonly Dictionary<string, GameObject> builtCategories = new();
    private readonly Dictionary<OptionIDs, UIOptions> builtOptions = new();
    private readonly Dictionary<OptionIDs, string> keybindName = new();
    private readonly Dictionary<OptionIDs, Key> keybinds = new();

    protected Dictionary<OptionIDs, UIOptions> optionList;

    protected Dictionary<OptionIDs, UIOptions> Build() {
        Dictionary<OptionIDs, UIOptions> result = new();

        HashSet<string> validCategories = new();
        HashSet<OptionIDs> validOptions = new();

        foreach (SettingsCategory category in database.categories) {
            validCategories.Add(category.categoryName);

            if (!builtCategories.TryGetValue(category.categoryName, out GameObject categoryObj)) {
                categoryObj = Instantiate(category.categoryPrefab, contentBox, false);
                categoryObj.name = category.categoryName;
                categoryObj.GetComponent<UICategory>().Init(category.categoryName);
                builtCategories.Add(category.categoryName, categoryObj);
            }

            Transform optionsParent = categoryObj.transform.Find("OptionsParent");

            foreach (SettingsOption option in category.options) {
                validOptions.Add(option.optionID);

                if (!builtOptions.TryGetValue(option.optionID, out UIOptions optionUI)) {
                    GameObject prefab = registry.GetPrefab(option.type);
                    GameObject optionObj = Instantiate(prefab, optionsParent, false);

                    optionObj.name = option.optionName;
                    optionUI = optionObj.GetComponent<UIOptions>();
                    builtOptions.Add(option.optionID, optionUI);
                }

                if (optionUI is UIInputKey) keybindName[option.optionID] = option.optionName;
                if (optionUI is UIDropdown dropdown) dropdown.AssignViewport(viewPort);
                optionUI.Init(option);
                result[option.optionID] = optionUI;
            }
        }

        foreach (var pair in builtOptions.ToList()) {
            if (!validOptions.Contains(pair.Key)) {
                Destroy(pair.Value.gameObject);
                builtOptions.Remove(pair.Key);
            }
        }

        foreach (var pair in builtCategories.ToList()) {
            if (!validCategories.Contains(pair.Key)) {
                Destroy(pair.Value);
                builtCategories.Remove(pair.Key);
            }
        }

        return result;
    }

    protected void SliderTypeA(OptionIDs id, float value, UnityAction<float> onChanged) {
        if (!optionList.TryGetValue(id, out UIOptions option)) return;

        if (option is not UISliderTypeA sliderUI) return;

        sliderUI.Slider.onValueChanged.RemoveAllListeners();

        sliderUI.Slider.SetValueWithoutNotify(value);
        sliderUI.UpdateDisplay(value);

        sliderUI.Slider.onValueChanged.AddListener(value => {
            sliderUI.UpdateDisplay(value);
            onChanged.Invoke(value);
        });
    }

    protected void SliderTypeB(OptionIDs id, float value, float min, float max,
        bool wholeNumbers, Func<float, string> textFormat, Action<float> apply) {

        if (!optionList.TryGetValue(id, out UIOptions option)) return;

        if (option is not UISliderTypeB sliderUI) return;

        sliderUI.Slider.onValueChanged.RemoveAllListeners();
        sliderUI.Input.onEndEdit.RemoveAllListeners();

        sliderUI.Slider.minValue = min;
        sliderUI.Slider.maxValue = max;
        sliderUI.Slider.wholeNumbers = wholeNumbers;

        SetSliderBValue(sliderUI, value, textFormat);
        apply(value);

        sliderUI.Slider.onValueChanged.AddListener(value => {
            SetSliderBValue(sliderUI, value, textFormat);
            apply(value);
        });

        sliderUI.Input.onEndEdit.AddListener(text => {
            if (!float.TryParse(text, out float val)) val = value;

            val = Mathf.Clamp(val, min, max);

            if (wholeNumbers) val = Mathf.Round(val);

            SetSliderBValue(sliderUI, val, textFormat);
            apply(val);

        });
    }

    protected void SetSliderBValue(UISliderTypeB sliderUI, float value, Func<float, string> textFormat) {
        sliderUI.Slider.SetValueWithoutNotify(value);
        sliderUI.Input.SetTextWithoutNotify(textFormat(value));
    }

    protected void Dropdown(OptionIDs id, int value, Action<int> apply) {
        if (!optionList.TryGetValue(id, out UIOptions option)) return;
        if (option is not UIDropdown dropdown) return;

        dropdown.Dropdown.onValueChanged.RemoveAllListeners();
        dropdown.Dropdown.SetValueWithoutNotify(value);

        dropdown.Dropdown.onValueChanged.AddListener(text => {
            apply(text);
        });
    }

    protected void Keybind(OptionIDs id, Key currentKey, Action<Key> apply) {
        if (!optionList.TryGetValue(id, out UIOptions option)) return;
        if (option is not UIInputKey keyUI) return;

        keyUI.KeyInput.onClick.RemoveAllListeners();
        keyUI.ChangeKeyVisible(currentKey.ToString());
        keyUI.DisplayWarning("");

        keyUI.KeyInput.onClick.AddListener(() =>
        {
            WaitingForKey = true;
            waitingOption = id;
            Key prev = keyUI.TextToKey();

            keyUI.ChangeKeyVisible("...");
            keyUI.DisplayWarning("Press any key or Escape for cancel");
            StartCoroutine(WaitForKey(keyUI, apply, prev));
        });
    }

    private IEnumerator WaitForKey(UIInputKey keyUI, Action<Key> apply, Key originalKey) {
        yield return null;

        while (WaitingForKey) {
            if (Keyboard.current != null) {
                foreach (KeyControl keyControl in Keyboard.current.allKeys) {
                    if (keyControl.wasPressedThisFrame) {
                        Key newKey = keyControl.keyCode;
                        keyUI.DisplayWarning("");

                        if (newKey == Key.Escape) {
                            WaitingForKey = false;

                            keyUI.ChangeKeyVisible(originalKey.ToString());
                            keyUI.DisplayWarning("");

                            yield break;
                        }

                        List<string> interrupted = IsKeyUsed(newKey, waitingOption);
                        if (interrupted.Count > 0) {
                            keyUI.DisplayWarning($"Key already assigned to " + string.Join(", ", interrupted));
                            //continue;
                        }

                        apply(newKey);
                        keyUI.ChangeKeyVisible(newKey.ToString());
                        RefreshKeybinds();
                        WaitingForKey = false;
                        yield break;
                    }
                }
            }

            yield return null;
        }
    }

    private List<string> IsKeyUsed(Key key, OptionIDs currentOption) {
        string missing = "Key Missing";

        return keybinds
            .Where(pair => pair.Key != currentOption && pair.Value == key)
            .Select(pair => keybindName.GetValueOrDefault(pair.Key, missing))
            .ToList();
    }

    private void RefreshKeybinds() {
        keybinds[OptionIDs.MoveFW] = SaveManager.Instance.moveFW;
        keybinds[OptionIDs.MoveBW] = SaveManager.Instance.moveBW;
        keybinds[OptionIDs.MoveL] = SaveManager.Instance.moveL;
        keybinds[OptionIDs.MoveR] = SaveManager.Instance.moveR;

        keybinds[OptionIDs.Interact] = SaveManager.Instance.interact;
        keybinds[OptionIDs.Sprint] = SaveManager.Instance.sprint;
    }
}