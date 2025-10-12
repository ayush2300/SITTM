using UnityEngine;
using System.Collections.Generic;

public class LitmusPaper2D : MonoBehaviour
{
    [Header("Health and Sprite Stages")]
    private int maxHealth;
    private int litmusHealth;

    [Tooltip("Sprites representing alkaline, neutral, and acidic states.")]
    public Sprite alkalineSprite;
    public Sprite neutralSprite;
    public Sprite acidicSprite;

    private SpriteRenderer spriteRenderer;

    [Header("Damage to Player")]
    public float baseDamagePerSecond = 5f;
    public float damageIncreaseRate = 1f;   // Damage increases per second while player stays in contact

    private bool playerInContact = false;
    private float playerDamageTimer = 0f;
    private float currentDamagePerSecond;

    private GameObject player;
    private HealthSystem playerHealth;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateSprite();
        currentDamagePerSecond = baseDamagePerSecond;
    }

    private void UpdateSprite()
    {
        if (maxHealth <= 0) return;

        float healthRatio = (float)litmusHealth / maxHealth;

        if (healthRatio > 0.66f && alkalineSprite != null)
            spriteRenderer.sprite = alkalineSprite;
        else if (healthRatio > 0.33f && neutralSprite != null)
            spriteRenderer.sprite = neutralSprite;
        else if (acidicSprite != null)
            spriteRenderer.sprite = acidicSprite;
    }

    public void TakeDamage()
    {
        UpdateSprite();
    }

    private void Update()
    {
        var hs = gameObject.GetComponent<HealthSystem>();
        maxHealth = hs.MaxHealth;
        litmusHealth = hs.CurrentHealth;

        TakeDamage();

        if (playerInContact)
        {
            playerDamageTimer -= Time.deltaTime;

            if (playerDamageTimer <= 0f && playerHealth != null)
            {
                playerHealth.Damage(Mathf.RoundToInt(currentDamagePerSecond));
                playerDamageTimer = 1f;
                currentDamagePerSecond += damageIncreaseRate;
            }
        }
        else
        {
            playerDamageTimer = 0f;
            currentDamagePerSecond = baseDamagePerSecond;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInContact = true;
            player = other.gameObject;
            playerHealth = player.GetComponent<HealthSystem>();

            playerDamageTimer = 0f;
            currentDamagePerSecond = baseDamagePerSecond;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInContact = false;
            player = null;
            playerHealth = null;
            currentDamagePerSecond = baseDamagePerSecond;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerInContact = true;
            player = other.gameObject;
            playerHealth = player.GetComponent<HealthSystem>();

            playerDamageTimer = 0f;
            currentDamagePerSecond = baseDamagePerSecond;
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerInContact = false;
            player = null;
            playerHealth = null;
            currentDamagePerSecond = baseDamagePerSecond;
        }
    }
}
