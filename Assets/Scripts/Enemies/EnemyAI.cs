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


    }


    public void Die()
    {
        Debug.Log("Enemy Dead");
        isDead = true;

        //    Drop XP orb
        if (XpOrbPrefab != null)
        {
            GameObject xpOrb = Instantiate(XpOrbPrefab, transform.position, Quaternion.identity);
            XpDrop orb = xpOrb.GetComponent<XpDrop>();
            if (orb != null)
            {
                orb.xpAmount = expDrop;
            }
        }

        //    Disable agent and deactivate for pooling
        if (agent != null)
                agent.ResetPath();

        gameObject.SetActive(false);
    }
}
