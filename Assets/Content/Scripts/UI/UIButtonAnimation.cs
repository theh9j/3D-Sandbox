using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonAnimation :
    MonoBehaviour,
    IPointerDownHandler,
    IPointerUpHandler,
    IPointerExitHandler {
    [SerializeField] private RectTransform face;
    [SerializeField] private TMP_Text front;
    [SerializeField] private TMP_Text back;

    [SerializeField] private Vector2 releasedPos;
    [SerializeField] private Vector2 pressedPos;

    [SerializeField] private float duration = 0.08f;

    private Tween currentTween;

    private void Awake() {
        if (face == null)
            face = transform as RectTransform;

        releasedPos = face.anchoredPosition;
    }

    public void OnPointerDown(PointerEventData eventData) {
        AnimateTo(pressedPos);
    }

    public void OnPointerUp(PointerEventData eventData) {
        AnimateTo(releasedPos);
    }

    public void OnPointerExit(PointerEventData eventData) {
        AnimateTo(releasedPos);
    }

    public void SetText(string text) {
        front.text = text;
        back.text = text;
    }

    private void AnimateTo(Vector2 target) {
        currentTween?.Kill();

        currentTween = face
            .DOAnchorPos(target, duration)
            .SetEase(Ease.OutQuad);
    }
}