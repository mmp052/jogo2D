using UnityEngine;
using System.Collections;
using System;

public class PlayerMovement : MonoBehaviour
{
    // Agora o evento carrega quem morreu
    public static event Action<PlayerMovement> OnPlayerDeath;
    // Informações de status do player
    [SerializeField] private int _vida = 100;
    private int _ataque = 10;
    private int _defesa = 1;

    // Movement parameters
    public float moveSpeed = 4f;
    public float jumpForce = 10f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;
    public LayerMask groundLayer;

    // Components
    private Rigidbody2D rb;
    private Animator animator;
    private AudioSource _audioSource;

    // Input keys
    public KeyCode attackKey = KeyCode.Space;
    public KeyCode jumpKey = KeyCode.W;
    public KeyCode blockKey = KeyCode.LeftControl;
    public KeyCode transformKey = KeyCode.T;
    public KeyCode jumpAttackKey = KeyCode.Space;
    public KeyCode airDashKey = KeyCode.LeftShift;
    public KeyCode dashKey = KeyCode.LeftShift;

    // State flags
    private bool isDead = false;
    private bool isAttacking = false;
    private bool isCharging = false;
    private bool readyToAttack = false;
    private bool isJumpAttacking = false;
    private bool isAirDashing = false;
    private bool hasAirDashed = false;
    private bool isDashing = false;
    private bool isKnockbacked = false;
    private bool isBlocking = false;
    private bool isTransformed = false;
    private bool isGrounded = false;
    private bool wasRunning = false;

    // Combo
    private int comboStep = 0;
    private float lastClickTime = 0f;
    private float comboMaxDelay = 0.4f;

    // Jump attack combo
    private float lastJumpAttackTime = 0f;
    private int jumpComboStep = 0;
    private float jumpComboMaxDelay = 0.4f;

    // Dash parameters
    public float airDashSpeed = 10f;
    public float airDashDuration = 0.2f;
    private float airDashTimer = 0f;
    public float dashSpeed = 12f;
    public float dashDuration = 0.2f;
    private float dashTimer = 0f;

    // Knockback
    public float knockbackForce = 10f;
    public float knockbackDuration = 0.3f;

    // Attack
    public KeyCode chargeKey = KeyCode.C;
    public float maxChargeTime = 0.5f;
    private float chargeStartTime = 0f;
    private bool _charge = false;
    private int _chargeAttack = 20;
    private BoxCollider2D _hitbox;
    [Tooltip("Tamanho da hitbox no estado normal")]
    public Vector2 normalHitboxSize = new Vector2(2.5f, 1f);
    [Tooltip("Tamanho da hitbox quando transformado")]
    public Vector2 transformedHitboxSize = new Vector2(5f, 1.2f);


    // Transformation
    [SerializeField]
    private bool canTransform = false;
    [Tooltip("Animator Controller da forma normal")]
    public RuntimeAnimatorController normalController;
    [Tooltip("Animator Controller da forma transformada")]
    public RuntimeAnimatorController transformedController;
    [Tooltip("Som de transformação")]
    public AudioClip transformSound;
    [Tooltip("Prefab do VFX de transformação")]
    public GameObject transformVFXPrefab;
    [Tooltip("Duração do VFX de transformação")]
    public float transformVFXDuration = 1f;

    // Internal
    private Vector2 movement;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
        _hitbox = GetComponent<BoxCollider2D>();
        _hitbox.enabled = false;              // desligada por padrão
        if (normalController != null)
            animator.runtimeAnimatorController = normalController;
    }

    void Update()
    {
        if (isDead || isKnockbacked) return;

        // só dispara transformação se tiver coletado
        if (canTransform && Input.GetKeyDown(transformKey))
            ToggleTransformation();

        UpdateGroundDetection();
        UpdateMovementInput();
        UpdateFacingDirection();
        UpdateAnimationParameters();

        UpdateAttackCombo();
        // Apenas se não transformado, habilita pulo, ataque normal e dash
        if (!isTransformed)
        {
            UpdateStopTrigger();
            UpdateChargeAttack();
            UpdateJumpAttack();
            UpdateJump();
            UpdateAirDash();
            UpdateDash();
            UpdateBlock();
        }
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

    // Lógica de transformação
    private void ToggleTransformation()
    {
        isTransformed = !isTransformed;
        animator.runtimeAnimatorController = isTransformed && transformedController != null
            ? transformedController
            : normalController;
        if (transformSound != null)
            _audioSource.PlayOneShot(transformSound);
        if (transformVFXPrefab != null)
        {
            var vfx = Instantiate(transformVFXPrefab, transform.position, Quaternion.identity);
            Destroy(vfx, transformVFXDuration);
        }
    }
    void UpdateGroundDetection()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        animator.SetBool("IsGrounded", isGrounded);
        if (isGrounded)
            hasAirDashed = false;
    }
    void UpdateMovementInput()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        movement.x = inputX;
        if (isAttacking || isCharging || Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
            movement.x = 0;
    }

    void UpdateFacingDirection()
    {
        if (Mathf.Abs(movement.x) > 0.01f)
            transform.localScale = new Vector3(Mathf.Sign(movement.x), 1, 1);
    }

    void UpdateAnimationParameters()
    {
        animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
        if (isAttacking)
            animator.SetFloat("Speed", 0);
    }
    void UpdateStopTrigger()
    {
        bool isRunning = Mathf.Abs(rb.linearVelocity.x) > 0.1f;
        bool isTryingToMove = Mathf.Abs(movement.x) > 0.1f;
        if (wasRunning && !isRunning && !isTryingToMove && isGrounded)
            animator.SetTrigger("Stop");
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
        // dispara o evento passando this
        OnPlayerDeath?.Invoke(this);
    }

    void HandleCombo()
    {
        // dispara o trigger de ataque só no primeiro golpe
        if (comboStep == 0)
            animator.SetTrigger("Attack");

        comboStep++;

        // define o número máximo de golpes segundo a forma
        int maxCombo = isTransformed ? 2 : 3;

        if (comboStep > maxCombo)
        {
            comboStep = 0;
            isAttacking = false;
        }

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
        if (isTransformed)
            damage -= _defesa * 10; // reduz defesa pela metade se transformado
        else
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
        // // Se não estivermos com o hitbox de ataque ligado, sai
        if (_hitbox == null || !_hitbox.enabled) return;

        if (other.CompareTag("Enemy"))
        {
            if (other.isTrigger) 
            return;

            var enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                if(isTransformed)
                    enemy.TakeDamage(transform.position, _ataque * 4);
                else
                    enemy.TakeDamage(transform.position, _charge ? _chargeAttack : _ataque);
            }
        }
    }

    public void IniciaAtaque()
    {
        _audioSource.Play(); // Toca o som do ataque
        // ajusta o tamanho conforme a forma
        _hitbox.size = isTransformed
            ? transformedHitboxSize
            : normalHitboxSize;

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

    public void EnableTransformation()
    {
        canTransform = true;
        // (opcional) feedback visual, som, UI, etc.
    }
    
    // reseta estado e move o player para a posição do checkpoint
    public void RespawnAt(Vector3 pos)
    {
        // reposiciona
        transform.position = pos;

        // reseta físicas
        var rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.zero;

        // reseta flags de morte / knockback
        isDead = false;
        isKnockbacked = false;
        animator.SetBool("IsDead", false);
        animator.SetBool("IsHurt", false);

        // opcional: reset combo, anim state, etc.
        ResetCombo();
    }

}
