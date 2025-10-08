using System;
using System.Collections.Generic;
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

    void Start()
    {
        // Apply blast damage instantly
        ApplyBlastDamage();

        // Destroy blast object after applying damage
        Destroy(gameObject, lifeTime);
    }

    protected void ApplyBlastDamage()
    {
        // Detect all enemies within radius
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius, enemyLayer);

        foreach (Collider2D hit in hits)
        {
            HealthSystem enemy = hit.GetComponent<HealthSystem>();
            if (enemy != null)
            {
                float distance = Vector2.Distance(transform.position, hit.transform.position);
                int damageToDeal;

                // Calculate band thresholds
                float innerRadius = radius * 0.33f; // 0–33% of radius
                float midRadius = radius * 0.66f;   // 33–66% of radius

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

                enemy.Damage(damageToDeal);
            }
        }
    }

    // Draw Gizmos for radius visualization
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius * 0.66f);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radius * 0.33f);
    }
}
