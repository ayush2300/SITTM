using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileDamage : MonoBehaviour
{
    public int damage = 10; // This can be set by SolarSystem script if needed
    public bool destroyOnCollision;
    public float lifeTime;
    private void Start()
    {
        if( destroyOnCollision )
            Destroy(gameObject,lifeTime);
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            HealthSystem enemy = other.gameObject.GetComponent<HealthSystem>();
            if (enemy != null)
            {
                enemy.Damage(damage);
            }
            if (destroyOnCollision)
                Destroy(gameObject);
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
            if (destroyOnCollision)
                Destroy(gameObject);
        }
    }
}
