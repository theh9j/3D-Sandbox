using UnityEngine;

public class Spawnpoint : MonoBehaviour
{
    public SpawnType spawnType;
    public Vector3 Position => transform.position;
    public Quaternion Rotation => transform.rotation;
}
