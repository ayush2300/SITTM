using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpawnPhase", menuName = "Enemy Spawning/Spawn Phase")]
public class SpawnPhaseSO : ScriptableObject
{
    [Header("Phase Timing")]
    [Tooltip("When this phase starts (in seconds).")]
    public float startTime;

    [Header("Enemy Spawn Data")]
    public List<EnemySpawnData> enemiesToSpawn = new List<EnemySpawnData>();

    [Header("Behavior")]
    [Tooltip("Keep enemies from the previous phase alive after switching.")]
    public bool keepPreviousEnemiesAlive = true;
}

[Serializable]
public class EnemySpawnData
{
    public GameObject enemyPrefab;

    [Header("Spawn Rate (Enemies per Second)")]
    [Tooltip("Spawn rate at the start of the phase (enemies/sec).")]
    public float minSpawnRate = 1f;

    [Tooltip("Maximum spawn rate when fully ramped (enemies/sec).")]
    public float maxSpawnRate = 10f;

    [Tooltip("Time in seconds to go from minSpawnRate to maxSpawnRate.")]
    public float timeToIncrease = 60f;

    [Header("Pooling")]
    [Tooltip("How many enemies to pool for this prefab.")]
    public int poolSize = 20;
}
