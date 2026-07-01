using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private PlayerDirection direction;

    [Header("Stamina Settings")] //PROBABLY NEED BALANCING
    [SerializeField] private float maxEnergy = 100f;
    [SerializeField] private float walkDrainRate = .2f;
    [SerializeField] private float sprintDrainRate = .8f;
    public float MaxEnergy => maxEnergy;
    public float Energy { get; private set; }

    [Header("Thirst Settings")]
    [SerializeField] private float maxThirst = 100f;
    [SerializeField] private float thirstDrainRate = .1f;
    public float MaxThirst => maxThirst;
    public float Thirst { get; private set; }
    public float ThirstMultiplier { get; set; } = 1f;

    [Header("Heatlh Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float healthRegenRate = 1f;
    [SerializeField] private float healthPoint;
    [SerializeField] private float damageThreshold = 10f;
    private float healthRegenCooldown = 0f;


    //EVENTS
    public event Action<float, float> OnEnergyChanged;
    public event Action<float, float> OnThirstChanged;
    public event Action<bool> OnDown;

    
    public bool CanSprint => Energy > 0f;

    void Awake() {
        NewDay();
    }

    void Start() {
        StartCoroutine(OnLaunch());
    }

    private IEnumerator OnLaunch() {
        yield return new WaitUntil(() =>
            SaveManager.Instance != null && SaveManager.Instance.init
        );
        float loadedThirst = SaveManager.Instance.thirst == 999f
            ? MaxThirst
            : SaveManager.Instance.thirst;

        SetThirst(loadedThirst);
    }

    void Update() {
        bool isMoving = direction.Move.magnitude > 0f;
        bool wantsSprint = direction.Sprint;
        bool isSprinting = wantsSprint && isMoving && CanSprint;

        SetThirst(Thirst - thirstDrainRate * ThirstMultiplier * Time.deltaTime);

        if (isSprinting) {
            SetEnergy(Energy - sprintDrainRate * Time.deltaTime);
        } else if (isMoving) {
            SetEnergy(Energy - walkDrainRate * Time.deltaTime);
        }


        if (healthRegenCooldown <= 0 && healthPoint < maxHealth) Regeneration();
    }

    public void NewDay() {
        SetEnergy(MaxEnergy);
    }

    //-------- STAMINA MANAGEMENT -----------

    private void SetEnergy(float value) {
        value = Mathf.Clamp(value, 0f, MaxEnergy);

        if (Mathf.Approximately(value, Energy)) {
            return;
        }

        Energy = value;
        OnEnergyChanged?.Invoke(Energy, MaxEnergy);
    }

    public void StaminaRestore(float value) {
        SetEnergy(Energy + value);
    }

    //-------- THIRST MANAGEMENT -----------

    public void Hydrate(float value) {
        SetThirst(Thirst + value);
    }

    private void SetThirst(float value) {
        value = Mathf.Clamp(value, 0f, MaxThirst);

        if (Mathf.Approximately(value, Thirst)) {
            return;
        }

        if (SaveManager.Instance != null)
            SaveManager.Instance.thirst = value;

        Thirst = value;
        OnThirstChanged?.Invoke(Thirst, MaxThirst);
    }


    //-------- DEATH MANAGEMENT -----------

    private void Down() {
        Debug.Log("Player is down!");
        OnDown?.Invoke(true);


    }

    private void OnCollisionEnter(Collision collision) {
        EvaluateImpact(collision.relativeVelocity.magnitude);
    }

    private void OnDamaged(float damage) {
        healthRegenCooldown += 20f;
        healthPoint -= damage;
        if (healthPoint <= 0) {
            healthPoint = 0;
            Down();
        }
    }

    private void Regeneration() {
        healthPoint += healthRegenRate;
        if (Mathf.Approximately(healthPoint, maxHealth) || healthPoint > maxHealth) healthPoint = maxHealth;
    }

    private void EvaluateImpact(float speed) {
        if (speed < damageThreshold) return;
        float damage = (speed - damageThreshold) * 20f;
        OnDamaged(damage);
    }

}