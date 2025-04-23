using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 movement;
    public KeyCode attackKey = KeyCode.Space; // Tecla de ataque
    private bool isDead = false;
    private int comboStep = 0;
    private float lastClickTime = 0f;
    private float comboMaxDelay = 0.8f; // tempo pra apertar o próximo ataque


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (isDead) return;

        // ataque
        if (Input.GetKeyDown(attackKey))
        {
            HandleCombo();
        }

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

    void HandleCombo()
    {
        float timeSinceLastClick = Time.time - lastClickTime;
        lastClickTime = Time.time;

        if (timeSinceLastClick > comboMaxDelay)
        {
            comboStep = 0;
        }

        comboStep++;

        if (comboStep > 3)
        {
            comboStep = 0;
        }

        animator.SetInteger("ComboStep", comboStep);
        animator.SetTrigger("Attack");
    }
    
    public void ResetCombo()
    {
        comboStep = 0;
    }

}
