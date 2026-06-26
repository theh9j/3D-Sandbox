using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

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
    public Entity Select { get; private set; } = null;
    public bool InputLocked { get; private set; }
    public event Action openSettings;

    private void Update() {
        OnSetting();
        if (InputLocked) return;
        switch (controller.CurrentMode) {
            case Modes.Normal:
                NormalInteractor();
                break;
            case Modes.Build:
                BuildInteractor();
                break;
        }
    }

    public void SetInputLock(bool set) {
        InputLocked = set;
    }

    //-------- COMMON -----------

    private void OnSetting() {
        if (direction.IsKeyDown(Key.Escape)) {
            openSettings.Invoke();
        }
    }

    public void FreeMouse(bool free) {
        if (free) {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        } else {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    //-------- NORMAL MODE -----------

    private void NormalInteractor() {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(.5f, .5f));

        if (Physics.Raycast(ray, out RaycastHit hit, normalDistance, normalMask)) {
            if (hit.collider.TryGetComponent(out Entity interactable)) {
                if (interactable == Select) {
                    if (!Select.OnHover) Select.HoverEnter();
                    if (direction.IsKeyDown(SaveManager.Instance.interact)) {
                        Select.Interact(controller);
                    }

                } else {
                    Select?.HoverExit();
                    Select = interactable;
                }


            } else {
                Select?.HoverExit();
            }

        } else {
            Select?.HoverExit();
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
