using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PreferencesPrefabs", menuName = "Scriptable Objects/PreferencesPrefabs")]
public class PreferencesPrefabs : ScriptableObject
{
    [SerializeField] private List<PrefPrefab> prefabs = new();

    public GameObject GetPrefab(OptionsType type) {
        PrefPrefab entry = prefabs.Find(x => x.type == type);

        if (entry == null) {
            Debug.Log("No prefabs");
            return null;
        }

        return entry.prefab;
    }
}

[Serializable]
public class PrefPrefab {
    public OptionsType type;
    public GameObject prefab;
}