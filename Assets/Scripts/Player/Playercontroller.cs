using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // ✅ Required for Slider

public class PlayerController : MonoBehaviour, IDamagable
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    [Header("Health Settings")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("UI")]
    public Slider healthSlider; // ✅ Assign in Inspector

    [Header("Components")]
    public Rigidbody2D rb;
    public SpriteRenderer spriteRenderer;

    private Vector2 moveInput;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI(); // ✅ Initialize slider
    }

    void Update()
    {
        // Get movement input
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput.Normalize();

        // Flip the sprite based on X movement
        if (moveInput.x > 0)
            spriteRenderer.flipX = false;
        else if (moveInput.x < 0)
            spriteRenderer.flipX = true;
    }

    void FixedUpdate()
    {
        // Apply movement
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }

    // ✅ IDamagable implementation
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // ✅ Prevent negative values

        Debug.Log($"Player took {damage} damage. Current Health: {currentHealth}");

        UpdateHealthUI(); // ✅ Update slider

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHealthUI()
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth / maxHealth; // ✅ Normalize value (0 to 1)
        }
    }

    void Die()
    {
        Debug.Log("Player Died!");
        // Add death logic (disable controls, show UI, restart, etc.)
    }
}
