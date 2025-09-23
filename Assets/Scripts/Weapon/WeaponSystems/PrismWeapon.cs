using System.Collections;
using UnityEngine;

public class PrismWeapon : MonoBehaviour
{
    [Header("Weapon Settings")]
    public GameObject projectilePrefab;
    public Transform prismCenter;
    public float fireRate = 1f;
    public float projectileSpeed = 6f;
    public float spawnOffset = 0.5f;
    public float spawnDelay = 0.1f; // delay between projectiles
    public float curveDelayStep = 0.2f; // incremental curve delay per projectile

    [Range(1, 7)]
    public int projectileCount = 1; // How many projectiles to shoot (1–7)

    private float nextFireTime;

    // Rainbow colors
    private Color[] rainbowColors = new Color[]
    {
        Color.red,
        new Color(1f, 0.5f, 0f), // Orange
        Color.yellow,
        Color.green,
        Color.blue,
        new Color(0.29f, 0f, 0.51f), // Indigo
        new Color(0.56f, 0f, 1f) // Violet
    };

    void Update()
    {
        if (Time.time >= nextFireTime)
        {
            StartCoroutine(FireRainbowStaggered());
            nextFireTime = Time.time + 1f / fireRate;
        }
    }

    IEnumerator FireRainbowStaggered()
    {
        int count = Mathf.Clamp(projectileCount, 1, rainbowColors.Length);

        // Spread range
        float spread = 30f; // total spread (-15 to +15)
        float startAngle = -spread / 2f;
        float step = (count > 1) ? spread / (count - 1) : 0f;

        for (int i = 0; i < count; i++)
        {
            // Base spawn position (all from same point, with a small offset forward)
            Vector3 spawnPos = prismCenter.position + prismCenter.right * spawnOffset;

            // Compute angle for this projectile
            float angle = startAngle + step * i;

            // Rotate direction by angle
            Vector2 launchDir = Quaternion.Euler(0f, 0f, angle) * prismCenter.right;

            GameObject proj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);

            // Set projectile color
            SpriteRenderer sr = proj.GetComponent<SpriteRenderer>();
            if (sr != null) sr.color = rainbowColors[i];

            // Setup projectile
            PrismProjectile prismProj = proj.GetComponent<PrismProjectile>();
            if (prismProj != null)
            {
                prismProj.curveDelay += i * curveDelayStep;
                prismProj.Launch(launchDir, projectileSpeed, -100); // bend angle
            }

            yield return new WaitForSeconds(spawnDelay);
        }
    }
}
