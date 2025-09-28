using UnityEngine;

public class LitIonProjectile : PreSecDamage
{
    [Header("Lithium Ion Settings")]
    public float fuseTime = 1.5f;         // Time before activating AoE damage
    public Sprite armedSprite;            // Sprite to switch to when active

    private SpriteRenderer sr;
    private bool isArmed = false;

    protected new void Start()
    {
        base.Start(); // keep PreSecDamage setup (like destroy after sec)
        sr = GetComponent<SpriteRenderer>();

        // Disable damage at start
        enabled = false;

        // Start fuse countdown
        Invoke(nameof(ArmProjectile), fuseTime);
    }

    void ArmProjectile()
    {
        isArmed = true;

        // Change sprite
        if (sr != null && armedSprite != null)
        {
            sr.sprite = armedSprite;
        }

        // Enable PreSecDamage logic
        enabled = true;
    }
}
