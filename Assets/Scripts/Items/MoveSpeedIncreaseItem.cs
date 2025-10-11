using UnityEngine;

public class MoveSpeedIncreaseItem : MonoBehaviour
{
    public static bool Exists => instance != null;
    private static MoveSpeedIncreaseItem instance;

    [Header("Speed Increase % (e.g., 0.25 = +25%)")]
    public float speedIncreasePercent = 0.25f;

    private void Awake()
    {
        instance = this;
    }

    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }

    public static float GetModifiedSpeed(float baseSpeed)
    {
        if (instance == null)
            return baseSpeed;

        float modified = baseSpeed * (1f + instance.speedIncreasePercent);
        return modified;
    }
}
