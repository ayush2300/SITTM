using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public GameObject WeaponPrefab;  // Assign prefab with PrismWeapon attached

    [Header("Spawn Offset")]
    public Vector2 spawnOffset = Vector2.zero; // Offset from spawner position (X,Y)

    private void Start()
    {
        if (WeaponPrefab == null)
        {
            Debug.LogWarning("WeaponSpawner: No prefab assigned!");
            return;
        }

        Vector3 spawnPos = transform.position + new Vector3(spawnOffset.x, spawnOffset.y, 0f);
        Instantiate(WeaponPrefab, spawnPos, Quaternion.identity);
    }
}
