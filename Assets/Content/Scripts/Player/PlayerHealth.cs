using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] PlayerDirection direction;
    [SerializeField] PlayerController controller;

    [Header("Stamina Settings")] //PROBABLY NEED BALANCING
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float walkDrainRate = .2f;
    [SerializeField] private float sprintDrainRate = .8f;

    //EVENTS
    public event Action<float, float> OnStaminaChanged;

    public float MaxStamina { get; private set; }
    public float Stamina { get; private set; }
    public bool CanSprint => Stamina > 0f;

    void Awake() {
        MaxStamina = maxStamina;
        Stamina = MaxStamina;
    }

    void Start() {
        StartCoroutine(OnLaunch());
    }

    private IEnumerator OnLaunch() {
        yield return new WaitUntil(() =>
            SaveManager.Instance != null && SaveManager.Instance.init
        );
        float loadedStamina = SaveManager.Instance.stamina == 999f
            ? MaxStamina
            : SaveManager.Instance.stamina;

        SetStamina(loadedStamina);
    }

    void Update() {
        bool isMoving = direction.Move.magnitude > 0f;
        bool wantsSprint = direction.Sprint;
        bool isSprinting = wantsSprint && isMoving && CanSprint;

        if (isSprinting) {
            SetStamina(Stamina - sprintDrainRate * Time.deltaTime);
        } else if (isMoving) {
            SetStamina(Stamina - walkDrainRate * Time.deltaTime);
        }
    }

    private void SetStamina(float value) {
        value = Mathf.Clamp(value, 0f, MaxStamina);

        if (Mathf.Approximately(value, Stamina)) {
            return;
        }
        if (SaveManager.Instance != null) 
            SaveManager.Instance.stamina = value;
        Stamina = value;
        OnStaminaChanged?.Invoke(Stamina, MaxStamina);
    }

    public void StaminaRestore(float value) {
        SetStamina(Stamina + value);
    }
}