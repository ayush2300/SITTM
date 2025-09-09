using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPooler

{
    private GameObject prefab;
    private Transform parent;
    private List<GameObject> pool;

    public EnemyPooler(GameObject prefab, int initialSize, Transform parent = null)
    {
        this.prefab = prefab;
        this.parent = parent;
        pool = new List<GameObject>();

        for (int i = 0; i < initialSize; i++)
        {
            GameObject obj = CreateNewObject();
            obj.SetActive(false);
            pool.Add(obj);
        }
    }

    private GameObject CreateNewObject()
    {
        GameObject obj = GameObject.Instantiate(prefab, parent);
        obj.SetActive(false);
        return obj;
    }

    public GameObject Get()
    {
        foreach (var obj in pool)
        {
            if (!obj.activeInHierarchy)
            {
                return obj;
            }
        }

        // Expand pool if all are in use
        GameObject newObj = CreateNewObject();
        pool.Add(newObj);
        return newObj;
    }
}
