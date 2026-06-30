using System.Collections;
using UnityEngine;

public class WeatherEFX : MonoBehaviour
{
    [SerializeField] private Transform follow;

    private void LateUpdate() {
        if (follow == null) return;

        transform.position = follow.position;
    }
}
