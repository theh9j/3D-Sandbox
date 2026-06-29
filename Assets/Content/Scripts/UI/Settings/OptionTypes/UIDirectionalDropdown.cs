using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIDirectionalDropdown : MonoBehaviour, IPointerDownHandler {

    [SerializeField] private UIDropdown dropdown;

    public void OnPointerDown(PointerEventData eventData) {
        if (dropdown.ViewPort == null) return;
        SetDirection();
    }

    private void SetDirection() {
        Vector3[] viewportCorners = new Vector3[4];
        Vector3[] dropdownCorners = new Vector3[4];

        dropdown.ViewPort.GetWorldCorners(viewportCorners);
        ((RectTransform)transform).GetWorldCorners(dropdownCorners);

        float viewportBottom = viewportCorners[0].y;
        float viewportTop = viewportCorners[1].y;

        float dropdownBottom = dropdownCorners[0].y;
        float dropdownTop = dropdownCorners[1].y;

        float templateHeight = dropdown.template.rect.height;

        float spaceBelow = dropdownBottom - viewportBottom;
        float spaceAbove = viewportTop - dropdownTop;

        bool openUp = spaceBelow < templateHeight && spaceAbove > spaceBelow;

        if (openUp) {
            dropdown.template.anchorMin = new Vector2(0f, 1f);
            dropdown.template.anchorMax = new Vector2(1f, 1f);
            dropdown.template.pivot = new Vector2(0.5f, 0f);
            dropdown.template.anchoredPosition = Vector2.zero;
        } else {
            dropdown.template.anchorMin = new Vector2(0f, 0f);
            dropdown.template.anchorMax = new Vector2(1f, 0f);
            dropdown.template.pivot = new Vector2(0.5f, 1f);
            dropdown.template.anchoredPosition = Vector2.zero;
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(dropdown.template);
    }
}
