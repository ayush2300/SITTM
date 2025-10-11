using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public float moveSpeed = 3f;
    public int expDrop = 10;

    private Transform target;
    private bool isDead = false;
    private NavMeshAgent agent;

    [Header("Collision Damage")]
    public bool canDamageOnCollision;
    public int collisionDamage;

    [Header("XPDrop")]
    public GameObject XpOrbPrefab;

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

        // Move towards the player
        agent.SetDestination(target.position);


    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!canDamageOnCollision) return;


        if(collision.gameObject.tag=="Player")
        {
            collision.gameObject.GetComponent<HealthSystem>().Damage(collisionDamage);
        }
        gameObject.GetComponent<HealthSystem>().Die();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!canDamageOnCollision) return;


        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<HealthSystem>().Damage(collisionDamage);
        }
        gameObject.GetComponent<HealthSystem>().Die();
    }


    public void SpawnXp()
    {
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
    }

    public void Freez(float freezeTime)
    {
        // Only start a freeze if not already frozen
        if (isDead || agent == null) return;

        StartCoroutine(FreezeCoroutine(freezeTime));
    }

    private IEnumerator FreezeCoroutine(float freezeTime)
    {
        // Store current speed to restore later
        float originalSpeed = agent.speed;

        // Stop movement
        agent.isStopped = true;
        agent.speed = 0f;

        // Optional: disable collision damage while frozen
        bool originalCanDamage = canDamageOnCollision;
        canDamageOnCollision = false;

        // Wait for freezeTime
        yield return new WaitForSeconds(freezeTime);

        // Resume movement
        agent.isStopped = false;
        agent.speed = originalSpeed;

        // Restore collision damage
        canDamageOnCollision = originalCanDamage;
    }
}
