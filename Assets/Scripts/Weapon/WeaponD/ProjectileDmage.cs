using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileDamage : MonoBehaviour
{
    public float damage = 10f; // This can be set by SolarSystem script if needed

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            IDamagable enemy = other.GetComponent<IDamagable>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
    }
}
