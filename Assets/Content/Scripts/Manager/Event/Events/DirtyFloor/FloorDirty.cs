using UnityEngine;

public class FloorDirty : MonoBehaviour
{
    private int playerOnTop;
    private float time;

    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Player")) return;

        playerOnTop++;
        time = 0f;
    }

    private void OnTriggerExit(Collider other) {
        if (!other.CompareTag("Player")) return;

        playerOnTop = Mathf.Max(0, playerOnTop - 1);
        time = 0f;
    }

    void Update() {
        if (playerOnTop <= 0) return;
        

        time += Time.deltaTime;
        if (time >= 4f) {
            Destroy(gameObject);
        }
    }
}
