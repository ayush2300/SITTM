using UnityEngine;

public class FallingItem : MonoBehaviour
{
    [Header("Chance Settings")]
    public float fallChance = 0.1f;
    public float chanceMultiplier = 0.05f;

    [Header("Damage Settings")]
    public float damage = 10f;

    [Header("Ground Settings")]
    public string groundTag = "Ground"; // Editable in inspector

    [Header("Behavior Settings")]
    public bool destroyOnFall = true;
    public float gravityOnFall = 2f;

    private bool hasFallen = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            fallChance += chanceMultiplier;

            if (!hasFallen && Random.value < fallChance)
            {
                Fall();
            }
        }
    }

    void Fall()
    {
        hasFallen = true;
        var rb = gameObject.GetComponent<Rigidbody2D>();
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = gravityOnFall;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if hit ground
        if (hasFallen && collision.collider.CompareTag(groundTag))
        {
            // Optionally do something when hitting ground (e.g., stop falling)
            if (destroyOnFall)
                Destroy(gameObject);
        }

        // Check if hit player
        if (hasFallen && collision.collider.CompareTag("Player"))
        {
            var healthSys = collision.collider.GetComponent<HealthSystem>();
            if (healthSys != null)
                healthSys.Damage(Mathf.RoundToInt(damage));
            var playerCtrl = collision.collider.GetComponent<PlayerController2D>();
            if (playerCtrl != null)
                //playerCtrl.TakeDamage(damage);

            if (destroyOnFall)
                Destroy(gameObject);
        }
    }
}
