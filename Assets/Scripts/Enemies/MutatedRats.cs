using UnityEngine;
using System.Collections;

public class MutatedRats : MonoBehaviour
{
    [Header("Rat Settings")]
    public float moveSpeed = 3f;
    public int damage = 10;
    public float pauseTime = 1f;          // How long the rat stops
    public float timeBeforePause = 2f;    // Time after spawning before first pause
    public GameObject spawnObjectPrefab;  // Object to spawn when paused
    public GameObject puddleSpawnPoint;

    private Transform player;
    private Rigidbody2D rb;
    private bool isPaused = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        StartCoroutine(PauseRoutine()); // Start the pause-spawn routine
    }

    void FixedUpdate()
    {
        if (player == null || isPaused) return;

        MoveTowardsPlayer();
        FacePlayer();
    }

    void MoveTowardsPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
    }

    void FacePlayer()
    {
        if (player.position.x < transform.position.x)
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);   // face left
        else
            transform.rotation = Quaternion.Euler(0f, 180f, 0f); // face right
    }

    private IEnumerator PauseRoutine()
    {
        yield return new WaitForSeconds(timeBeforePause); // Wait before first pause

        while (true)
        {
            // Pause
            isPaused = true;

            // Spawn object
            if (spawnObjectPrefab != null)
                Instantiate(spawnObjectPrefab, puddleSpawnPoint.transform.position, Quaternion.identity);

            // Wait for pause duration
            yield return new WaitForSeconds(pauseTime);

            // Resume chasing
            isPaused = false;

            // Wait some time before next pause
            yield return new WaitForSeconds(timeBeforePause);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            IDamagable damagable = collision.gameObject.GetComponent<IDamagable>();
            if (damagable != null)
                damagable.TakeDamage(damage);
        }
    }
}
