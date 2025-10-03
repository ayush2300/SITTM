using UnityEngine;

public class BoneProjectile2D : MonoBehaviour
{
    public int damage = 10;
    public float lifetime = 5f;

    private void Start()
    {
        // Destroy the bone after 'lifetime' seconds to avoid clutter
        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Deal damage to player if HealthSystem exists
            var health = collision.gameObject.GetComponent<HealthSystem>();
            if (health != null)
            {
                health.Damage(damage);
            }

            // Destroy bone on hitting player
            Destroy(gameObject);
        }
        else if (!collision.gameObject.CompareTag("Enemy"))
        {
            // Optionally destroy bone if hitting other objects except enemies
            Destroy(gameObject);
        }
    }
}
