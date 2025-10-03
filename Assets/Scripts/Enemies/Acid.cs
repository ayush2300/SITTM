using UnityEngine;

public class AcidDamage2D : MonoBehaviour
{
    public int acidDamage = 5;
    public float lifetime = 3f;

    private void Start()
    {
        Destroy(gameObject, lifetime);  // Destroy after lifetime seconds
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
}
