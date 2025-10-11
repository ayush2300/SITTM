using System.Collections;
using UnityEngine;

public class EnvironmentSpawner2D : MonoBehaviour
{
    public GameObject[] assetPrefabs;       // Prefab assets to spawn, assigned in inspector
    public Transform[] spawnPoints;         // Spawn locations in the scene
    public float minInterval = 5f;           // Minimum spawn interval in seconds
    public float maxInterval = 15f;          // Maximum spawn interval in seconds
    public int itemsPerSpawn = 2;             // Number of items to spawn each time per location

    // Track spawned objects per spawn point and per item
    private GameObject[][] spawnedItems;

    void Start()
    {
        // Initialize the array to track spawned objects for each spawn point and items per spawn
        spawnedItems = new GameObject[spawnPoints.Length][];
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            spawnedItems[i] = new GameObject[itemsPerSpawn];
            // Initialize entries to null
            for (int j = 0; j < itemsPerSpawn; j++)
            {
                spawnedItems[i][j] = null;
            }
            StartCoroutine(SpawnRoutine(i));
        }
    }

    IEnumerator SpawnRoutine(int spawnIndex)
    {
        while (true)
        {
            // Wait until all previously spawned items at this spawn point are destroyed or null
            bool allDestroyed;
            do
            {
                allDestroyed = true;
                for (int j = 0; j < itemsPerSpawn; j++)
                {
                    // Unity safe check: if reference exists and the object not destroyed
                    if (spawnedItems[spawnIndex][j])
                    {
                        allDestroyed = false;
                        break;
                    }
                }
                yield return null; // wait for next frame
            }
            while (!allDestroyed);

            // Wait a random interval before spawning new items
            float waitTime = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(waitTime);

            // Spawn new prefabs at the spawn point and keep references
            for (int j = 0; j < itemsPerSpawn; j++)
            {
                int prefabIndex = Random.Range(0, assetPrefabs.Length);
                GameObject prefabToSpawn = assetPrefabs[prefabIndex];

                spawnedItems[spawnIndex][j] = Instantiate(prefabToSpawn, spawnPoints[spawnIndex].position, Quaternion.identity);
            }
        }
    }
}
