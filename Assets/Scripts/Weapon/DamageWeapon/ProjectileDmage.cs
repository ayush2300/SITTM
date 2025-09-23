using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileDamage : MonoBehaviour
{
    public float damage = 10f; // This can be set by SolarSystem script if needed

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            IDamagable enemy = other.gameObject.GetComponent<IDamagable>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
    }
}
