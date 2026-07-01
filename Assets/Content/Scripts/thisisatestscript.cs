using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.InputSystem;

public class thisisatestscript : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private GameObject player;

    void Update() {
        Interaction();
    }

    private void Interaction() {
        if (Keyboard.current != null
            && Keyboard.current[Key.B].isPressed
            && Keyboard.current[Key.T].isPressed) {

            Instantiate(prefab, new Vector3(0f, 10f, 0f), Quaternion.identity);
        }

        if (Keyboard.current != null && Keyboard.current[Key.Backslash].wasPressedThisFrame)
            player.transform.position = Vector3.up * 100f;
    }

}
