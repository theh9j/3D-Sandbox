using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public abstract class AInteractable : MonoBehaviour
{
    [SerializeField] private Outline outline;
    [SerializeField] private string baseHexColor = "#FFBF76";
    [SerializeField] private float outlineWidth = 10f;
    private Coroutine outlineFluctuation;
    public bool OnHover { get; private set; }

    protected virtual void Awake() {
        if (outline == null) outline = transform.GetComponentInChildren<Outline>();
        if (outline != null) {
            if (ColorUtility.TryParseHtmlString(baseHexColor, out Color baseColor))
                outline.OutlineColor = baseColor;
            outline.enabled = false;
            outline.OutlineMode = Outline.Mode.OutlineVisible;
            outline.OutlineWidth = outlineWidth;
        }
    }

    public virtual void HoverEnter() {
        if (outline == null)
            return;

        OnHover = true;
        outline.enabled = true;

        if (outlineFluctuation != null)
            StopCoroutine(outlineFluctuation);

        outlineFluctuation = StartCoroutine(HoverFluct());
    }

    private IEnumerator HoverFluct() {
        while (OnHover && outline != null) {
            float alpha = 0.15f + Mathf.PingPong(Time.time * .95f, 0.5f);

            Color c = outline.OutlineColor;
            c.a = alpha;
            outline.OutlineColor = c;

            yield return null;
        }
    }

    public virtual void HoverExit() {
        OnHover = false;

        if (outlineFluctuation != null) {
            StopCoroutine(outlineFluctuation);
            outlineFluctuation = null;
        }

        if (outline != null)
            outline.enabled = false;
    }

    public abstract void Interact(PlayerController player);
}
