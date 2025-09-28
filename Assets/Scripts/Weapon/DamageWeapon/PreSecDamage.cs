using System.Collections.Generic;
using UnityEngine;

public class PreSecDamage: MonoBehaviour
{
    public float damagePerSecond = 10f; // Damage dealt per second
    public float deathAfterSec = 3f;

    private readonly List<IDamagable> enemiesInRange = new List<IDamagable>();

    protected void Start()
    {
        Destroy(gameObject,deathAfterSec);
    }

    void Update()
    {
        // Apply DPS every frame
        for (int i = 0; i < enemiesInRange.Count; i++)
        {
            if (enemiesInRange[i] != null)
            {
                enemiesInRange[i].TakeDamage(damagePerSecond * Time.deltaTime);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            IDamagable enemy = other.GetComponent<IDamagable>();
            if (enemy != null && !enemiesInRange.Contains(enemy))
            {
                enemiesInRange.Add(enemy);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            IDamagable enemy = other.GetComponent<IDamagable>();
            if (enemy != null && enemiesInRange.Contains(enemy))
            {
                enemiesInRange.Remove(enemy);
            }
        }
    }
}
