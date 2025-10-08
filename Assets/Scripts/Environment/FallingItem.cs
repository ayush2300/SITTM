using UnityEngine;
using DG.Tweening;

public class FallingItem : MonoBehaviour
{
    [Header("Chance Settings")]
    [Tooltip("Base chance for the item to fall when player approaches.")]
    public float fallChance = 0.1f;

    [Tooltip("Increment to the fall chance each time the player approaches.")]
    public float chanceMultiplier = 0.05f;

    [Header("Damage Settings")]
    [Tooltip("Damage dealt to the player on hit.")]
    public int damage = 10;

    [Header("Fall Animation Settings")]
    [Tooltip("Transform position where the item falls to.")]
    public Transform fallTarget;

    [Tooltip("Duration of the fall animation.")]
    public float fallDuration = 1.0f;

    [Header("Behavior Settings")]
    [Tooltip("Destroy item after falling and hitting player or ground.")]
    public bool destroyOnFall = true;

    private bool hasFallen = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            fallChance += chanceMultiplier;

            if (!hasFallen && Random.value < fallChance)
            {
                StartFall();
            }
        }
    }

    private void StartFall()
    {
        hasFallen = true;

        Vector3 targetPosition = fallTarget != null ? fallTarget.position : transform.position;

        transform.DOMove(targetPosition, fallDuration)
            .SetEase(Ease.InQuad)
            .OnComplete(() =>
            {
                ApplyDamageAtTarget();
                if (destroyOnFall)
                {
                    Destroy(gameObject);
                }
            });
    }

    private void ApplyDamageAtTarget()
    {
        // Check all colliders overlapping the target position
        Collider2D[] collidersAtTarget = Physics2D.OverlapPointAll(transform.position);

        foreach (Collider2D col in collidersAtTarget)
        {
            if (col.CompareTag("Player"))
            {
                HealthSystem healthSystem = col.GetComponent<HealthSystem>();
                if (healthSystem != null)
                {
                    healthSystem.Damage(damage);
                }

                PlayerController2D playerController = col.GetComponent<PlayerController2D>();
                if (playerController != null)
                {
                    playerController.ApplyTemporarySpeedModifier(0.5f, 1f); // Optional slow effect
                }
            }
        }
    }
}
