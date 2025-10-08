using UnityEditorInternal.Profiling.Memory.Experimental.FileFormat;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{


    public float moveSpeed = 3f;
    //public int damage = 10;
    public float attackCooldown = 1f;
    public int expDrop = 10;

    private Transform target;
    private float attackCooldownTimer = 0f;
    private bool isDead = false;
    private NavMeshAgent agent;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.updateRotation = false;
            agent.updateUpAxis = false;
            agent.speed = moveSpeed;
        }

        target = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    private void Update()
    {
        if (isDead || target == null) return;

        if (attackCooldownTimer > 0f)
            attackCooldownTimer -= Time.deltaTime;

        if (agent != null)
        {
            agent.SetDestination(target.position);
        }

        float distanceToPlayer = Vector3.Distance(transform.position, target.position);
        if (distanceToPlayer <= agent.stoppingDistance && attackCooldownTimer <= 0f)
        {
            Attack();
        }
    }

    private void Attack()
    {
        attackCooldownTimer = attackCooldown;
        // Damage logic here, for example:
        var playerHealth = target.GetComponent<HealthSystem>();
        if (playerHealth != null)
        {
            //playerHealth.Damage(damage);
        }
    }

    public void Die()
    {
        isDead = true;
        // Handle death logic such as dropping experience
        // Example: DropExp();
        gameObject.SetActive(false);
    }
}
