using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public List<GameObject> tier1EnemyPrefabs; // Tier1 enemies (Proton, Neutron, Electron)
    public List<GameObject> tier2EnemyPrefabs; // Tier2 enemies (spawned after combination)

    public int poolSizePerEnemy = 10;
    public float spawnInterval = 3f;
    public float spawnDistance = 10f;

    private Camera mainCamera;
    private float timer;

    private List<EnemyPooler> tier1Pools = new List<EnemyPooler>();
    private List<EnemyPooler> tier2Pools = new List<EnemyPooler>();

    public static EnemySpawner Instance; // Singleton for easy access

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        mainCamera = Camera.main;

        // Create pools for Tier1 enemies
        foreach (var prefab in tier1EnemyPrefabs)
        {
            tier1Pools.Add(new EnemyPooler(prefab, poolSizePerEnemy, transform));
        }

        // Create pools for Tier2 enemies
        foreach (var prefab in tier2EnemyPrefabs)
        {
            tier2Pools.Add(new EnemyPooler(prefab, poolSizePerEnemy, transform));
        }

        timer = spawnInterval;
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            SpawnRandomTier1Enemy();
            timer = spawnInterval;
        }
    }

    void SpawnRandomTier1Enemy()
    {
        if (tier1Pools.Count == 0) return;

        // Pick a random Tier1 pool
        EnemyPooler selectedPool = tier1Pools[Random.Range(0, tier1Pools.Count)];
        GameObject enemy = selectedPool.Get();

        enemy.transform.position = GetRandomPositionOutsideCamera();
        enemy.SetActive(true);
    }

    Vector3 GetRandomPositionOutsideCamera()
    {
        float camHeight = 2f * mainCamera.orthographicSize;
        float camWidth = camHeight * mainCamera.aspect;

        int edge = Random.Range(0, 4);
        float x = 0, y = 0;

        switch (edge)
        {
            case 0: // Top
                x = Random.Range(-camWidth / 2, camWidth / 2);
                y = camHeight / 2 + spawnDistance;
                break;
            case 1: // Bottom
                x = Random.Range(-camWidth / 2, camWidth / 2);
                y = -camHeight / 2 - spawnDistance;
                break;
            case 2: // Left
                x = -camWidth / 2 - spawnDistance;
                y = Random.Range(-camHeight / 2, camHeight / 2);
                break;
            case 3: // Right
                x = camWidth / 2 + spawnDistance;
                y = Random.Range(-camHeight / 2, camHeight / 2);
                break;
        }

        Vector3 cameraPos = mainCamera.transform.position;
        return new Vector3(cameraPos.x + x, cameraPos.y + y, 0f);
    }

    // ✅ Spawn Tier2 Enemy at a specific position (called by Tier1Enemy)
    public GameObject SpawnTier2Enemy(int index, Vector3 position)
    {
        if (index < 0 || index >= tier2Pools.Count)
            return null;

        GameObject enemy = tier2Pools[index].Get();
        enemy.transform.position = position;
        enemy.SetActive(true);

        return enemy;
    }
}
