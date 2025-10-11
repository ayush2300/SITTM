using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LitmusPaper2D : MonoBehaviour
{
    [Header("Health and Color Stages")]
    private int maxHealth;
    private int litmusHealth;

    [Tooltip("Colors representing alkaline, neutral, and acidic states.")]
    public Color alkalineColor = Color.blue;
    public Color neutralColor = Color.magenta;
    public Color acidicColor = Color.red;

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
        UpdateColor();

        currentDamagePerSecond = baseDamagePerSecond;
    }

    private void UpdateColor()
    {
        float healthRatio = (float)litmusHealth / maxHealth;

        if (healthRatio > 0.66f)
            spriteRenderer.color = alkalineColor;
        else if (healthRatio > 0.33f)
            spriteRenderer.color = neutralColor;
        else
            spriteRenderer.color = acidicColor;
    }

    public void TakeDamage()
    {
        //if (currentHealth <= 0) return;

        //currentHealth -= damage;
        //currentHealth = Mathf.Max(0, currentHealth);

        UpdateColor();

    }

    private void Update()
    {
        maxHealth = gameObject.GetComponent<HealthSystem>().MaxHealth;
        litmusHealth = gameObject.GetComponent<HealthSystem>().CurrentHealth;
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
            // Reset damage timer and damage per second if player leaves
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

            playerDamageTimer = 0f;  // Start damaging immediately
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

            playerDamageTimer = 0f;  // Start damaging immediately
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
