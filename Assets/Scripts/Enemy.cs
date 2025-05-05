using UnityEngine;

public class Enemy : MonoBehaviour
{
    // public float moveSpeed = 2f;
    // public float detectionRange = 5f;
    // public int damage = 1;
    // public Transform player;
    // public LayerMask playerLayer;

    // [Header("Ground Check")]
    // public Transform groundCheck;
    // public float groundCheckDistance = 0.1f;
    // public LayerMask groundLayer;

    // private Animator animator;
    // private Rigidbody2D rb;
    // private bool isFacingRight = true;
    // private bool isDead = false;
    // private bool isAttacking = false;
    // private bool isHurt = false;

    // void Start()
    // {
    //     animator = GetComponent<Animator>();
    //     rb = GetComponent<Rigidbody2D>();
    // }

    // void Update()
    // {
    //     if (isDead || isHurt) return;

    //     float distance = Vector2.Distance(transform.position, player.position);

    //     if (distance < detectionRange && !isAttacking)
    //     {
    //         MoveTowardPlayer();
    //         animator.SetFloat("Speed", moveSpeed);
    //     }
    //     else
    //     {
    //         animator.SetFloat("Speed", 0f);
    //     }
    // }

    // void MoveTowardPlayer()
    // {
    //     OrigemX = transform.position.x + _raycastOffset.x;
    //     OrigemY = transform.position.y + _raycastOffset.y;

    //     raycastParedeDireita = Physics2D.Raycast(new Vector2(OrigemX, OrigemY), Vector2.right, _raycastDistancia, _layerParede);

    //     // Direção horizontal (para onde o inimigo quer ir)
    //     float dirX = player.position.x - transform.position.x;

    //     // Ponto de origem do raycast deslocado horizontalmente conforme direção
    //     Vector2 origin = new Vector2(transform.position.x + Mathf.Sign(dirX) * 0.3f, groundCheck.position.y);

    //     // Faz o raycast para baixo a partir da frente
    //     bool noGroundAhead = !Physics2D.Raycast(origin, Vector2.down, groundCheckDistance, groundLayer);

    //     if (noGroundAhead)
    //     {
    //         moveSpeed = 0;
    //         return;
    //     }

    //     // Move apenas no eixo X
    //     Vector2 direction = new Vector2(player.position.x - transform.position.x, 0).normalized;
    //     transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;

    //     // Virar sprite
    //     if (direction.x < 0 && !isFacingRight) Flip();
    //     else if (direction.x > 0 && isFacingRight) Flip();
    // }

    // void Flip()
    // {
    //     isFacingRight = !isFacingRight;
    //     Vector3 scale = transform.localScale;
    //     scale.x *= -1;
    //     transform.localScale = scale;
    // }

    // void OnTriggerEnter2D(Collider2D other)
    // {
    //     if (isDead) return;

    //     if (other.CompareTag("Player"))
    //     {
    //         isAttacking = true;
    //         animator.SetTrigger("Attack");

    //         Vector2 hitFrom = transform.position;
    //         other.GetComponent<PlayerMovement>()?.TakeDamage(hitFrom);

    //         Invoke(nameof(ResetAttack), 0.5f);
    //     }
    // }

    // void ResetAttack()
    // {
    //     isAttacking = false;
    // }

    // public void TakeHit()
    // {
    //     if (isDead) return;

    //     isHurt = true;
    //     animator.SetBool("IsHurt", true);
    //     Invoke(nameof(RecoverFromHit), 0.4f);
    // }

    // void RecoverFromHit()
    // {
    //     isHurt = false;
    //     animator.SetBool("IsHurt", false);
    // }

    // public void Die()
    // {
    //     if (isDead) return;

    //     isDead = true;
    //     animator.SetBool("IsDead", true);

    //     GetComponent<Collider2D>().enabled = false;
    //     rb.linearVelocity = Vector2.zero;
    //     this.enabled = false;
    // }

    // void OnDrawGizmosSelected()
    // {
    //     if (groundCheck != null && player != null)
    //     {
    //         float dirX = player.position.x - transform.position.x;
    //         Vector2 origin = new Vector2(transform.position.x + Mathf.Sign(dirX) * 0.3f, groundCheck.position.y);
    //         Gizmos.color = Color.red;
    //         Gizmos.DrawLine(origin, origin + Vector2.down * groundCheckDistance);
    //     }
    // }
}
