using System.Collections;
using UnityEngine;

public class PrismWeapon : MonoBehaviour
{
    [Header("Orbit Settings")]
    public Transform target;
    public float orbitDistance = 2f;
    public float orbitSpeed = 60f;
    public Vector3 orbitAxis = Vector3.forward;

    [Header("Offset Settings")]
    public Vector3 initialOffsetDirection = Vector3.right;

    [Header("Shooting Settings")]
    public GameObject projectilePrefab;
    public float projectileSpeed = 6f;
    [Range(1, 7)] public int range = 7;
    public float spacing = 0.3f;
    public float fireRate = 10f;
    public float bendAngle = -100f;
    public float curveDelayStep = 0.02f;

    private readonly Color[] rainbowColors = new Color[]
    {
        Color.red,
        new Color(1f, 0.5f, 0f),
        Color.yellow,
        Color.green,
        Color.blue,
        new Color(0.29f, 0f, 0.51f),
        new Color(0.56f, 0f, 1f)
    };

    private Coroutine fireRoutine;
    private bool isShooting = false;

    void Start()
    {
        if (target == null && transform.parent != null)
            target = transform.parent;

        if (target != null)
            transform.position = target.position + initialOffsetDirection.normalized * orbitDistance;
    }

    void OnEnable()
    {
        StartShooting();
    }

    void OnDisable()
    {
        StopShooting();
    }

    void Update()
    {
        if (target == null) return;

        transform.RotateAround(target.position, orbitAxis, orbitSpeed * Time.deltaTime);
    }

    public void StartShooting()
    {
        if (isShooting) return;
        isShooting = true;
        fireRoutine = StartCoroutine(FireContinuously());
    }

    public void StopShooting()
    {
        if (!isShooting) return;
        isShooting = false;
        if (fireRoutine != null)
        {
            StopCoroutine(fireRoutine);
            fireRoutine = null;
        }
    }

    IEnumerator FireContinuously()
    {
        range = Mathf.Max(1, range);
        float interval = (fireRate > 0f) ? (1f / fireRate) : 0.1f;

        while (true)
        {
            FireOneShotVolley();
            yield return new WaitForSeconds(interval);
        }
    }

    void FireOneShotVolley()
    {
        if (projectilePrefab == null) return;

        int count = Mathf.Max(1, range);
        Vector3 basePos = transform.position;
        Vector3 right = transform.right.normalized;
        Vector3 up = transform.up.normalized;

        for (int i = 0; i < count; i++)
        {
            // Center the spacing around the middle
            float offsetY = (i - (count - 1) * 0.5f) * spacing;
            Vector3 spawnPos = basePos + up * offsetY;

            GameObject proj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);

            // Assign rainbow color
            Color colorToUse = rainbowColors[i % rainbowColors.Length];
            var sr = proj.GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.color = colorToUse;
            else
            {
                var mr = proj.GetComponent<MeshRenderer>();
                if (mr != null && mr.material != null && mr.material.HasProperty("_Color"))
                    mr.material.color = colorToUse;
            }

            // Launch logic
            var prismProj = proj.GetComponent<PrismProjectile>();
            if (prismProj != null)
            {
                prismProj.curveDelay += i * curveDelayStep;
                prismProj.Launch(right, projectileSpeed, bendAngle);
            }
            else
            {
                var rb2d = proj.GetComponent<Rigidbody2D>();
                if (rb2d != null)
                    rb2d.velocity = right * projectileSpeed;
                else
                {
                    var rb = proj.GetComponent<Rigidbody>();
                    if (rb != null)
                        rb.velocity = right * projectileSpeed;
                }
            }
        }
    }
}
