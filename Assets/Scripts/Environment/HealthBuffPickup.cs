using UnityEngine;

public class HealthBuffPickup : MonoBehaviour
{
    [Header("Buff Settings")]
    public int healAmount = 20;                 // Amount of health restored on pickup
    public GameObject pickupEffectPrefab;      // Optional particle effect prefab

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("HealthBuffPickup: OnTriggerEnter2D with " + other.name);
        if (other.CompareTag("Player"))
        {
            HealthSystem playerHealth = other.GetComponent<HealthSystem>();

            if (playerHealth != null && !playerHealth.IsDead)
            {
                // Heal only if health less than max, otherwise no change
                if (playerHealth.CurrentHealth < playerHealth.MaxHealth)
                {
                    playerHealth.Heal(healAmount);
                }

                // Show pickup effect always
                if (pickupEffectPrefab != null)
                {
                    GameObject effectInstance = Instantiate(pickupEffectPrefab, other.transform.position, Quaternion.identity);
                    effectInstance.transform.SetParent(other.transform);

                    Destroy(effectInstance, 1f); // Destroy effect after 2 seconds
                }

                // Destroy pickup regardless of health state
                Destroy(gameObject);
            }
        }
    }

}
