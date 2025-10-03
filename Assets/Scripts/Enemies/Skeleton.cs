using UnityEngine;

public class Skeleton2D : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 3f;

    [Header("Attack")]
    public GameObject boneProjectilePrefab;      // Assign bone projectile prefab
    public float attackRange = 5f;                // Distance to player to start throwing bones
    public float attackCooldown = 2f;             // Time between throws
    public Transform throwPoint;                   // Where the bones spawn (an empty child transform)

    private Rigidbody2D rb;
    private Transform playerTarget;
    private float attackTimer = 0f;
    private bool isDead = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerTarget = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    private void Update()
    {
        if (isDead || playerTarget == null)
            return;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTarget.position);

        if (distanceToPlayer > attackRange)
        {
            MoveTowardsPlayer();
        }
        else
        {
            rb.velocity = Vector2.zero;
            Attack();
        }
    }

    private void MoveTowardsPlayer()
    {
        Vector2 direction = (playerTarget.position - transform.position).normalized;
        rb.MovePosition(rb.position + direction * moveSpeed * Time.deltaTime);
    }

    private void Attack()
    {
        if (attackTimer <= 0f)
        {
            ThrowBone();
            attackTimer = attackCooldown;
        }
        else
        {
            attackTimer -= Time.deltaTime;
        }
    }

    private void ThrowBone()
    {
        if (boneProjectilePrefab != null && throwPoint != null && playerTarget != null)
        {
            Vector2 start = throwPoint.position;
            Vector2 target = playerTarget.position;

            float gravity = Mathf.Abs(Physics2D.gravity.y);

            float launchAngleDegrees = 45f; // Adjust angle for arc height - typically 30-60 degrees

            float launchAngleRadians = launchAngleDegrees * Mathf.Deg2Rad;

            // Calculate distance and height difference
            float distance = Vector2.Distance(start, target);
            float heightDifference = target.y - start.y;

            // Calculate initial velocity magnitude
            float initialVelocitySq = (gravity * distance * distance) / (2 * (heightDifference - Mathf.Tan(launchAngleRadians) * distance) * Mathf.Pow(Mathf.Cos(launchAngleRadians), 2));

            if (initialVelocitySq <= 0f)
            {
                // Fallback: shoot straight towards player if calculation invalid
                ThrowStraightBone();
                return;
            }

            float initialVelocity = Mathf.Sqrt(initialVelocitySq);

            // Calculate velocity components
            float vx = initialVelocity * Mathf.Cos(launchAngleRadians);
            float vy = initialVelocity * Mathf.Sin(launchAngleRadians);

            // Direction (left or right)
            Vector2 direction = (target - start).normalized;
            if (direction.x < 0) vx = -vx; // Flip horizontal velocity for left side

            // Instantiate projectile
            GameObject bone = Instantiate(boneProjectilePrefab, start, Quaternion.identity);
            Rigidbody2D boneRb = bone.GetComponent<Rigidbody2D>();

            if (boneRb != null)
            {
                boneRb.velocity = new Vector2(vx, vy);
                boneRb.angularVelocity = Random.Range(-500f, 500f);
            }
        }
    }

    // Optional fallback function
    private void ThrowStraightBone()
    {
        Vector2 direction = (playerTarget.position - throwPoint.position).normalized;

        GameObject bone = Instantiate(boneProjectilePrefab, throwPoint.position, Quaternion.identity);
        Rigidbody2D boneRb = bone.GetComponent<Rigidbody2D>();
        if (boneRb != null)
        {
            float speed = 10f;
            boneRb.velocity = direction * speed;
            boneRb.angularVelocity = Random.Range(-500f, 500f);
        }
    }



    public void Die()
    {
        isDead = true;
        // Disable object, play death animation, etc.
        gameObject.SetActive(false);
    }
}
