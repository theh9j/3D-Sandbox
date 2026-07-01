using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnAreasHandler : MonoBehaviour
{
    [SerializeField] private List<Spawnpoint> spawnPoints = new();

    void Awake() {
        RefreshSpawnpoints();
    }

    private void RefreshSpawnpoints() {
        spawnPoints.Clear();
        spawnPoints.AddRange(transform.GetComponentsInChildren<Spawnpoint>());
    }

    public Spawnpoint GetSpawnpointType(SpawnType type) {
        switch (type) {
            case SpawnType.Inside:

                return GetRandomPointsOfSpawnList(spawnPoints.
                    Where(sp => sp.spawnType == SpawnType.Inside).
                    Select(sp => sp).
                    ToList());

            case SpawnType.Outdoor:

                return GetRandomPointsOfSpawnList(spawnPoints.
                    Where(sp => sp.spawnType == SpawnType.Outdoor)
                    .Select(sp => sp)
                    .ToList());

            default:
                Debug.LogError($"Spawnpoint of type {type} not found.");
                return null;
        }
    }

    private Spawnpoint GetRandomPointsOfSpawnList(List<Spawnpoint> spawnpoints) {
        List<Spawnpoint> validSpawns = spawnpoints
            .Where(sp => sp.transform.childCount == 0)
            .Select(sp => sp)
            .ToList();

        if (validSpawns.Count == 0) return null;
        return validSpawns[Random.Range(0, validSpawns.Count)];
    }
}   