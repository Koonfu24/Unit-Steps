using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerMove : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Movement Settings")]
    [SerializeField] private float movementSpeed = 6f;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float groundCheckRadius = 0.15f;
    [SerializeField] private LayerMask groundLayer;

    private Vector2 movementInput;
    private bool jumpRequested;
    private bool isGrounded;

    private Animator anim;

    // =========================
    // INIT
    // =========================
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            inputReader.MoveEvent += HandleMove;
            inputReader.JumpEvent += HandleJump;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsOwner)
        {
            inputReader.MoveEvent -= HandleMove;
            inputReader.JumpEvent -= HandleJump;
        }
    }

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }

    // =========================
    // UPDATE (OWNER ONLY)
    // =========================
    private void Update()
    {
        if (!IsOwner) return;

        // Flip + sync
        if (movementInput.x != 0)
        {
            bool flip = movementInput.x < 0;
            FlipServerRpc(flip);
        }

        // Animation (NetworkAnimator จะ sync ให้เอง)
        anim.SetFloat("isWalk", Mathf.Abs(movementInput.x));
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;

        MoveServerRpc(movementInput, jumpRequested);
        jumpRequested = false;
    }

    // =========================
    // INPUT
    // =========================
    private void HandleMove(Vector2 input)
    {
        movementInput = new Vector2(input.x, 0f);
    }

    private void HandleJump()
    {
        jumpRequested = true;
    }

    // =========================
    // SERVER MOVEMENT (สำคัญสุด)
    // =========================
    [ServerRpc(RequireOwnership = false)]
    private void MoveServerRpc(Vector2 input, bool jump)
    {
        // เช็คพื้น
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );

        isGrounded = false;
        foreach (var hit in hits)
        {
            if (hit.gameObject != gameObject)
            {
                isGrounded = true;
                break;
            }
        }

        // เดิน
        rb.velocity = new Vector2(input.x * movementSpeed, rb.velocity.y);

        // กระโดด
        if (jump && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    // =========================
    // FLIP SYNC
    // =========================
    [ServerRpc(RequireOwnership = false)]
    private void FlipServerRpc(bool flip)
    {
        FlipClientRpc(flip);
    }

    [ClientRpc]
    private void FlipClientRpc(bool flip)
    {
        spriteRenderer.flipX = flip;
    }

    // =========================
    // GIZMOS
    // =========================
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
#endif
}