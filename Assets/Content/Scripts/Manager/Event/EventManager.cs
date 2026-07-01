using UnityEngine;

public class EventManager : MonoBehaviour
{

    [SerializeField] private EnvironmentManager env;
    [SerializeField] private EventsHandler data;
    [SerializeField] private SpawnAreasHandler spawn;

    [Header("Sorting Location")]
    [SerializeField] private Transform eventFolder;

    [Header("Settings")]
    [SerializeField] private int eventSpawnChance = 2;
    [SerializeField] private float delayUntilChanceOfEvent = 60f;

    private float time = 0f;

    void Update() {
        time += Time.deltaTime;

        if (time >= delayUntilChanceOfEvent) {
            time = 0f;
            OnEvent();
        }

    }

    private void OnEvent() {
        Debug.Log("Attempt Spawning");
        bool happen = Random.Range(0, eventSpawnChance) == 1;
        if (!happen) return;

        Debug.Log("Event Spawned");
        IndividualEvent e = RandomEvent();
        Spawnpoint sp = spawn.GetSpawnpointType(e.spawnType);
        if (sp == null) {
            Debug.Log("Out of spawn location");
            return;
        }

        e.EventSpawn(sp.Position, sp.Rotation, sp.transform, eventFolder);
    }

    private IndividualEvent RandomEvent() {
        int totalWeight = 0;
        foreach (var e in data.eventData) {
            if (e.EventCondition())
                totalWeight += e.eventWeight;
        }
        int randomWeight = Random.Range(0, totalWeight);
        int currentWeight = 0;
        foreach (var e in data.eventData) {
            if (!e.EventCondition()) continue;
            currentWeight += e.eventWeight;
            if (randomWeight < currentWeight) {
                return e;
            }
        }
        return null;
    }
}
