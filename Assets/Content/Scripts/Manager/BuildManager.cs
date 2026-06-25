using UnityEngine;

public class BuildManager : MonoBehaviour
{

    [Header("Size Settings")]
    [SerializeField] private Vector3 minSize = new(.2f, .2f, .2f);
    [SerializeField] private Vector3 maxSize = new(5f, 5f, 5f);

    [Header("Camera Settings")]
    [SerializeField] private float distanceCamera = 10f;


    private Vector3 modelPos;
    private Vector3 baseSize;

    void Start() {
        baseSize = transform.localScale;
    }

    void Update() {

    }

    private void CheckForValidity() {
        CheckSizes();
        CheckForMovement();
    }

   

    private void CheckSizes() {
        CheckSize();
    }

    private void CheckForMovement() {

    }

    //SIZE CHECKER

    private void CheckSize() {
        Mathf.Clamp(baseSize.x, minSize.x, maxSize.x);
        Mathf.Clamp(baseSize.y, minSize.y, maxSize.y);
        Mathf.Clamp(baseSize.z, minSize.z, maxSize.z);
    }

    //MOVEMENT CHECKER

    private void ObjectAtPosition() {

    }

    private void CheckForWall() {

    }

    private void Follow() {
        transform.position = modelPos;
    }

}
