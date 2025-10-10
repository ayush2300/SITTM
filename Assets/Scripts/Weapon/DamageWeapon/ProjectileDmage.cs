using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileDamage : MonoBehaviour
{
    public int damage = 10; // This can be set by SolarSystem script if needed

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            HealthSystem enemy = other.gameObject.GetComponent<HealthSystem>();
            if (enemy != null)
            {
                enemy.Damage(damage);
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            HealthSystem enemy = other.gameObject.GetComponent<HealthSystem>();
            if (enemy != null)
            {
                enemy.Damage(damage);
            }
        }
    }
}
