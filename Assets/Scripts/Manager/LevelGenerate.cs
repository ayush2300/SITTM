using System.Collections.Generic;
using UnityEngine;
using NavMeshPlus.Components;

public class LevelGenerator : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public List<LevelChunk> levelPrefabs;
    public NavMeshSurface navMeshSurface; // single scene-wide surface

    [Header("Initial Setup")]
    public List<LevelChunk> startingChunks; // the 3 already in the scene
    public float spawnTriggerDistance = 10f;
    public int maxActiveChunks = 7;

    private List<LevelChunk> activeChunks = new List<LevelChunk>();

    void Start()
    {
        foreach (var chunk in startingChunks)
            activeChunks.Add(chunk);
    }

    void Update()
    {
        if (player == null || activeChunks.Count == 0)
            return;

        LevelChunk rightmost = activeChunks[activeChunks.Count - 1];
        LevelChunk leftmost = activeChunks[0];

        float distToRight = Vector3.Distance(player.position, rightmost.endPoint.position);
        float distToLeft = Vector3.Distance(player.position, leftmost.startPoint.position);

        if (distToRight < spawnTriggerDistance)
            SpawnChunk(Direction.Right);

        if (distToLeft < spawnTriggerDistance)
            SpawnChunk(Direction.Left);

        CleanupOldChunks();
    }

    enum Direction { Left, Right }

    void SpawnChunk(Direction dir)
    {
        LevelChunk prefabToSpawn = levelPrefabs[Random.Range(0, levelPrefabs.Count)];
        LevelChunk newChunk = Instantiate(prefabToSpawn);

        if (dir == Direction.Right)
        {
            LevelChunk rightmost = activeChunks[activeChunks.Count - 1];
            Vector3 offset = rightmost.endPoint.position - newChunk.startPoint.position;
            newChunk.transform.position += offset;
            activeChunks.Add(newChunk);
        }
        else
        {
            LevelChunk leftmost = activeChunks[0];
            Vector3 offset = leftmost.startPoint.position - newChunk.endPoint.position;
            newChunk.transform.position += offset;
            activeChunks.Insert(0, newChunk);
        }

        // ? Rebuild navmesh to include new prefab’s modifiers
        if (navMeshSurface != null)
            navMeshSurface.BuildNavMesh();
    }

    void CleanupOldChunks()
    {
        while (activeChunks.Count > maxActiveChunks)
        {
            LevelChunk chunkToRemove = null;

            float distLeft = Vector3.Distance(player.position, activeChunks[0].endPoint.position);
            float distRight = Vector3.Distance(player.position, activeChunks[activeChunks.Count - 1].startPoint.position);

            if (distLeft > distRight)
            {
                chunkToRemove = activeChunks[0];
                activeChunks.RemoveAt(0);
            }
            else
            {
                chunkToRemove = activeChunks[activeChunks.Count - 1];
                activeChunks.RemoveAt(activeChunks.Count - 1);
            }

            if (chunkToRemove != null)
                Destroy(chunkToRemove.gameObject);
        }
    }
}
