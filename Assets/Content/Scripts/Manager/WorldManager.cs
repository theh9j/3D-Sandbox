using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public static WorldManager Instance;

    

    public float DepthLimit { get; private set; } = -20f;


    void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
}
