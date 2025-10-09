using UnityEngine;

public class XpDrop : MonoBehaviour
{
    public int xpAmount = 10;
    public float pickupRange = 1.5f;
    public float moveSpeed = 5f;

    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= pickupRange)
        {
            // Move toward player for smooth pickup feel
            transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerXP xpSystem = other.GetComponent<PlayerXP>();
            if (xpSystem != null)
            {
                xpSystem.AddXP(xpAmount);
            }

            gameObject.SetActive(false); // or Destroy(gameObject);
        }
    }
}
