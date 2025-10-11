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

    [Header("Chance Settings")]
    [Tooltip("Initial chance to start shake and spawn (0 to 1, e.g. 0.4 for 40%)")]
    public float initialChance = 0.4f;
    [Tooltip("Chance multiplier added on each failed attempt (e.g. 0.05 for 5%)")]
    public float chanceMultiplier = 0.05f;

    [Header("Slow Settings")]
    [Tooltip("Multiplier to slow player speed (e.g. 0.5 means 50% speed)")]
    public float slowMultiplier = 0.5f;
    [Tooltip("Duration of slow in seconds")]
    public float slowDuration = 1f;

    private float currentChance;

    private bool playerDetectedPrevFrame = false;  // Tracks if player was inside last frame
    private bool hasSpawned = false;

    private Sequence shakeSequence;

    void Awake()
    {
        currentChance = initialChance;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

    void Update()
    {
        // Removed the early return to allow repeated triggering
        // if (hasSpawned) return;

        bool playerDetectedNow = false;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius);
        foreach (var col in hits)
        {
            if (col.CompareTag(playerTag))
            {
                playerDetectedNow = true;
                break;
            }
        }

        // Trigger only on player entering detection radius (rising edge)
        if (playerDetectedNow && !playerDetectedPrevFrame)
        {
            if (Random.value < currentChance)
            {
                StartShakeAndSpawn();
                // Optionally reset chance after successful trigger
                currentChance = initialChance;
            }
            else
            {
                currentChance += chanceMultiplier;
                currentChance = Mathf.Clamp01(currentChance);
            }
        }

        playerDetectedPrevFrame = playerDetectedNow;
    }

    void StartShakeAndSpawn()
    {
        hasSpawned = true;

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
            StartCoroutine(DealDamageOverTime(particle.transform));
        }
    }

    IEnumerator DealDamageOverTime(Transform particleTransform)
    {
        float elapsed = 0f;
        while (elapsed < damageDuration)
        {
            if (particleTransform == null) yield break;

            Collider2D[] hits = Physics2D.OverlapCircleAll(particleTransform.position, damageParticleRadius);
            foreach (var col in hits)
            {
                if (col.CompareTag(playerTag))
                {
                    var health = col.GetComponent<HealthSystem>();
                    if (health != null && !health.IsDead)
                    {
                        health.Damage(damageAmount);

                        var playerController = col.GetComponent<PlayerController2D>();
                        if (playerController != null)
                        {
                            playerController.ApplyTemporarySpeedModifier(slowMultiplier, slowDuration);
                        }
                    }
                }
            }
            yield return new WaitForSeconds(damageCheckInterval);
            elapsed += damageCheckInterval;
        }
        // Reset flag to allow triggering again on next player entry
        hasSpawned = false;
    }
}
