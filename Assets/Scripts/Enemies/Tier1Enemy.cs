using UnityEngine;
using System.Collections.Generic;

public class Tier1Enemy : MonoBehaviour
{
    public enum EnemyType { Proton, Neutron, Electron }
    public EnemyType enemyType;

    [Header("Settings")]
    public float moveSpeed = 4.5f; // Increased for snappier feel
    public int damage = 10;
    public float combineRange = 1.5f;

    private Transform player;
    private Rigidbody2D rb;

    // Track active Tier1 enemies for combination logic
    private static List<Tier1Enemy> activeEnemies = new List<Tier1Enemy>();

    // ✅ Prevent multiple combinations at once
    private bool isCombining = false;

    void OnEnable()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        activeEnemies.Add(this);
        isCombining = false;
    }

    void OnDisable()
    {
        activeEnemies.Remove(this);
    }

    void FixedUpdate()
    {
        if (player != null && !isCombining)
        {
            MoveTowardsPlayer();
            CheckForCombination();
        }
    }

    void MoveTowardsPlayer()
    {
        // ✅ Direct snapping movement like Vampire Survivors
        Vector2 targetPosition = Vector2.MoveTowards(rb.position, player.position, moveSpeed * Time.fixedDeltaTime);
        rb.MovePosition(targetPosition);
    }

    void CheckForCombination()
    {
        bool hasProton = false, hasNeutron = false, hasElectron = false;
        List<Tier1Enemy> nearbyEnemies = new List<Tier1Enemy>();

        foreach (Tier1Enemy enemy in activeEnemies)
        {
            if (Vector2.Distance(transform.position, enemy.transform.position) <= combineRange)
            {
                nearbyEnemies.Add(enemy);
                if (enemy.enemyType == EnemyType.Proton) hasProton = true;
                if (enemy.enemyType == EnemyType.Neutron) hasNeutron = true;
                if (enemy.enemyType == EnemyType.Electron) hasElectron = true;
            }
        }

        if (hasProton && hasNeutron && hasElectron)
        {
            CombineEnemies(nearbyEnemies);
        }
    }

    void CombineEnemies(List<Tier1Enemy> nearbyEnemies)
    {
        isCombining = true;

        // ✅ Disable only Proton, Neutron, Electron in range
        foreach (Tier1Enemy enemy in nearbyEnemies)
        {
            if (enemy.enemyType == EnemyType.Proton || enemy.enemyType == EnemyType.Neutron || enemy.enemyType == EnemyType.Electron)
            {
                enemy.gameObject.SetActive(false);
            }
        }

        // ✅ Spawn new Tier2 enemy from Tier2 pool
        if (EnemySpawner.Instance != null)
        {
            // 0 = First Tier2 enemy in list (you can change index for different Tier2 types)
            GameObject newEnemy = EnemySpawner.Instance.SpawnTier2Enemy(0, transform.position);
            if (newEnemy != null)
            {
                newEnemy.SetActive(true);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            IDamagable damagable = other.GetComponent<IDamagable>();
            if (damagable != null)
            {
                damagable.TakeDamage(damage);
            }
        }
    }

}
