using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileThrowingWeapon : MonoBehaviour
{
    [Header("Projectile Settings")]
    public GameObject projectilePrefab;
    public int projectilesPerThrow = 3;
    public float throwForce = 5f;

    [Header("Throw Radius")]
    public float radius = 3f;

    [Header("Fire Rate")]
    public float fireInterval = 2f; // time between throws

    private float nextFireTime;

    void Update()
    {
        if (Time.time >= nextFireTime)
        {
            ThrowProjectiles();
            nextFireTime = Time.time + fireInterval;
        }
    }

    void ThrowProjectiles()
    {
        for (int i = 0; i < projectilesPerThrow; i++)
        {
            // Spawn at weapon position
            GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

            // Ensure it has Rigidbody2D
            Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // Pick a random point inside circle, normalize to get direction
                Vector2 randomDir = Random.insideUnitCircle.normalized;

                // Apply force
                rb.AddForce(randomDir * throwForce, ForceMode2D.Impulse);
            }
        }
    }

    // Draw gizmos in editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
