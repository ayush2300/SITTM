using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public GameObject WeaponPrefab;  // Assign prefab with PrismWeapon attached

    private void Start()
    {
        if (WeaponPrefab == null)
        {
            Debug.LogWarning("PrismWeaponSpawner: No prefab assigned!");
            return;
        }

        // Spawn at this spawner's world position
        Instantiate(WeaponPrefab, transform.position, Quaternion.identity);
    }
}
