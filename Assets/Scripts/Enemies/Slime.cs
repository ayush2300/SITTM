using UnityEngine;

public class Slime2D : MonoBehaviour
{
    public float slowAmount = 0.5f;           // Reduce speed by 50%
    public float slowDuration = 2f;           // Slowness lasts 2 seconds
    public int damageOnCollision = 0;         // Optional damage on hit

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            var playerController = collision.gameObject.GetComponent<PlayerController2D>();

            if (playerController != null)
            {
                float modifier = 1f - slowAmount;
                playerController.ApplyTemporarySpeedModifier(modifier, slowDuration);
            }
        }
    }

}
