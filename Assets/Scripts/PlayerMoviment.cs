using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    // Informações de status do player
    [SerializeField]
    private int _vida = 100;
    private int _ataque = 10;
    private int _defesa = 1;
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
    private bool wasRunning = false;
    public KeyCode chargeKey = KeyCode.C;
    private bool isCharging = false;
    private float chargeStartTime = 0f;
    public float maxChargeTime = 0.5f;
    private bool readyToAttack = false;
    public KeyCode jumpAttackKey = KeyCode.Space;
    private bool isJumpAttacking = false;
    private int jumpComboStep = 0;
    private float jumpComboMaxDelay = 0.4f;
    private float lastJumpAttackTime = 0f;

    // som
    private AudioSource _audioSource;

    // ataque
    private BoxCollider2D _hitbox;

    // ataque carregado
    private bool _charge = false;
    private int _chargeAttack = 20;

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
    public float knockbackForce = 10f;
    public float knockbackDuration = 0.3f;
    private bool isKnockbacked = false;
    private bool isAttacking = false;

    // Block
    private bool isBlocking = false;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
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
        else if (isKnockbacked)
        {
            return;
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
        if(isAttacking)
        {
            movement.x = 0;
        }
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
        if (isAttacking)
        {
            animator.SetFloat("Speed", 0);
        }
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
            isAttacking = false;
        }

        if (isGrounded && Input.GetKeyDown(attackKey))
        {
            isAttacking = true;
            _audioSource.Play(); // Toca o som do ataque
            lastClickTime = Time.time;
            HandleCombo();
        }
    }
    void UpdateChargeAttack()
    {
        if (Input.GetKeyDown(chargeKey) && movement.x == 0)
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
                _audioSource.Play(); // Toca o som do ataque
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
            _audioSource.Play(); // Toca o som do ataque
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

            animator.SetTrigger("JumpAttack");

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
        // atualiza flag de bloqueio
        if (Input.GetKey(blockKey))
        {
            isBlocking = true;
            animator.SetBool("Block", true);
            movement.x = 0;
        }
        else
        {
            isBlocking = false;
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
        if (comboStep > 3)
        {
            comboStep = 0;
            isAttacking = false;
        }

        Debug.Log("ComboStep: " + comboStep);
        animator.SetInteger("ComboStep", comboStep);
    }

    public void ResetCombo()
    {
        comboStep = 0;
        animator.SetInteger("ComboStep", 0);
        // isAttacking = false;
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
        this.FinalizaAtaque();
        _charge = false;
    }
    public void EndJumpAttack()
    {
        isJumpAttacking = false;
        jumpComboStep = 0;
        animator.SetBool("IsJumpAttacking", false);
        animator.SetInteger("JumpComboStep", 0);
    }

    public void TakeDamage(Vector2 damageSourcePosition, int damage)
    {
        if (isKnockbacked || isDead) return;

        // se estiver bloqueando, executa animação de bloqueio com feedback e não aplica dano
        if (isBlocking)
        {
            animator.SetTrigger("BlockHit"); // animação de bloqueio especial
            return;
        }

        isKnockbacked = true;
        animator.SetBool("IsHurt", true);

        // calcula direção do knockback
        float direction = transform.position.x > damageSourcePosition.x ? 1f : -1f;
        rb.linearVelocity = new Vector2(direction * knockbackForce, rb.linearVelocity.y + 2f);

        // aplica dano
        damage -= _defesa;
        _vida -= damage;
        if (_vida <= 0)
        {
            Die();
        }

        StartCoroutine(EndKnockbackAfterDelay());
    }
    IEnumerator EndKnockbackAfterDelay()
    {
        yield return new WaitForSeconds(knockbackDuration);

        animator.SetBool("IsHurt", false);
        isKnockbacked = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Se não estivermos com o hitbox de ataque ligado, sai
        if (_hitbox == null || !_hitbox.enabled) return;
        
        if (other.CompareTag("Enemy"))
        {
            var enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(transform.position, _charge ? _chargeAttack : _ataque);
                Debug.Log("Dano causado" + (_charge ? _chargeAttack : _ataque) + " para o inimigo!");
            }
        }
    }

    public void IniciaAtaque()
    {
        _hitbox = GetComponent<BoxCollider2D>();
        _hitbox.enabled = true;
    }
    public void FinalizaAtaque()
    {
        _hitbox.enabled = false;
    }

    public void iniciaChargeAttack()
    {
        this.IniciaAtaque();
        _charge = true;
    }

}
