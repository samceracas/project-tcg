using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Prefab Map", menuName = "Database/Prefab Map")]
public class PrefabMap : ScriptableObject
{
    public List<PrefabMapData> entries;

    public PrefabMapData GetEntry(string key)
    {
        return entries.Find((a) => a.key.Equals(key));
    }
}

[Serializable]
public class PrefabMapData
{
    public string key;
    public GameObject prefab;
}
