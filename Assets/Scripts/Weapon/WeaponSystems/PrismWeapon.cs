using System.Collections;
using UnityEngine;

public class PrismWeapon : MonoBehaviour
{
    [Header("Weapon Settings")]
    public GameObject projectilePrefab;
    public Transform prismCenter;
    public float projectileSpeed = 6f;
    public float spawnOffset = 0.5f;
    public float spawnDelay = 0.1f;
    public float curveDelayStep = 0.2f;

    [Range(1, 7)]
    public int projectileCount = 7;    // assign in prefab (1 = single, 7 = rainbow)
    public int cycles = 1;             // how many loops through 4 directions
    public float spreadAngle = 30f;
    public float bendAngle = -100f;

    // Rainbow colors
    private Color[] rainbowColors = new Color[]
    {
        Color.red,
        new Color(1f, 0.5f, 0f), // Orange
        Color.yellow,
        Color.green,
        Color.blue,
        new Color(0.29f, 0f, 0.51f), // Indigo
        new Color(0.56f, 0f, 1f)      // Violet
    };

    void Start()
    {
        StartCoroutine(FireInCycles());
    }

    IEnumerator FireInCycles()
    {
        Vector2[] dirs = { Vector2.right, Vector2.down, Vector2.left, Vector2.up };

        for (int c = 0; c < cycles; c++)
        {
            foreach (var dir in dirs)
            {
                yield return StartCoroutine(FireVolley(dir));
                yield return new WaitForSeconds(0.2f); // pause between directions
            }
        }

        Destroy(gameObject); // done after all cycles
    }

    IEnumerator FireVolley(Vector2 baseDir)
    {
        int count = Mathf.Clamp(projectileCount, 1, rainbowColors.Length);
        float startAngle = -spreadAngle / 2f;
        float step = (count > 1) ? spreadAngle / (count - 1) : 0f;

        for (int i = 0; i < count; i++)
        {
            float angle = startAngle + step * i;
            Vector2 launchDir = Quaternion.Euler(0f, 0f, angle) * baseDir;

            Vector3 spawnPos = prismCenter.position + (Vector3)(baseDir.normalized * spawnOffset);
            GameObject proj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);

            // Set projectile color (single or rainbow depending on count)
            SpriteRenderer sr = proj.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.color = (projectileCount == 1) ? rainbowColors[0] : rainbowColors[i];
            }

            // Launch projectile
            PrismProjectile prismProj = proj.GetComponent<PrismProjectile>();
            if (prismProj != null)
            {
                prismProj.curveDelay += i * curveDelayStep;
                prismProj.Launch(launchDir, projectileSpeed, bendAngle);
            }

            yield return new WaitForSeconds(spawnDelay);
        }
    }
}
