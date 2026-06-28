using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public partial class UISettings : MonoBehaviour
{
    [Header("Preferences")]
    [SerializeField] private GameObject preferenceSettings;
    [SerializeField] private ScrollRect scrollRect;

    [Header("Preferences Layout")]
    [SerializeField] private PreferencesType database;
    [SerializeField] private PreferencesPrefabs registry;
    [SerializeField] private Transform contentBox;


    [Header("Option Preferences")]

    private readonly Dictionary<string, GameObject> builtCategories = new();
    private readonly Dictionary<OptionIDs, UIOptions> builtOptions = new();
    private List<UIOptions> optionList;
    private Transform preferencesTrans;
    private Tween prefTween;

    private bool OpenPreferences() {
        if (preferenceSettings.activeSelf) return false;
        if (IsPrefAnimating()) return false;

        prefTween = preferencesTrans.DOLocalMove(Vector3.zero, .15f)
            .From(GetBottomPos())
            .SetEase(Ease.OutBack, 2f)
            .OnStart(() => {
                optionList = Build();
                SetActivityPref(true);
            })
            .OnKill(() => { prefTween = null; });

        return true;
    }

    private bool ClosePreferences() {
        if (!preferenceSettings.activeSelf) return false;
        if (IsPrefAnimating()) return false;

        prefTween = preferencesTrans.DOLocalMove(GetBottomPos(), .15f)
            .OnComplete(() => {
                SetActivityPref(false);
            })
            .OnKill(() => { prefTween = null; });

        return true;
    }

    private void SetActivityPref(bool active) {
        preferenceSettings.SetActive(active);
        Pref = active;
        if (active) ResetScroll(scrollRect);
    }

    public List<UIOptions> Build() {
        optionList = new();
        List<UIOptions> result = new();

        HashSet<string> validCategories = new();
        HashSet<OptionIDs> validOptions = new();

        foreach (SettingsCategory category in database.categories) {
            validCategories.Add(category.categoryName);

            if (!builtCategories.TryGetValue(category.categoryName, out GameObject categoryObj)) {
                categoryObj = Instantiate(category.categoryPrefab, contentBox);
                categoryObj.name = category.categoryName;
                categoryObj.GetComponent<UICategory>().Init(category.categoryName);
                builtCategories.Add(category.categoryName, categoryObj);
            }

            Transform optionsParent = categoryObj.transform;

            foreach (SettingsOption option in category.options) {
                validOptions.Add(option.optionID);

                if (!builtOptions.TryGetValue(option.optionID, out UIOptions optionUI)) {
                    GameObject prefab = registry.GetPrefab(option.type);
                    GameObject optionObj = Instantiate(prefab, optionsParent);
                    optionObj.name = option.optionName;
                    optionUI = optionObj.GetComponent<UIOptions>();
                    builtOptions.Add(option.optionID, optionUI);
                }

                optionUI.Init(option);
                result.Add(optionUI);
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



    //HELPERS

    private Vector2 GetBottomPos() {
        float canvasHeight = canvas.rect.height;
        float panelHeight = (preferencesTrans as RectTransform).rect.height;

        return new Vector2(
            0f, -(canvasHeight / 2f + panelHeight / 2f + startOffset));
    }

    private bool IsPrefAnimating() {
        return prefTween != null && prefTween.IsActive() && prefTween.IsPlaying();
    }
}
