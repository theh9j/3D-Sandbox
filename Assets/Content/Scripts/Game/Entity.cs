using UnityEngine;

public class Entity : AInteractable
{
    [SerializeField] private Rigidbody rb;

    public override void Interact(PlayerController player) {
        rb.AddForce(Vector3.up * 2, ForceMode.Impulse); 
    }

}