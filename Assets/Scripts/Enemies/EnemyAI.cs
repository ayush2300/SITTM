using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 3f;

    [Header("Collision Damage")]
    public bool canDamageOnCollision = false;
    public int collisionDamage = 10;

    [Header("XP Drop")]
    public int expDrop = 10;
    public GameObject XpOrbPrefab;

    private Transform target;
    private NavMeshAgent agent;
    private HealthSystem health;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        health = GetComponent<HealthSystem>();

        if (agent != null)
        {
            agent.updateRotation = false;
            agent.updateUpAxis = false;
            agent.speed = moveSpeed;
        }
    }

    private void OnEnable()
    {
        // Reset enemy state for pooling
       // health.ResetHealth();

        if (agent != null && !agent.isOnNavMesh)
        {
            agent.enabled = true;
            agent.Warp(transform.position);
        }

        target = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    private void Update()
    {
        if (health.IsDead || target == null || agent == null || !agent.isOnNavMesh) return;

        // Move towards player
        agent.SetDestination(target.position);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandleCollision(collision.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        HandleCollision(collision.gameObject);
    }

    private void HandleCollision(GameObject other)
    {
        if (!canDamageOnCollision || health.IsDead) return;

        if (other.CompareTag("Player"))
        {
            HealthSystem playerHealth = other.GetComponent<HealthSystem>();
            if (playerHealth != null)
            {
                playerHealth.Damage(collisionDamage);

                // Enemy dies after successfully hitting the player
                health.Die();
                SpawnXp();
            }
        }
    }

    public void SpawnXp()
    {
        if (XpOrbPrefab == null) return;

        GameObject xpOrb = Instantiate(XpOrbPrefab, transform.position, Quaternion.identity);
        XpDrop orb = xpOrb.GetComponent<XpDrop>();
        if (orb != null)
            orb.xpAmount = expDrop;
    }

    public void Freeze(float freezeTime)
    {
        if (health.IsDead || agent == null) return;
        StartCoroutine(FreezeCoroutine(freezeTime));
    }

    private IEnumerator FreezeCoroutine(float freezeTime)
    {
        float originalSpeed = agent.speed;
        agent.isStopped = true;
        agent.speed = 0f;

        bool originalCanDamage = canDamageOnCollision;
        canDamageOnCollision = false;

        yield return new WaitForSeconds(freezeTime);

        agent.isStopped = false;
        agent.speed = originalSpeed;
        canDamageOnCollision = originalCanDamage;
    }
}
