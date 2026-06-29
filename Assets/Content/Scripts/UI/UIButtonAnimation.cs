using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonAnimation :
    MonoBehaviour,
    IPointerDownHandler,
    IPointerUpHandler,
    IPointerExitHandler {

    [SerializeField] private RectTransform face;

    [SerializeField] private Vector2 releasedPos;
    [SerializeField] private Vector2 hoverPos;
    [SerializeField] private Vector2 pressedPos;

    [SerializeField] private float duration = 0.08f;

    private Tween currentTween;
    private bool hovering;

    private void Awake() {
        if (face == null)
            face = transform as RectTransform;

        releasedPos = face.anchoredPosition;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        hovering = true;
        AnimateTo(hoverPos);
    }

    public void OnPointerExit(PointerEventData eventData) {
        hovering = false;
        AnimateTo(releasedPos);
    }

    public void OnPointerDown(PointerEventData eventData) {
        AnimateTo(pressedPos);
    }

    public void OnPointerUp(PointerEventData eventData) {
        AnimateTo(hovering ? hoverPos : releasedPos);
    }

    private void AnimateTo(Vector2 target) {
        currentTween?.Kill();

        currentTween = face
            .DOAnchorPos(target, duration)
            .SetEase(Ease.OutQuad);
    }
}