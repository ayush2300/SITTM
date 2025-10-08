using System.Collections.Generic;
using UnityEngine;

public class PreSecDamage : MonoBehaviour
{
    public int damagePerSecond = 10; // Damage dealt per second
    public float deathAfterSec = 3f;

    private readonly List<HealthSystem> enemiesInRange = new List<HealthSystem>();
    private Dictionary<HealthSystem, float> damageBuffer = new Dictionary<HealthSystem, float>();

    void Start()
    {
        Destroy(gameObject, deathAfterSec);
    }

    void Update()
    {
        float damageThisFrame = damagePerSecond * Time.deltaTime;

        for (int i = enemiesInRange.Count - 1; i >= 0; i--)
        {
            HealthSystem enemy = enemiesInRange[i];
            if (enemy == null)
            {
                enemiesInRange.RemoveAt(i);
                continue;
            }

            // Accumulate fractional damage
            if (!damageBuffer.ContainsKey(enemy))
                damageBuffer[enemy] = 0f;

            damageBuffer[enemy] += damageThisFrame;

            // Apply only whole number damage
            int intDamage = Mathf.FloorToInt(damageBuffer[enemy]);
            if (intDamage > 0)
            {
                enemy.Damage(intDamage);
                damageBuffer[enemy] -= intDamage;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            HealthSystem enemy = other.GetComponent<HealthSystem>();
            if (enemy != null && !enemiesInRange.Contains(enemy))
            {
                enemiesInRange.Add(enemy);
                damageBuffer[enemy] = 0f;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            HealthSystem enemy = other.GetComponent<HealthSystem>();
            if (enemy != null && enemiesInRange.Contains(enemy))
            {
                enemiesInRange.Remove(enemy);
                damageBuffer.Remove(enemy);
            }
        }
    }
}
