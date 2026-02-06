using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    private Vector2 movementInput;
    private bool isGrounded;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) { return; }

        inputReader.MoveEvent += HandleMove;
        inputReader.JumpEvent += HandleJump;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) { return; }

        inputReader.MoveEvent -= HandleMove;
        inputReader.JumpEvent -= HandleJump;
    }

    private void Update()
    {
        if (!IsOwner) { return; }

        // เช็คว่าติดพื้นไหม
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );

        // หันหน้าตามทิศที่เดิน
        if (movementInput.x != 0)
        {
            bodyTransform.localScale = new Vector3(
                Mathf.Sign(movementInput.x),
                1f,
                1f
            );
        }
    }

    private void FixedUpdate()
    {
        if (!IsOwner) { return; }

        rb.velocity = new Vector2(
            movementInput.x * movementSpeed,
            rb.velocity.y
        );
    }

    private void HandleMove(Vector2 input)
    {
        // ใช้แค่แกน X (A / D)
        movementInput = new Vector2(input.x, 0f);
    }

    private void HandleJump()
    {
        if (!isGrounded) { return; }

        rb.velocity = new Vector2(rb.velocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) { return; }

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
#endif
}

