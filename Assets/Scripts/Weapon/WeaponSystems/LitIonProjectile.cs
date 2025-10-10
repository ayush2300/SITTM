using System.Collections;
using UnityEngine;

public class LitIonProjectile : BlastDamage
{
    [Header("Lithium Ion Projectile Settings")]
    public float fuseTime = 2f; // Time before it explodes
    public Sprite armedSprite;  // Sprite to show when armed

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private bool hasExploded = false;

    protected void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        // Start the fuse countdown
        StartCoroutine(FuseTimer());
    }

    private IEnumerator FuseTimer()
    {
        // Wait fuse time
        yield return new WaitForSeconds(fuseTime);

        // Stop movement completely
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.isKinematic = true; // freezes it so it won't move anymore
        }

        // Change to armed sprite if provided
        if (armedSprite != null && spriteRenderer != null)
        {
            spriteRenderer.sprite = armedSprite;
        }

        // Small delay to show armed state (optional)
        yield return new WaitForSeconds(0.2f);

        // Trigger blast
        Explode();
    }

    private void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;

        ApplyBlastDamage(); // Call the inherited blast logic
        Destroy(gameObject, lifeTime); // Cleanup after
    }
}
