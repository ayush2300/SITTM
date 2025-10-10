using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerController2D : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 moveInput;
    private float lastHorizontalDir = 1f; // 1 = right, -1 = left

    public float speedModifier;
    private Coroutine speedCoroutine;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Read movement input
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput.Normalize();

        bool isMoving = moveInput.sqrMagnitude > 0.01f;

        // Track last horizontal direction for animation facing
        if (moveInput.x > 0.1f)
            lastHorizontalDir = 1f;
        else if (moveInput.x < -0.1f)
            lastHorizontalDir = -1f;

        // Update animator
        animator.SetInteger("move", isMoving ? 1 : 0);
        animator.SetFloat("facing", lastHorizontalDir);
    }

    private void FixedUpdate()
    {
        // Move the player
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
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
