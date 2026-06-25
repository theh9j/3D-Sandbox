using System;
using System.Collections;
using UnityEngine;

public enum Modes {
    Normal,
    Build,
    Spectate
}

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerDirection direction;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private Camera cam;

    private Modes currentMode;

    [Header("Movement Multiplier")]
    [SerializeField] private float baseSpeed = 5f;
    [SerializeField] private float buildSpeed = 10f;
    [SerializeField] private readonly float jumpForce = 7f;

    [Header("Ground")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float groundDist = 1.1f;

    [Header("Sensitivity")]
    [SerializeField] private float sensitivity = .15f;
    [SerializeField] private float fov = 70f;
    [SerializeField] private readonly float minPitch = -85f;
    [SerializeField] private readonly float maxPitch = 85f;

    private float yaw;
    private float pitch;
    private bool jumpQueued = false;

    private Vector3 spawPos;
    private Quaternion spawnRot;

    void Awake() {
        if (rb == null) {
            rb = transform.GetComponent<Rigidbody>();
        }

        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    void Start() {
        
        yaw = transform.eulerAngles.y;

        ChangeMode(Modes.Normal);
        NormalMode();
        StartCoroutine(OnLaunch());
    }

    void Update() {
        if (WorldManager.Instance != null && transform.position.y < WorldManager.Instance.DepthLimit) {
            Respawn();
        }

        if (cam != null) {
            cam.fieldOfView = fov;
        }

        if (currentMode == Modes.Normal) {
            NormalLook();
        }

        if (currentMode == Modes.Normal && direction.Jump) {
            jumpQueued = true;
        }
    }

    private IEnumerator OnLaunch() {
        yield return new WaitUntil(() => SaveManager.Instance.init);
        if (SaveManager.Instance != null) {
            sensitivity = SaveManager.Instance.sensitivity;
            fov = SaveManager.Instance.fov;
        }
    }

    void FixedUpdate() {
        switch (currentMode) {
            case Modes.Normal:
                NormalMove();
                break;
            case Modes.Build:
                BuildMove();
                break;
            default:
                break;
        }
    }

    public int CurrentMode() {
        return currentMode switch {
            Modes.Normal => 1,
            Modes.Build => 2,
            Modes.Spectate => 3, //STC
            _ => 0
        };
    }

    public void ChangeSensitivity(float sensitivity) {
        this.sensitivity = sensitivity;
    }

    public void ChangeFOV(float fov) {
        this.fov = fov;
    }

    public bool ChangeMode(string mode) {
        if (!Enum.TryParse(mode, out Modes newMode)) return false;
        return ChangeMode(newMode);
    }

    //-------- NORMAL MODE -----------

    private void NormalLook() {
        Vector2 look = direction.LookDelta * sensitivity;

        yaw += look.x;
        pitch -= look.y;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    private void NormalMove() {
        Vector3 move =
            transform.right * direction.Move.x +
            transform.forward * direction.Move.y;

        if (move.magnitude > 1f)
            move.Normalize();

        Vector3 velocity = rb.linearVelocity;

        velocity.x = move.x * baseSpeed;
        velocity.z = move.z * baseSpeed;

        rb.linearVelocity = velocity;

        if (jumpQueued && IsGrounded()) {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        jumpQueued = false;
    }

    private void NormalMode() {
        if (currentMode != Modes.Normal) return;
        rb.useGravity = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Respawn() {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        transform.SetPositionAndRotation(spawPos, spawnRot);
    }

    //-------- BUILD MODE -----------

    private void BuildMove() {
        Vector3 move = transform.right * direction.Move.x + transform.forward * direction.Move.y;

        if (direction.Jump) move += Vector3.up;

        if (move.magnitude > 1f) move.Normalize();

        rb.linearVelocity = move * buildSpeed;
    }

    private void BuildMode() {
        if (currentMode != Modes.Build) return;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    //-------- HELPERS -----------

    private bool ChangeMode(Modes mode) {
        if (mode == currentMode) return false;

        currentMode = mode;
        switch (currentMode) {
            case Modes.Normal:
                NormalMode();
                break;
            case Modes.Build:
                BuildMode(); 
                break;
        }

        return true;
    }
    private bool IsGrounded() {
        return Physics.Raycast(transform.position, Vector3.down, groundDist, groundMask);
    }
}
