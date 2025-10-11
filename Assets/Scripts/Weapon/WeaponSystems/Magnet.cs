using System.Collections;
using UnityEngine;

public class Magnet : MonoBehaviour
{
    [Header("Magnet Settings")]
    public float radius = 3f;              // detection radius
    public int damage = 10;                // damage to enemies
    public float damageInterval = 0.5f;    // time between repeated damage ticks

    [Header("Optional Layer Mask")]
    public LayerMask enemyLayer;           // layer to detect enemies

    private Coroutine damageRoutine;

    private void OnEnable()
    {
        // Start damaging enemies when enabled
        damageRoutine = StartCoroutine(DamageEnemiesRoutine());
    }

    private void OnDisable()
    {
        // Stop damaging when disabled
        if (damageRoutine != null)
        {
            StopCoroutine(damageRoutine);
            damageRoutine = null;
        }
    }

    private IEnumerator DamageEnemiesRoutine()
    {
        while (true)
        {
            Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, radius, enemyLayer);

            foreach (var enemyCol in enemies)
            {
                EnemyAI enemy = enemyCol.GetComponent<EnemyAI>();
                if (enemy != null)
                {
                    HealthSystem hs = enemyCol.GetComponent<HealthSystem>();
                    if (hs != null)
                    {
                        hs.Damage(damage);
                    }
                }
            }

            yield return new WaitForSeconds(damageInterval);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
