using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 movement;
    public KeyCode attackKey = KeyCode.Space; // Tecla de ataque
    private bool isDead = false;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (isDead) return;

        // ataque
        if (!IsAttacking() && Input.GetKeyDown(attackKey))
        {
            animator.SetTrigger("Bite");
        }

        if(!IsAttacking())
        {
            movement.x = Input.GetAxisRaw("Horizontal");
            // movement.y = Input.GetAxisRaw("Vertical");

            if (movement.x > 0)
                transform.localScale = new Vector3(1, 1, 1); // olhando pra direita
            else if (movement.x < 0)
                transform.localScale = new Vector3(-1, 1, 1); // olhando pra esquerda

            animator.SetFloat("Horizontal", movement.x);
            // animator.SetFloat("Vertical", movement.y);
            animator.SetFloat("Speed", movement.sqrMagnitude);
        }
        else
        {
            // se estiver atacando, não se mover
            movement.x = 0;
            movement.y = 0;
        }
        
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    public void Die()
    {
        isDead = true;
        animator.SetBool("IsDead", true);
        rb.linearVelocity = Vector2.zero;
        // opcional: GetComponent<Collider2D>().enabled = false;
    }

    bool IsAttacking()
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName("Bite");
    }

}
