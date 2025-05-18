using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    // health bar
    [SerializeField] private Slider BarraDeVida;  
    [SerializeField] private int vidaMaxima = 100; 
    private int _vida;

    public float moveSpeed = 4f;
    public float jumpForce = 10f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 movement;
    public KeyCode attackKey = KeyCode.Space;
    public KeyCode jumpKey = KeyCode.W;
    public KeyCode blockKey = KeyCode.LeftControl;
    private bool isDead = false;
    private int comboStep = 0;
    private float lastClickTime = 0f;
    private float comboMaxDelay = 0.4f;
    private bool isGrounded = false;
    private bool wasRunning = false; // 👈 novo: pra saber se estava correndo antes
    public KeyCode chargeKey = KeyCode.C; // ou use o mesmo botão de ataque
    private bool isCharging = false;
    private float chargeStartTime = 0f;
    public float maxChargeTime = 0.5f; // tempo máximo para carga total
    private bool readyToAttack = false;
    public KeyCode jumpAttackKey = KeyCode.Space;
    private bool isJumpAttacking = false;
    private int jumpComboStep = 0;
    private float jumpComboMaxDelay = 0.4f;
    private float lastJumpAttackTime = 0f;

    // Airdash
    public float airDashSpeed = 10f;
    public float airDashDuration = 0.2f;
    public KeyCode airDashKey = KeyCode.LeftShift;

    private bool isAirDashing = false;
    private bool hasAirDashed = false;
    private float airDashTimer = 0f;

    // Dash terrestre
    public float dashSpeed = 12f;
    public float dashDuration = 0.2f;
    public KeyCode dashKey = KeyCode.LeftShift;

    private bool isDashing = false;
    private float dashTimer = 0f;

    // Knockback
    public float knockbackForce = 5f;
    public float knockbackDuration = 0.3f;
    private bool isKnockbacked = false;

    // health bar

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        _vida = vidaMaxima;
        AtualizarHUD();
    }

    void Update()
    {
        if (isDead) return;
        if (isKnockbacked) return;

        UpdateGroundDetection();
        UpdateMovementInput();
        UpdateFacingDirection();
        UpdateAnimationParameters();
        UpdateStopTrigger();

        UpdateAttackCombo();
        UpdateChargeAttack();
        UpdateJumpAttack();
        UpdateJump();
        UpdateBlock();
        UpdateAirDash();
        UpdateDash();
    }
    void FixedUpdate()
    {
        if (isAirDashing)
        {
            float dashDirection = transform.localScale.x > 0 ? 1f : -1f;
            rb.linearVelocity = new Vector2(dashDirection * airDashSpeed, 0f);
        }
        else if (isDashing)
        {
            float dashDirection = transform.localScale.x > 0 ? 1f : -1f;
            rb.linearVelocity = new Vector2(dashDirection * dashSpeed, rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = new Vector2(movement.x * moveSpeed, rb.linearVelocity.y);
        }
    }
    void UpdateGroundDetection()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        animator.SetBool("IsGrounded", isGrounded);
        if (isGrounded)
        {
            hasAirDashed = false;
        }
    }
    void UpdateMovementInput()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        movement.x = inputX;
    }
    void UpdateFacingDirection()
    {
        if (Mathf.Abs(movement.x) > 0.01f)
        {
            transform.localScale = new Vector3(Mathf.Sign(movement.x), 1, 1);
        }
    }
    void UpdateAnimationParameters()
    {
        animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
    }
    void UpdateStopTrigger()
    {
        bool isRunning = Mathf.Abs(rb.linearVelocity.x) > 0.1f;
        bool isTryingToMove = Mathf.Abs(movement.x) > 0.1f;

        if (wasRunning && !isRunning && !isTryingToMove && isGrounded)
        {
            animator.SetTrigger("Stop");
        }

        wasRunning = isRunning;
    }
    void UpdateAttackCombo()
    {
        float timeSinceLastClick = Time.time - lastClickTime;

        if (timeSinceLastClick > comboMaxDelay)
        {
            comboStep = 0;
            animator.SetInteger("ComboStep", 0);
        }

        if (isGrounded && Input.GetKeyDown(attackKey))
        {
            lastClickTime = Time.time;
            HandleCombo();
        }
    }
    void UpdateChargeAttack()
    {
        if (Input.GetKeyDown(chargeKey))
        {
            isCharging = true;
            chargeStartTime = Time.time;
            animator.SetBool("Charging", true);
        }

        if (isCharging && Input.GetKeyUp(chargeKey))
        {
            isCharging = false;
            animator.SetBool("Charging", false);

            float chargeDuration = Time.time - chargeStartTime;
            if (chargeDuration >= maxChargeTime)
            {
                readyToAttack = true;
                animator.SetBool("ReadyToAttack", true);
                animator.SetTrigger("ChargedAttack");
            }
        }
    }
    void UpdateJumpAttack()
    {
        if (!isGrounded && Input.GetKeyDown(jumpAttackKey))
        {
            float timeSinceLast = Time.time - lastJumpAttackTime;

            if (timeSinceLast > jumpComboMaxDelay)
                jumpComboStep = 1;
            else
            {
                jumpComboStep++;
                if (jumpComboStep > 3) jumpComboStep = 1;
            }

            lastJumpAttackTime = Time.time;
            isJumpAttacking = true;

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);

            animator.SetBool("IsJumpAttacking", true);
            animator.SetInteger("JumpComboStep", jumpComboStep);

            Debug.Log("JumpComboStep: " + jumpComboStep);

            animator.SetTrigger("JumpAttack"); // 🟢 volta o trigger aqui

        }
    }
    void UpdateJump()
    {
        if (Input.GetKeyDown(jumpKey) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

            // define tipo de pulo
            int jumpType = Mathf.Abs(movement.x) > 0.1f ? 1 : 0;
            animator.SetInteger("JumpType", jumpType);
            animator.SetTrigger("Jump");
        }
    }
    void UpdateBlock()
    {
        if (Input.GetKey(blockKey))
        {
            animator.SetBool("Block", true);
            movement.x = 0;
        }
        else
        {
            animator.SetBool("Block", false);
        }
    }
    void UpdateAirDash()
    {
        if (!isGrounded && !hasAirDashed && Input.GetKeyDown(airDashKey))
        {
            isAirDashing = true;
            hasAirDashed = true;
            airDashTimer = airDashDuration;

            // animação ou efeito opcional
            animator.SetTrigger("AirDash");
        }

        if (isAirDashing)
        {
            airDashTimer -= Time.deltaTime;
            if (airDashTimer <= 0f)
            {
                isAirDashing = false;
            }
        }
    }
    void UpdateDash()
    {
        if (isGrounded && !isDashing && Input.GetKeyDown(dashKey))
        {
            isDashing = true;
            animator.SetBool("IsDashing", true);
            dashTimer = dashDuration;

            animator.SetTrigger("Dash"); // animação opcional
        }

        if (isDashing)
        {
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0f)
            {
                isDashing = false;
                animator.SetBool("IsDashing", false);
            }
        }
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

    public void EndChargedAttack()
    {
        readyToAttack = false;
        animator.SetBool("ReadyToAttack", false);
    }
    public void EndJumpAttack()
    {
        isJumpAttacking = false;
        jumpComboStep = 0;
        animator.SetBool("IsJumpAttacking", false);
        animator.SetInteger("JumpComboStep", 0);
    }

    public void TakeDamage(Vector2 damageSourcePosition)
    {   
        _vida -= damage;
        AtualizarHUD();
        if (isKnockbacked || isDead) return;

        isKnockbacked = true;
        animator.SetBool("IsHurt", true);

        // calcula direção do knockback (empurra do lado contrário do dano)
        float direction = transform.position.x > damageSourcePosition.x ? 1f : -1f;
        rb.linearVelocity = new Vector2(direction * knockbackForce, rb.linearVelocity.y + 2f); // também joga pra cima levemente

        // desabilita controle por tempo curto
        StartCoroutine(EndKnockbackAfterDelay());
    }
    IEnumerator EndKnockbackAfterDelay()
    {
        yield return new WaitForSeconds(knockbackDuration);

        animator.SetBool("IsHurt", false);
        isKnockbacked = false;
    }

    // hud
    private void AtualizarHUD()
    {
        if (barraDeVida != null)
            barraDeVida.value = (float)_vida / vidaMaxima;
    }

}
