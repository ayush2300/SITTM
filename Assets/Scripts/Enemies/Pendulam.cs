using UnityEngine;
using UnityEngine.AI;

public class PendulumEnemy2D : MonoBehaviour
{
    [Header("Movement")]
    public float stoppingDistance = 5f;
    public float moveSpeed = 3.5f;

    [Header("Wave Effect")]
    public GameObject frequencyWaveParticles;   // Particle system GameObject (child or prefab)
    public float attackCooldown = 3f;

    private NavMeshAgent agent;
    private Transform playerTarget;
    private float attackTimer;
    private bool isDead = false;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.speed = moveSpeed;
            agent.updateRotation = false;
            agent.updateUpAxis = false;
            agent.stoppingDistance = stoppingDistance;
        }

        playerTarget = GameObject.FindGameObjectWithTag("Player")?.transform;
        attackTimer = attackCooldown;

    }

    private void Update()
    {
        if (isDead || playerTarget == null || agent == null)
            return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);

        if (distanceToPlayer > stoppingDistance)
        {
            agent.isStopped = false;
            agent.SetDestination(playerTarget.position);

            // Disable wave particle while moving
            if (frequencyWaveParticles != null)
                frequencyWaveParticles.SetActive(true);
        }
        else
        {
            agent.isStopped = true;

            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0f)
            {
                SpawnWave();
                attackTimer = attackCooldown;
            }
        }
    }

    private void SpawnWave()
    {
        if (frequencyWaveParticles != null)
        {
            Quaternion rotation = Quaternion.Euler(0f, 0f, 90f);  // Rotate 90 degrees around z-axis

            // Instantiate the particle effect at Pendulum's position with z = 90 rotation
            GameObject psObj = Instantiate(frequencyWaveParticles, transform.position, rotation);

            ParticleSystem ps = psObj.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                Destroy(psObj, ps.main.duration);
            }
            else
            {
                Destroy(psObj, 2f);
            }
        }

        // TODO: Add debuff logic here if needed, e.g., increase weapon cooldown
    }





    public void Die()
    {
        isDead = true;
        agent.isStopped = true;
        gameObject.SetActive(false);
    }
}
