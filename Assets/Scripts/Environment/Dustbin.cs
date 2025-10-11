using UnityEngine;

public class DamageItem : MonoBehaviour
{
    [Header("Damage Settings")]
    public int damageAmount = 10;

    [Header("Spawn Settings")]
    [Tooltip("Chance (0 to 1) to spawn the prefab after destruction")]
    public float spawnChance = 0.3f;

    public GameObject prefabToSpawn;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var healthSys = other.GetComponent<HealthSystem>();

            if (healthSys != null)
            {
                healthSys.Damage(damageAmount);
            }

            // Decide based on chance if prefab should spawn
            if (prefabToSpawn != null && Random.value < spawnChance)
            {
                
                Instantiate(prefabToSpawn, transform.position, Quaternion.identity);
            }

            Destroy(gameObject);
        }
    }
}
