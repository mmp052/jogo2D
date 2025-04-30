using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float jumpForce = 5f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 movement;
    public KeyCode attackKey = KeyCode.Space;
    public KeyCode jumpKey = KeyCode.W;
    public KeyCode dashKey = KeyCode.LeftShift;
    public KeyCode blockKey = KeyCode.LeftControl;
    private bool isDead = false;
    private int comboStep = 0;
    private float lastClickTime = 0f;
    private float comboMaxDelay = 0.4f;
    private bool isGrounded = false;
    private bool wasRunning = false; // 👈 novo: pra saber se estava correndo antes
    private bool wasMoving = false;  // Novo: pra saber se estava se movendo no frame anterior

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (isDead) return;

        // Detectar se está no chão
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        animator.SetBool("IsGrounded", isGrounded);

        // Captura o input de movimento
        float inputX = Input.GetAxisRaw("Horizontal");

        // Atualiza movimento
        movement.x = inputX;

        // Flipar sprite apenas se tiver movimento
        if (Mathf.Abs(inputX) > 0.01f)
        {
            transform.localScale = new Vector3(Mathf.Sign(inputX), 1, 1);
        }

        // Atualiza parâmetro de Speed (pra animações de correr e idle)
        animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));

        // Se o jogador estava se movendo rápido e parou completamente
        bool isRunning = Mathf.Abs(rb.linearVelocity.x) > 0.1f;
        bool isTryingToMove = Mathf.Abs(inputX) > 0.1f;

        // Detectar parada real: estava correndo e parou completamente
        if (wasRunning && !isRunning && !isTryingToMove && isGrounded)
        {
            animator.SetTrigger("Stop");
        }

        wasRunning = isRunning;

        // Attack
        float timeSinceLastClick = Time.time - lastClickTime;

        if (timeSinceLastClick > comboMaxDelay)
        {
            comboStep = 0;
            animator.SetInteger("ComboStep", 0);
        }

        if (Input.GetKeyDown(attackKey))
        {
            lastClickTime = Time.time;
            HandleCombo();
        }

        // Block
        animator.SetBool("Block", false);
        if (Input.GetKey(blockKey))
        {
            animator.SetBool("Block", true);
            movement.x = 0;
        }

        // Jump (só se estiver no chão)
        if (Input.GetKeyDown(jumpKey) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            animator.SetTrigger("Jump");
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(movement.x * moveSpeed, rb.linearVelocity.y);
    }

    public void Die()
    {
        isDead = true;
        animator.SetBool("IsDead", true);
        rb.linearVelocity = Vector2.zero;
    }

    void HandleCombo()
    {
        if (comboStep == 0) animator.SetTrigger("Attack");

        comboStep++;
        if (comboStep > 3) comboStep = 0;

        Debug.Log("ComboStep: " + comboStep);
        animator.SetInteger("ComboStep", comboStep);
    }

    public void ResetCombo()
    {
        comboStep = 0;
        animator.SetInteger("ComboStep", 0);
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
