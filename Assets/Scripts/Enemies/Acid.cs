using UnityEngine;
using System.Collections;

public class AcidDamage2D : MonoBehaviour
{
    public int acidDamage = 5;
    public float lifetime = 3f;
    public float damageInterval = 0.5f; // How often damage is applied
    public GameObject particleEffectPrefab;  // Assign particle prefab in inspector
    private GameObject spawnedEffect;

    private Coroutine damageCoroutine;
    private HealthSystem playerHealth;

    private void Start()
    {
        if (particleEffectPrefab != null)
        {
            spawnedEffect = Instantiate(particleEffectPrefab, transform.position, transform.rotation);
            spawnedEffect.transform.SetParent(transform);
        }
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerHealth = other.GetComponent<HealthSystem>();
            if (playerHealth != null && damageCoroutine == null)
            {
                damageCoroutine = StartCoroutine(DealContinuousDamage());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (damageCoroutine != null)
            {
                StopCoroutine(damageCoroutine);
                damageCoroutine = null;
            }
            playerHealth = null;
        }
    }

    IEnumerator DealContinuousDamage()
    {
        while (true)
        {
            if (playerHealth != null && !playerHealth.IsDead)
            {
                playerHealth.Damage(acidDamage);
            }
            yield return new WaitForSeconds(damageInterval);
        }
    }

    private void OnDestroy()
    {
        if (spawnedEffect != null)
        {
            Destroy(spawnedEffect);
        }
        if (damageCoroutine != null)
        {
            StopCoroutine(damageCoroutine);
        }
    }
}
