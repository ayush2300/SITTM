using UnityEngine;
using System.Collections.Generic;

public class Tier1Enemy : MonoBehaviour
{
    public enum EnemyType { Proton, Neutron, Electron }
    public EnemyType enemyType;

    [Header("Settings")]
    public float moveSpeed = 4.5f;
    public int damage = 10;
    public float combineRange = 1.5f;

    private Transform player;
    private Rigidbody2D rb;

    // Track active Tier1 enemies for combination logic
    private static List<Tier1Enemy> activeEnemies = new List<Tier1Enemy>();

    private bool isCombining = false;

    void OnEnable()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

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

        // Deactivate only the combined Tier1 enemies
        foreach (Tier1Enemy enemy in nearbyEnemies)
        {
            if (enemy.enemyType == EnemyType.Proton || enemy.enemyType == EnemyType.Neutron || enemy.enemyType == EnemyType.Electron)
            {
                enemy.gameObject.SetActive(false);
            }
        }

        // Spawn Tier2 enemy from current active phase pools
        //if (EnemySpawner.Instance != null && EnemySpawner.Instance.CurrentPools.Count > 0)
        //{
        //    // Assumes the first pool in CurrentPools is the Tier2 enemy pool
        //    foreach (var pool in EnemySpawner.Instance.CurrentPools)
        //    {
        //        if (pool.EnemyData.enemyPrefab != null) // basic check
        //        {
        //            GameObject newEnemy = pool.Get();
        //            newEnemy.transform.position = transform.position;
        //            newEnemy.SetActive(true);
        //            break; // spawn only one
        //        }
        //    }
        //}
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
