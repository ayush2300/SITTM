using System.Collections.Generic;
using UnityEngine;

public class EnemyPooler
{
    public EnemySpawnData EnemyData; // reference to spawn info
    private GameObject prefab;
    private Transform parent;
    private List<GameObject> pool;

    public EnemyPooler(GameObject prefab, int size, Transform parent = null)
    {
        this.prefab = prefab;
        this.parent = parent;
        pool = new List<GameObject>();

        for (int i = 0; i < size; i++)
        {
            GameObject obj = GameObject.Instantiate(prefab, parent);
            obj.SetActive(false);
            pool.Add(obj);
        }
    }

    public GameObject Get()
    {
        foreach (var obj in pool)
        {
            if (!obj.activeInHierarchy) return obj;
        }

        GameObject newObj = GameObject.Instantiate(prefab, parent);
        pool.Add(newObj);
        return newObj;
    }

    public void ClearPool()
    {
        foreach (var obj in pool)
            if (obj != null) obj.SetActive(false);
    }
}
