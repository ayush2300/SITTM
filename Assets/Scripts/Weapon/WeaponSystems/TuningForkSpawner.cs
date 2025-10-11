using UnityEngine;

public class TuningForkSpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public GameObject tuningForkPrefab;

    [Header("Offset Settings")]
    public Vector3 spawnOffset = new Vector3(0f, 1f, 0f); // default above player

    private GameObject activeTuningFork;

    void OnEnable()
    {
        SpawnTuningFork();
    }

    void OnDisable()
    {
        ClearTuningFork();
    }

    void Update()
    {
        if (activeTuningFork != null)
        {
            // Follow player with offset
            activeTuningFork.transform.position = transform.position + spawnOffset;
        }
    }

    void SpawnTuningFork()
    {
        ClearTuningFork();
        if (tuningForkPrefab == null) return;

        activeTuningFork = Instantiate(tuningForkPrefab);
        activeTuningFork.transform.position = transform.position + spawnOffset;
        activeTuningFork.transform.SetParent(null); // optional, so it doesn’t inherit rotation/scale
    }

    void ClearTuningFork()
    {
        if (activeTuningFork != null)
        {
            Destroy(activeTuningFork);
            activeTuningFork = null;
        }
    }
}
