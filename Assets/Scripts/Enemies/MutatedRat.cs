using UnityEngine;

public class MutatedRat2D : MonoBehaviour
{
    [Header("Movement & Damage")]
    public float moveSpeed = 3.5f;

    [Header("Acid Spawning")]
    public GameObject acidParticlePrefab;   // Prefab for particle effect
    public GameObject acidSpritePrefab;     // Prefab for damage-dealing sprite
    public float spawnRadius = 3f;
    public float acidSpawnInterval = 2f;

    private Rigidbody2D rb;
    private Transform playerTarget;
    private bool isDead = false;

    private float spawnTimer = 0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerTarget = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    private void Update()
    {
        if (isDead) return;

        if (playerTarget != null)
        {
            Vector2 direction = (playerTarget.position - transform.position).normalized;
            rb.MovePosition(rb.position + direction * moveSpeed * Time.deltaTime);

            float distanceToPlayer = Vector2.Distance(transform.position, playerTarget.position);
            if (distanceToPlayer <= spawnRadius)
            {
                if (spawnTimer <= 0f)
                {
                    SpawnAcid();
                    spawnTimer = acidSpawnInterval;
                }
            }
        }

        spawnTimer -= Time.deltaTime;
    }

    private void SpawnAcid()
    {
        float lifetime = 2f; // Set the lifetime to match your particle effect duration

        if (acidParticlePrefab != null)
        {
            GameObject particleObj = Instantiate(acidParticlePrefab, transform.position, Quaternion.identity);
            Destroy(particleObj, lifetime);
        }

        if (acidSpritePrefab != null)
        {
            GameObject spriteObj = Instantiate(acidSpritePrefab, transform.position, Quaternion.identity);
            Destroy(spriteObj, lifetime);
        }
    }

}
