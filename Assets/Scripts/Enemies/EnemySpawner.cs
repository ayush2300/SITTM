using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Phase Setup")]
    public List<SpawnPhaseSO> spawnPhases;

    [Header("Spawn Settings")]
    public float spawnDistance = 10f;
    public Transform poolParent;

    private Camera mainCamera;
    private float elapsedTime = 0f;
    private int currentPhaseIndex = -1;

    private List<EnemyPooler> currentPools = new List<EnemyPooler>();
    private Dictionary<EnemyPooler, float> spawnTimers = new Dictionary<EnemyPooler, float>();
    private Dictionary<EnemyPooler, float> elapsedPoolTime = new Dictionary<EnemyPooler, float>();

    void Start()
    {
        mainCamera = Camera.main;
        if (spawnPhases.Count > 0)
            ActivatePhase(0);
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;

        // Check for next phase
        if (currentPhaseIndex + 1 < spawnPhases.Count &&
            elapsedTime >= spawnPhases[currentPhaseIndex + 1].startTime)
        {
            ActivatePhase(currentPhaseIndex + 1);
        }

        // Spawn enemies for each pool
        foreach (var pool in currentPools)
        {
            if (!spawnTimers.ContainsKey(pool))
            {
                spawnTimers[pool] = 0f;
                elapsedPoolTime[pool] = 0f;
            }

            elapsedPoolTime[pool] += Time.deltaTime;

            // Compute current spawn rate
            var data = pool.EnemyData;
            float t = Mathf.Clamp01(elapsedPoolTime[pool] / data.timeToIncrease);
            float currentSpawnRate = Mathf.Lerp(data.minSpawnRate, data.maxSpawnRate, t);
            float interval = 1f / currentSpawnRate;

            spawnTimers[pool] -= Time.deltaTime;
            if (spawnTimers[pool] <= 0f)
            {
                SpawnFromPool(pool);
                spawnTimers[pool] = interval;
            }
        }
    }

    void ActivatePhase(int phaseIndex)
    {
        currentPhaseIndex = phaseIndex;
        var phase = spawnPhases[phaseIndex];

        // Clear old pools if not keeping previous enemies alive
        if (!phase.keepPreviousEnemiesAlive)
        {
            foreach (var pool in currentPools)
                pool.ClearPool();

            currentPools.Clear();
            spawnTimers.Clear();
            elapsedPoolTime.Clear();
        }

        // Create pools for new enemies
        foreach (var enemyData in phase.enemiesToSpawn)
        {
            EnemyPooler pool = new EnemyPooler(enemyData.enemyPrefab, enemyData.poolSize, poolParent);
            pool.EnemyData = enemyData;
            currentPools.Add(pool);
        }
    }

    void SpawnFromPool(EnemyPooler pool)
    {
        GameObject enemy = pool.Get();
        enemy.transform.position = GetRandomPositionOutsideCamera();
        enemy.SetActive(true);
    }

    Vector3 GetRandomPositionOutsideCamera()
    {
        float camHeight = 2f * mainCamera.orthographicSize;
        float camWidth = camHeight * mainCamera.aspect;

        int side = Random.Range(0, 2); // 0 = left, 1 = right
        float x = side == 0 ? -camWidth / 2 - spawnDistance : camWidth / 2 + spawnDistance;
        float y = Random.Range(-camHeight / 2, camHeight / 2);

        Vector3 cameraPos = mainCamera.transform.position;
        return new Vector3(cameraPos.x + x, cameraPos.y + y, 0f);
    }
}
