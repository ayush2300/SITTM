using UnityEngine;
using DG.Tweening;
using System.Collections;

public class ShakeAndSpawnDamage : MonoBehaviour
{
    [Header("Detection")]
    public float detectionRadius = 2f;
    public string playerTag = "Player";

    [Header("Shake Settings")]
    public float shakeDuration = 3f;       // Shake duration before spawning particle
    public float shakeStrength = 10f;      // Degrees to shake on Z axis
    public int shakeVibrato = 20;

    [Header("Damage Settings")]
    public GameObject damageParticlePrefab;  // Particle prefab to spawn that deals damage
    public float damageDuration = 2f;         // How long the particle lasts before destruction
    public int damageAmount = 10;              // Amount of damage to deal

    public float damageCheckInterval = 0.5f;  // Interval between damage applications while particle active
    public float damageParticleRadius = 1f;   // Radius around particle to detect and damage players

    private bool playerDetected = false;
    private bool hasSpawned = false;

    private Sequence shakeSequence;

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

    void Update()
    {
        if (!playerDetected && !hasSpawned)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius);
            foreach (var col in hits)
            {
                if (col.CompareTag(playerTag))
                {
                    StartShakeAndSpawn();
                    break;
                }
            }
        }
    }

    void StartShakeAndSpawn()
    {
        playerDetected = true;

        shakeSequence = DOTween.Sequence();
        shakeSequence.Append(transform.DOPunchRotation(new Vector3(0, 0, shakeStrength), shakeDuration, shakeVibrato));
        shakeSequence.OnComplete(() =>
        {
            SpawnDamageParticle();
        });
    }

    void SpawnDamageParticle()
    {
        if (damageParticlePrefab != null)
        {
            GameObject particle = Instantiate(damageParticlePrefab, transform.position, Quaternion.identity);
            Destroy(particle, damageDuration);
            // Start damage coroutine to apply damage over damageDuration
            StartCoroutine(DealDamageOverTime(particle.transform));
        }
        hasSpawned = true;
    }

    IEnumerator DealDamageOverTime(Transform particleTransform)
    {
        float elapsed = 0f;
        while (elapsed < damageDuration)
        {
            // Detect players inside damage radius around particle
            Collider2D[] hits = Physics2D.OverlapCircleAll(particleTransform.position, damageParticleRadius);
            foreach (var col in hits)
            {
                if (col.CompareTag(playerTag))
                {
                    var health = col.GetComponent<HealthSystem>();
                    if (health != null && !health.IsDead)
                    {
                        health.Damage(damageAmount);
                    }
                }
            }
            yield return new WaitForSeconds(damageCheckInterval);
            elapsed += damageCheckInterval;
        }
    }
}
