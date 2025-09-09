using UnityEngine;

public class Enemy : MonoBehaviour, IDamagable
{
    [Header("Enemy Settings")]
    public float maxHealth = 50f;
    private float currentHealth;

    void OnEnable()
    {
        currentHealth = maxHealth; // Reset health when enabled
    }

    // IDamagable implementation
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage. Current health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Option 1: Destroy the enemy
        // Destroy(gameObject);

        // Option 2: Use object pooling
        gameObject.SetActive(false);

        // Optional: add death effects or sounds here
        Debug.Log($"{gameObject.name} died.");
    }
}
