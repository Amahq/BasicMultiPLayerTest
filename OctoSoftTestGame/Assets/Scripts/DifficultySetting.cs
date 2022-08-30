using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu, Serializable]
public class DifficultySetting : ScriptableObject
{
    public string name;
    public float spawnRate;
    public float SpawnRateModifierPerObject;
    public float MinimumSpawnRate;
    public int MaxObjectAmount;
}
