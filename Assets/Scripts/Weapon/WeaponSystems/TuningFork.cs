using System.Collections;
using UnityEngine;

public class TuningFork : MonoBehaviour
{
  
    [Header("Freeze time")]
    public float freezeTime = 1f;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<EnemyAI>().Freeze(freezeTime);
        }
    }
}
