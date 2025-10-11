using UnityEngine;

public class BlastDamage : MonoBehaviour
{
    [Header("Blast Damage Settings")]
    public float radius = 5f;
    public int minDamage = 10;
    public int midDamage = 20;
    public int maxDamage = 40;

    [Tooltip("How long before this blast disappears")]
    public float lifeTime = 0.2f;

    [Header("Layers")]
    public LayerMask enemyLayer;

    [Header("Debug")]
    public bool debug = false;

    void Start()
    {
        // Apply blast damage instantly for non-projectile blasts
        // (projectiles like LitIonProjectile usually override Start, so they won't call this)
        ApplyBlastDamage();

        // Cleanup
        Destroy(gameObject, lifeTime);
    }

    protected virtual void ApplyBlastDamage()
    {
        // Ensure we operate on the 2D plane (Z = 0)
        Vector3 blastPos = new Vector3(transform.position.x, transform.position.y, 0f);

        // Overlap search
        Collider2D[] hits = Physics2D.OverlapCircleAll(blastPos, radius, enemyLayer);

        if (debug)
        {
            Debug.Log($"[BlastDamage] Explosion at {blastPos} | radius: {radius} | hits: {hits.Length}");
            Debug.DrawLine(blastPos, blastPos + Vector3.up * 0.1f, Color.red, 1f);
        }

        // Band thresholds
        float innerRadius = radius * 0.33f; // 0–33%
        float midRadius = radius * 0.66f;   // 33–66%

        foreach (Collider2D hit in hits)
        {
            // Try to find HealthSystem on the collider or its parents
            HealthSystem health = hit.GetComponent<HealthSystem>();
            if (health == null)
                health = hit.GetComponentInParent<HealthSystem>();

            if (health != null)
            {
                float distance = Vector2.Distance(new Vector2(blastPos.x, blastPos.y), hit.transform.position);
                int damageToDeal;

                if (distance <= innerRadius)
                {
                    damageToDeal = maxDamage;
                }
                else if (distance <= midRadius)
                {
                    damageToDeal = midDamage;
                }
                else
                {
                    damageToDeal = minDamage;
                }

                // Call the existing HealthSystem method
                health.Damage(DamageItem.GetModifiedDamage(damageToDeal));


                if (debug)
                    Debug.Log($"[BlastDamage] Damaged '{hit.name}' for {damageToDeal} (dist={distance:F2})");

                if (debug)
                    Debug.DrawLine(blastPos, hit.transform.position, Color.yellow, 1f);
            }
            else
            {
                if (debug)
                    Debug.Log($"[BlastDamage] No HealthSystem on '{hit.name}' or parents.");
            }
        }

        // Note: we don't Destroy() here if a caller (like a projectile) handles cleanup,
        // but Start() above will destroy by lifeTime for non-projectile uses.
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        // Draw outer -> mid -> inner with colors similar to your old script
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius * 0.66f);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radius * 0.33f);
    }
#endif
}
