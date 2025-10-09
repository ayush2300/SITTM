using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float attackCooldown = 1f;
    public int expDrop = 10;

    private Transform target;
    private float attackCooldownTimer = 0f;
    private bool isDead = false;
    private NavMeshAgent agent;

    public GameObject xpOrbPrefab;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.updateRotation = false;
            agent.updateUpAxis = false;
            agent.speed = moveSpeed;
        }
    }

    private void OnEnable()
    {
        // Reset AI when re-activated from pool
        isDead = false;
        attackCooldownTimer = 0f;

        // Make sure the agent is properly placed on the NavMesh
        if (agent != null && !agent.isOnNavMesh)
        {
            agent.enabled = true;
            agent.Warp(transform.position); // force placement on NavMesh
        }

        target = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    private void Update()
    {
        if (isDead || target == null || agent == null || !agent.isOnNavMesh) return;

        if (attackCooldownTimer > 0f)
            attackCooldownTimer -= Time.deltaTime;

        // Move towards the player
        agent.SetDestination(target.position);

        // Attack if close enough
        float distanceToPlayer = Vector3.Distance(transform.position, target.position);
        if (distanceToPlayer <= agent.stoppingDistance && attackCooldownTimer <= 0f)
        {
            Attack();
        }
    }

    private void Attack()
    {
        attackCooldownTimer = attackCooldown;
        var playerHealth = target.GetComponent<HealthSystem>();
        if (playerHealth != null)
        {
            //playerHealth.Damage(damage);
        }
    }

    public void Die()
    {
        isDead = true;

        // Drop XP orb
        if (xpOrbPrefab != null)
        {
            GameObject xpOrb = Instantiate(xpOrbPrefab, transform.position, Quaternion.identity);
            XPOrb orb = xpOrb.GetComponent<XPOrb>();
            if (orb != null)
            {
                orb.xpAmount = expDrop;
            }
        }

        // Disable agent and deactivate for pooling
        if (agent != null)
            agent.ResetPath();

        gameObject.SetActive(false);
    }
}
