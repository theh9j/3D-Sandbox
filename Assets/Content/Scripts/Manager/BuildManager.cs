using UnityEngine;

public class BuildManager : MonoBehaviour
{

    public static BuildManager Instance;

    [Header("Object Settings")]
    [SerializeField] private LayerMask buildMask;
    [SerializeField] private LayerMask entityMask;
    [SerializeField] private Vector3 minSize = new(.2f, .2f, .2f);
    [SerializeField] private Vector3 maxSize = new(5f, 5f, 5f);

    [Header("Camera Settings")]


    private Vector3 modelPos;
    private Vector3 baseSize;

    void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Update() {


    }

    private void CanMove() {

    }
}
