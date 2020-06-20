using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Unit Map", menuName = "Database/Unit Map")]
public class UnitMap : ScriptableObject
{
    public List<UnitData> entries;

    public UnitData GetUnitDataByID(string id)
    {
        return entries.Find((a) => a.ID.Equals(id));
    }
}


[Serializable]
public class GameObjectDictionary : SerializableDictionary<string, GameObject> { }
[Serializable]
public class StringDictionary : SerializableDictionary<string, object> { }

[Serializable]
public class UnitData
{
    public string ID;
    public bool IsClass = false;
    public string ClassName = "";
    public GameObject UnitPrefab;
    public GameObject UnitFrame;
}