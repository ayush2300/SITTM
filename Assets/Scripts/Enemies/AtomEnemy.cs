using UnityEngine;

public class AtomExplosion : MonoBehaviour
{
    public float explosionRadius = 3f;          // Radius in which explosion checks for player
    public int explosionDamage = 30;
    public GameObject explosionEffect;
    public float proximityRadius = 1.5f;        // The radius for atom to detect player and explode

    private bool exploded = false;

    private void Update()
    {
        if (exploded) return;

        // Check if any player is within proximity radius
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, proximityRadius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                Explode();
                break;
            }
        }
    }

    private void Explode()
    {
        if (exploded) return;
        exploded = true;

        // Damage all players in big explosion radius
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                var playerHealth = hit.GetComponent<HealthSystem>();
                if (playerHealth != null)
                    playerHealth.Damage(explosionDamage);
            }
        }
        
        if (explosionEffect != null)
        {
            GameObject effect = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            var ps = effect.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                Destroy(effect, ps.main.duration + ps.main.startLifetime.constantMax);
            }
        }
        Destroy(gameObject);
    }

    // Optional: for editor visualization
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, proximityRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
