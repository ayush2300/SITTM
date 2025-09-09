using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour, IDamagable
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    [Header("Health Settings")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("UI")]
    public Slider healthSlider;

    [Header("Components")]
    public Rigidbody2D rb;
    public Animator animator;

    private Vector2 moveInput;
    private float lastHorizontalDir = 1f; // 1 = right, -1 = left

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    void Update()
    {
        // Movement Input
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput.Normalize();

        bool isMoving = moveInput.sqrMagnitude > 0.01f;

        // Track last horizontal direction if there is X movement
        if (moveInput.x > 0.1f)
            lastHorizontalDir = 1f;
        else if (moveInput.x < -0.1f)
            lastHorizontalDir = -1f;

        // Reset animator bools
        animator.SetBool("IdleRight", false);
        animator.SetBool("IdleLeft", false);
        animator.SetBool("RunRight", false);
        animator.SetBool("RunLeft", false);

        // Logic for animations
        if (!isMoving)
        {
            animator.SetInteger("move", 0); // Idle
            
        }
        else
        {
            if (moveInput.x > 0.1f) // Moving right
                animator.SetBool("isright", true);

            else if (moveInput.x < -0.1f) // Moving left
                animator.SetBool("isright", false);

            animator.SetInteger("move", 1); // Running

        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        Debug.Log($"Player took {damage} damage. Current Health: {currentHealth}");
        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHealthUI()
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth / maxHealth;
        }
    }

    void Die()
    {
        Debug.Log("Player Died!");
    }
}
