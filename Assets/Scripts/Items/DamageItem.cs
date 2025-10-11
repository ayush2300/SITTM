using UnityEngine;

public class DamageItem : MonoBehaviour
{
    [Header("Damage Boost Settings")]
    [Tooltip("Percent boost to total weapon damage. Example: 20 = +20% damage")]
    [Range(0, 500)]
    public float percentBonus = 20f;

    // Static trackers
    private static bool isActive = false;
    private static float totalPercentBonus = 0f; // total accumulated percentage

    private void OnEnable()
    {
        isActive = true;
        totalPercentBonus += percentBonus;
        Debug.Log($"[DamageItem] Activated: +{percentBonus}% (total {totalPercentBonus}%)");
    }

    private void OnDisable()
    {
        totalPercentBonus -= percentBonus;
        if (totalPercentBonus <= 0f)
        {
            totalPercentBonus = 0f;
            isActive = false;
        }

        Debug.Log($"[DamageItem] Deactivated: remaining total {totalPercentBonus}%");
    }

    /// <summary>
    /// Returns the adjusted damage depending on whether DamageItem exists.
    /// </summary>
    public static int GetModifiedDamage(int baseDamage)
    {
        if (!isActive || totalPercentBonus <= 0f)
            return baseDamage; // No item active — use base damage

        // Convert percent to multiplier
        float multiplier = 1f + (totalPercentBonus / 100f);
        float finalDamage = baseDamage * multiplier;

        return Mathf.RoundToInt(finalDamage);
    }

    /// <summary>
    /// Returns true if at least one DamageItem exists in the scene.
    /// </summary>
    public static bool IsActive() => isActive;
}
