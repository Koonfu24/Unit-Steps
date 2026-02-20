using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerMove : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform bodyTransform;

    [Header("Movement Settings")]
    [SerializeField] private float movementSpeed = 6f;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.15f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Vector2 movementInput;

    private bool isGrounded;
    private bool wasGrounded;
    private bool jumpRequested;
    private bool canJump;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        inputReader.MoveEvent += HandleMove;
        inputReader.JumpEvent += HandleJump;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;

        inputReader.MoveEvent -= HandleMove;
        inputReader.JumpEvent -= HandleJump;
    }
    
    private void Update()
    {
        if (!IsOwner) return;

        if (movementInput.x != 0)
        {
            spriteRenderer.flipX = movementInput.x < 0;
        }
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;

        // ตรวจพื้น
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );

        // เดิน
        rb.velocity = new Vector2(
            movementInput.x * movementSpeed,
            rb.velocity.y
        );

        // กระโดด (กระโดดได้เมื่อแตะพื้น)
        if (jumpRequested && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        jumpRequested = false;
    }

    private void HandleMove(Vector2 input)
    {
        movementInput = new Vector2(input.x, 0f);
    }

    private void HandleJump()
    {
        jumpRequested = true;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
#endif
}