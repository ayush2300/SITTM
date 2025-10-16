using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro; // ✅ for TextMeshProUGUI

public class EnemySpawner : MonoBehaviour
{
    [Header("Phase Setup")]
    public List<SpawnPhaseSO> spawnPhases;

    [Header("Spawn Settings")]
    public float spawnDistance = 10f;
    public Transform poolParent;

    [Header("UI")]
    public TextMeshProUGUI timerText; // ✅ displays elapsed time
    public GameObject EndPanel;

    [Header("Debug Settings")]
    public bool debugMode = false;           // ✅ enables debug time mode
    [Range(0f, 900f)]
    public float debugStartTime = 0f;        // ✅ start elapsed time manually in seconds

    private Camera mainCamera;
    private float elapsedTime = 0f;
    private int currentPhaseIndex = -1;

    private List<EnemyPooler> currentPools = new List<EnemyPooler>();
    private Dictionary<EnemyPooler, float> spawnTimers = new Dictionary<EnemyPooler, float>();
    private Dictionary<EnemyPooler, float> elapsedPoolTime = new Dictionary<EnemyPooler, float>();

    void Start()
    {
        mainCamera = Camera.main;

        // ✅ Apply debug starting time if enabled
        elapsedTime = debugMode ? debugStartTime : 0f;

        if (spawnPhases.Count > 0)
        {
            // ✅ Find which phase we should start at based on debugStartTime
            int startPhase = 0;
            for (int i = 0; i < spawnPhases.Count; i++)
            {
                if (elapsedTime >= spawnPhases[i].startTime)
                    startPhase = i;
            }

            ActivatePhase(startPhase);
        }

        UpdateTimerUI();
    }

    void Update()
    {
        // ✅ Skip normal time progression if debug mode is paused or controlled externally
        if (!debugMode)
            elapsedTime += Time.deltaTime;

        UpdateTimerUI();

        // End panel after 15 minutes (900 sec)
        if (elapsedTime >= 901)
        {
            EndPanel.SetActive(true);
        }

        // ✅ Check for next phase
        if (currentPhaseIndex + 1 < spawnPhases.Count &&
            elapsedTime >= spawnPhases[currentPhaseIndex + 1].startTime)
        {
            ActivatePhase(currentPhaseIndex + 1);
        }

        // ✅ Handle enemy spawning
        foreach (var pool in currentPools)
        {
            if (!spawnTimers.ContainsKey(pool))
            {
                spawnTimers[pool] = 0f;
                elapsedPoolTime[pool] = 0f;
            }

            elapsedPoolTime[pool] += Time.deltaTime;

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

    void UpdateTimerUI()
    {
        if (timerText == null) return;

        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    void ActivatePhase(int phaseIndex)
    {
        currentPhaseIndex = phaseIndex;
        var phase = spawnPhases[phaseIndex];

        // ✅ Clear old pools if not keeping previous enemies alive
        if (!phase.keepPreviousEnemiesAlive)
        {
            foreach (var pool in currentPools)
                pool.ClearPool();

            currentPools.Clear();
            spawnTimers.Clear();
            elapsedPoolTime.Clear();
        }

        // ✅ Create pools for new enemies
        foreach (var enemyData in phase.enemiesToSpawn)
        {
            EnemyPooler pool = new EnemyPooler(enemyData.enemyPrefab, enemyData.poolSize, poolParent);
            pool.EnemyData = enemyData;
            currentPools.Add(pool);
        }

        Debug.Log($"[EnemySpawner] Activated Phase {phaseIndex} at {elapsedTime:F1}s");
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
        Vector3 randomPos = new Vector3(cameraPos.x + x, cameraPos.y + y, 0f);

        // ✅ Try to find nearest NavMesh point
        if (NavMesh.SamplePosition(randomPos, out NavMeshHit hit, 5f, NavMesh.AllAreas))
        {
            return hit.position;
        }
        else
        {
            randomPos.x = cameraPos.x + (side == 0 ? -camWidth / 2 : camWidth / 2);
            if (NavMesh.SamplePosition(randomPos, out hit, 10f, NavMesh.AllAreas))
                return hit.position;

            return cameraPos;
        }
    }
}
