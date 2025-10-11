using UnityEngine;

public class ActiveTimeIncrease : MonoBehaviour
{
    [Header("Active Time Boost Settings")]
    [Tooltip("Percentage increase to weapon active time (e.g., 20 = +20%)")]
    [Range(0f, 100f)]
    public float activeTimeIncreasePercent = 20f;

    private static bool isActive = false;
    private static float totalMultiplier = 1f;

    private void OnEnable()
    {
        isActive = true;
        totalMultiplier *= 1f + (activeTimeIncreasePercent / 100f);
        Debug.Log($"[ActiveTimeIncrease] Activated: +{activeTimeIncreasePercent}% active time, total x{totalMultiplier:F2}");
    }

    private void OnDisable()
    {
        totalMultiplier /= 1f + (activeTimeIncreasePercent / 100f);

        // If no boosts remain, reset
        if (Mathf.Approximately(totalMultiplier, 1f))
            isActive = false;

        Debug.Log($"[ActiveTimeIncrease] Deactivated: remaining x{totalMultiplier:F2}");
    }

    /// <summary>
    /// Returns the modified active time after applying active time boosts.
    /// </summary>
    public static float GetModifiedActiveTime(float baseActiveTime)
    {
        if (!isActive)
            return baseActiveTime;

        return baseActiveTime * totalMultiplier;
    }

    /// <summary>
    /// Returns true if at least one ActiveTimeIncrease item exists.
    /// </summary>
    public static bool IsActive() => isActive;
}
