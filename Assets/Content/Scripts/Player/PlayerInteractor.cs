using Unity.VisualScripting;
using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private PlayerController controller;
    [SerializeField] private PlayerDirection direction;

    [Header("Normal Settings")]
    [SerializeField] private float normalDistance;
    [SerializeField] private LayerMask normalMask;

    [Header("Build Settings")]
    [SerializeField] private float buildDistance;
    [SerializeField] private LayerMask buildMask;
    public bool Select { get; private set; } = false;


    private void Update() {
        switch (controller.CurrentMode()) {
            case 1:
                NormalInteractor();
                break;
            case 2:
                BuildInteractor();
                break;
        }
    }

    //-------- NORMAL MODE -----------

    private void NormalInteractor() {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(.5f, .5f));

        if (Physics.Raycast(ray, out RaycastHit hit, normalDistance, normalMask)) {
            if (hit.collider == null) return;

            if (direction.IsKeyDown(SaveManager.Instance.interact)) {

            }

        }
    }




    //-------- BUILD MODE -----------

    private void BuildInteractor() {
        Ray ray = Camera.main.ScreenPointToRay(direction.Cursor);

        if (Physics.Raycast(ray, out RaycastHit hit, buildDistance, buildMask)) {
            if (hit.collider == null) return;

            if (direction.IsMouseDown()) {

            }

        }
    }

}
