using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Class Map", menuName = "Database/ClassMap")]
public class ClassMap : ScriptableObject
{
    public ClassEntry[] _entries;

    public ClassEntry GetEntryByKey(string key)
    {
        foreach (ClassEntry entry in _entries)
        {
            if (entry.key.Equals(key)) return entry;
        }
        return null;
    }
}

[Serializable]
public class ClassEntry
{
    public string key;
    public ClassData data;
}

[Serializable]
public class ClassData
{
    public string className;
    public GameObject prefab;
}