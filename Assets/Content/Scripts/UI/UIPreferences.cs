using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public partial class UISettings : MonoBehaviour
{
    [Header("Preferences")]
    [SerializeField] private GameObject preferenceSettings;
    [SerializeField] private ScrollRect scrollRect;

    [Header("Audio Preferences")]
    [SerializeField] 

    [Header("Option Preferences")]

    private Transform preferencesTrans;
    private Tween prefTween;

    private bool OpenPreferences() {
        if (preferenceSettings.activeSelf) return false;
        if (IsPrefAnimating()) return false;

        prefTween = preferencesTrans.DOLocalMove(Vector3.zero, .15f)
            .From(GetBottomPos())
            .SetEase(Ease.OutBack, 2f)
            .OnStart(() => {
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
