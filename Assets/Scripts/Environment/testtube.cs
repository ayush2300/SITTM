using UnityEngine;
using DG.Tweening;
using System.Collections;

public class InteractiveTiltingItem : MonoBehaviour
{
    [Header("Prefab Settings")]
    public GameObject prefabToSpawn;

    [Header("Chance Settings")]
    [Tooltip("Initial tilt chance (0 to 1, e.g. 0.2 for 20%)")]
    public float tiltChance = 0.2f;
    [Tooltip("Chance multiplier added on each failed tilt attempt (e.g. 0.05 for 5%)")]
    public float chanceMultiplier = 0.05f;

    [Header("Detection Settings")]
    public float detectionRadius = 1.5f;

    [Header("Tilt & Animation")]
    public Transform tiltTarget;
    public float tiltDuration = 0.7f;
    public Vector3 targetRotationEuler = new Vector3(45f, 45f, 0f);

    [Header("Damage Settings")]
    public int damage = 10;

    [Header("Destroy")]
    public bool destroyOnTilt = true;

    [Header("Reset Settings")]
    [Tooltip("Time in seconds to wait after rotating back before allowing tilt again")]
    public float resetInterval = 0.5f;

    [Header("Sprite Settings")]
    [Tooltip("Sprite to switch to when player enters detection radius")]
    public Sprite spriteOnPlayerEnter;

    private SpriteRenderer spriteRenderer;
    private Sprite originalSprite;

    private bool hasTilted = false;
    private Quaternion originalRotation;

    private bool playerInsidePrevFrame = false;

    private void Awake()
    {
        originalRotation = transform.localRotation;

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalSprite = spriteRenderer.sprite;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        if (tiltTarget != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(transform.position, tiltTarget.position);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(tiltTarget.position, 0.3f);
        }
    }

    void Update()
    {
        if (hasTilted) return;

        bool playerInsideNow = false;
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius);
        foreach (var col in hits)
        {
            if (col.CompareTag("Player"))
            {
                playerInsideNow = true;
                break;
            }
        }

        // Change sprite on player entering detection radius (rising edge)
        if (playerInsideNow && !playerInsidePrevFrame)
        {
            if (spriteRenderer != null && spriteOnPlayerEnter != null)
            {
                spriteRenderer.sprite = spriteOnPlayerEnter;
            }
        }

        // Optionally revert sprite on player exit
        if (!playerInsideNow && playerInsidePrevFrame)
        {
            if (spriteRenderer != null && originalSprite != null)
            {
                spriteRenderer.sprite = originalSprite;
            }
        }

        // Trigger tilt on player entering detection radius (rising edge)
        if (playerInsideNow && !playerInsidePrevFrame)
        {
            if (Random.value < tiltChance)
            {
                StartTilt();
            }
            else
            {
                tiltChance += chanceMultiplier;
                tiltChance = Mathf.Clamp01(tiltChance);
            }
        }

        playerInsidePrevFrame = playerInsideNow;
    }

    void StartTilt()
    {
        hasTilted = true;
        transform.DOLocalRotate(targetRotationEuler, tiltDuration).SetEase(Ease.OutSine).OnComplete(() =>
        {
            TryHitPlayer();

            if (prefabToSpawn != null && tiltTarget != null)
            {
                Instantiate(prefabToSpawn, tiltTarget.position, Quaternion.identity);
            }

            if (destroyOnTilt)
            {
                Destroy(gameObject);
            }
            else
            {
                transform.DOLocalRotateQuaternion(originalRotation, tiltDuration).SetEase(Ease.InSine).OnComplete(() =>
                {
                    StartCoroutine(ResetTiltAfterInterval());
                });
            }
        });
    }

    IEnumerator ResetTiltAfterInterval()
    {
        yield return new WaitForSeconds(resetInterval);
        hasTilted = false;
    }

    void TryHitPlayer()
    {
        if (tiltTarget == null) return;
        Collider2D[] hits = Physics2D.OverlapCircleAll(tiltTarget.position, 0.4f);
        foreach (var col in hits)
        {
            if (col.CompareTag("Player"))
            {
                var healthSys = col.GetComponent<HealthSystem>();
                if (healthSys != null) healthSys.Damage(damage);
            }
        }
    }
}
