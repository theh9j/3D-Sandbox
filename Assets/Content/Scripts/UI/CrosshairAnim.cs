using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CrosshairAnim : MonoBehaviour
{
    [SerializeField] PlayerController controller;
    [SerializeField] private Sprite[] crosshairs;
    [SerializeField] private Image crosshair;
    [SerializeField] private float delay = .2f;
    
    void Update() {
        if (controller.CurrentMode == Modes.Normal) crosshair.gameObject.SetActive(true);
        else crosshair.gameObject.SetActive(false);
    }

    void Awake() {
        StartCoroutine(CrosshairAnimation());
    }

    private IEnumerator CrosshairAnimation() {
        while (true) {
            foreach (Sprite sprite in crosshairs) {
                crosshair.sprite = sprite;
                yield return new WaitForSeconds(delay);
            }
        }
        
    }
}
