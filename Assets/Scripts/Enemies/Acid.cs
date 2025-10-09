using UnityEngine;

public class AcidDamage2D : MonoBehaviour
{
    public int acidDamage = 5;
    public float lifetime = 3f;
    public GameObject particleEffectPrefab;  // Assign particle prefab in inspector
    private GameObject spawnedEffect;

    private void Start()
    {
        // Spawn particle effect at this object's position and rotation
        if (particleEffectPrefab != null)
        {
            spawnedEffect = Instantiate(particleEffectPrefab, transform.position, transform.rotation);
            // Optionally parent the particle effect to this object
            spawnedEffect.transform.SetParent(transform);
        }
        Destroy(gameObject, lifetime); // Destroy after lifetime seconds
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var health = other.GetComponent<HealthSystem>();
            if (health != null)
            {
                health.Damage(acidDamage);
            }
        }
    }

    private void OnDestroy()
    {
        // Also destroy the particle effect when this object is destroyed
        if (spawnedEffect != null)
        {
            Destroy(spawnedEffect);
        }
    }
}
