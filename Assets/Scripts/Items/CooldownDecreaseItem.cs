using UnityEngine;

public class CooldownDecreaseItem : MonoBehaviour
{
    [Header("Cooldown Reduction Settings")]
    [Tooltip("Percentage reduction in cooldown (e.g., 20 = 20% faster)")]
    [Range(0f, 100f)]
    public float cooldownReductionPercent = 20f;

    private static bool isActive = false;
    private static float totalReductionMultiplier = 1f; // 1 means 100% (no change)

    private void OnEnable()
    {
        float reductionFactor = 1f - (cooldownReductionPercent / 100f);
        totalReductionMultiplier *= reductionFactor;
        isActive = true;

        Debug.Log($"[CooldownDecreaseItem] Activated: {cooldownReductionPercent}% reduction -> total x{totalReductionMultiplier:F2}");
    }

    private void OnDisable()
    {
        float reductionFactor = 1f - (cooldownReductionPercent / 100f);
        totalReductionMultiplier /= reductionFactor;

        if (Mathf.Approximately(totalReductionMultiplier, 1f))
            isActive = false;

        Debug.Log($"[CooldownDecreaseItem] Deactivated: total x{totalReductionMultiplier:F2}");
    }

    /// <summary>
    /// Returns the adjusted cooldown based on active CooldownDecreaseItems.
    /// </summary>
    public static float GetModifiedCooldown(float baseCooldown)
    {
        if (!isActive)
            return baseCooldown;

        return baseCooldown * totalReductionMultiplier;
    }

    public static bool IsActive() => isActive;
}
