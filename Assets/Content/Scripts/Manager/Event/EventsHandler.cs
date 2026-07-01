using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EventsHandler", menuName = "Scriptable Objects/EventsHandler")]
public class EventsHandler : ScriptableObject
{
    public List<IndividualEvent> eventData;
}

public abstract class IndividualEvent : ScriptableObject {

    public Events eventID;
    public SpawnType spawnType;
    public int eventWeight;
    public GameObject eventPrefab = null;

    public bool globalObject = false;
    

    public virtual bool EventCondition() => true;

    public virtual void EventAction() { }

    public virtual void EventSpawn(Vector3 pos, Quaternion rot, Transform parent, Transform global) {
        if (eventPrefab == null) return;
        if (globalObject) parent = global;
        GameObject obj = Instantiate(eventPrefab, pos, rot, parent);

        Collider col = obj.GetComponent<Collider>();

        if (col != null) {
            float bottomOffset = col.bounds.min.y - obj.transform.position.y;
            obj.transform.position -= new Vector3(0f, bottomOffset, 0f);
        }
    }
}