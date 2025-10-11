using UnityEngine;

public class HealthRegenItem : MonoBehaviour
{
    [Header("Regen Settings")]
    [Tooltip("Percentage of max health restored per second (e.g., 0.02 = 2%)")]
    [Range(0f, 1f)] public float regenPercentPerSecond = 0.02f;

    private static bool isActive = false;
    private static float totalRegenPercent = 0f;

    private void OnEnable()
    {
        isActive = true;
        totalRegenPercent += regenPercentPerSecond;
        Debug.Log($"[HealthRegenItem] Activated: +{regenPercentPerSecond * 100f}% regen per second");
    }

    private void OnDisable()
    {
        totalRegenPercent -= regenPercentPerSecond;
        if (totalRegenPercent <= 0f)
        {
            totalRegenPercent = 0f;
            isActive = false;
        }

        Debug.Log($"[HealthRegenItem] Deactivated: remaining +{totalRegenPercent * 100f}% regen per second");
    }

    /// <summary>
    /// Returns true if any HealthRegenItem is active in the scene.
    /// </summary>
    public static bool IsActive() => isActive;

    /// <summary>
    /// Call this every frame or via coroutine from PlayerStats to apply regen.
    /// </summary>
    public static float GetRegenAmountPerSecond(float maxHealth)
    {
        if (!isActive || totalRegenPercent <= 0f)
            return 0f;

        return maxHealth * totalRegenPercent;
    }
}
