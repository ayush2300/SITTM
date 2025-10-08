using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(WeaponManager))]
public class PlayerController2D : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rollForce = 8f;
    [SerializeField] private float rollCooldown = 1f;

    private Rigidbody2D rb;
    private Vector2 inputDirection;
    private float lastRollTime;
    private bool isRolling;

    [SerializeField] private KeyCode rollKeyCode = KeyCode.Space;

    private Animator animator;

    private float speedModifier = 1f;
    private Coroutine speedCoroutine;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        HandleInput();
        if (!isRolling)
            AnimateMovement();

        if (Input.GetKeyDown(rollKeyCode) && Time.time > lastRollTime + rollCooldown)
            StartCoroutine(PerformRoll());
    }

    private void FixedUpdate()
    {
        if (!isRolling)
            Move();
    }

    private void HandleInput()
    {
        inputDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
    }

    private void Move()
    {
        Vector2 movement = inputDirection * moveSpeed * speedModifier * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);
    }

    private void AnimateMovement()
    {
        if (animator != null)
            animator.SetBool("Run", inputDirection != Vector2.zero);
    }

    private IEnumerator PerformRoll()
    {
        isRolling = true;
        lastRollTime = Time.time;
        float timer = 0.2f;
        Vector2 rollDir = inputDirection;
        while (timer > 0f)
        {
            rb.MovePosition(rb.position + rollDir * rollForce * Time.fixedDeltaTime);
            timer -= Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        isRolling = false;
    }

    public void ApplyTemporarySpeedModifier(float modifier, float duration)
    {
        if (speedCoroutine != null)
            StopCoroutine(speedCoroutine);

        speedCoroutine = StartCoroutine(TemporarySpeedModifierRoutine(modifier, duration));
    }

    private IEnumerator TemporarySpeedModifierRoutine(float modifier, float duration)
    {
        speedModifier = modifier;
        yield return new WaitForSeconds(duration);
        speedModifier = 1f;
        speedCoroutine = null;
    }
}