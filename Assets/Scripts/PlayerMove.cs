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

    private Animator anim;

    // ✅ Sync ค่าที่จำเป็น
    private NetworkVariable<bool> netIsGrounded = new NetworkVariable<bool>();
    private NetworkVariable<float> netMoveX = new NetworkVariable<float>();

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

    private void Update()
    {
        // ✅ Owner คุม input + flip
        if (IsOwner)
        {
            if (movementInput.x != 0)
            {
                bool flip = movementInput.x < 0;
                FlipServerRpc(flip);
            }
        }

        // ✅ ทุก client ใช้ค่าที่ sync
        anim.SetFloat("isWalk", Mathf.Abs(netMoveX.Value));
        anim.SetBool("IsGrounds", netIsGrounded.Value);
    }

    private void FixedUpdate()
    {
        if (IsOwner)
        {
            MoveServerRpc(movementInput, jumpRequested);
            jumpRequested = false;
        }
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
    // SERVER LOGIC
    // =========================
    [ServerRpc]
    private void MoveServerRpc(Vector2 input, bool jump)
    {
        // ✅ sync ค่าเดิน
        netMoveX.Value = input.x;

        // เช็คพื้น
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );

        bool grounded = false;
        foreach (var hit in hits)
        {
            if (hit.gameObject != gameObject)
            {
                grounded = true;
                break;
            }
        }

        // ✅ sync ground
        netIsGrounded.Value = grounded;

        // เดิน
        rb.velocity = new Vector2(input.x * movementSpeed, rb.velocity.y);

        // กระโดด
        if (jump && grounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    // =========================
    // FLIP
    // =========================
    [ServerRpc]
    private void FlipServerRpc(bool flip)
    {
        FlipClientRpc(flip);
    }

    [ClientRpc]
    private void FlipClientRpc(bool flip)
    {
        spriteRenderer.flipX = flip;
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