using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public enum InputType {
    None,
    Computer,
    Touch
}

public class PlayerDirection : MonoBehaviour
{
    [SerializeField] private PlayerInteractor interactor;
    public InputType CurrentInputType { get; private set; }
    public Vector2 LookDelta { get; private set; }
    public Vector2 Cursor {  get; private set; }
    public Vector2 Move {  get; private set; }
    public bool Jump { get; private set; }
    public bool Sprint { get; private set; } = false;


    void Update() {
        if (interactor.InputLocked) return;
        switch (CheckForInputType()) {
            case InputType.Computer:
                ComputerHandler();
                break;
            case InputType.Touch:
                TouchHandler();
                break;
            default:
                ClearInput();
                break;
        }
    }

    private InputType CheckForInputType() {
        if (Mouse.current != null || Keyboard.current != null) CurrentInputType = InputType.Computer;
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed) CurrentInputType = InputType.Touch;
        return CurrentInputType;
    }

    //------------------------------------------

    private void ComputerHandler() {
        Movement();
        MouseDelta();
        CursorPos();
    }

    private void TouchHandler() {
        Debug.Log("No Support yet, lol");
    }

    private void ClearInput() {
        LookDelta = Vector2.zero;
        Cursor = Vector2.zero;
        Move = Vector2.zero;
    }

    //------------------------------------------  COMPUTER


    private void MouseDelta() {
        LookDelta = Mouse.current == null ? Vector2.zero : Mouse.current.delta.ReadValue();
    }

    private void CursorPos() {
        Cursor = Mouse.current == null ? Vector2.zero : Mouse.current.position.ReadValue();
    }

    private void Movement() {
        Move = Keyboard.current == null ? Vector2.zero : new Vector2(
            GetKey(Key.A, Key.D),
            GetKey(Key.S, Key.W)
            );

        Jump = Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame;

        if (SaveManager.Instance.sprintToggle) {
            if (Keyboard.current != null && Keyboard.current[SaveManager.Instance.sprint].wasPressedThisFrame) Sprint = !Sprint;
        } else {
            Sprint = Keyboard.current != null && Keyboard.current[SaveManager.Instance.sprint].isPressed;
        }
    }

    private float GetKey(Key negative, Key positive) {
        float defaulted = 0f;

        if (Keyboard.current[negative].isPressed) defaulted -= 1f;
        if (Keyboard.current[positive].isPressed) defaulted += 1f;
        return defaulted;
    }

    public bool IsMouseDown() {
        return Mouse.current.leftButton.isPressed;
    }

    public bool IsKeyDown(Key key) {
        return Keyboard.current[key].wasPressedThisFrame;
    }

    //------------------------------------------  TOUCH
}
