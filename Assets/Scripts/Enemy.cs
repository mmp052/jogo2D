using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private int maxVida = 60;
    [SerializeField] private int dano = 10;
    [SerializeField] private float velocidade = 2f;
    [SerializeField] private float velocidadePatrulha = 2f;

    [Header("Attack Hitbox")]
    [SerializeField] private Vector2 hitboxSize = new Vector2(1f, 1f);
    [SerializeField] private Vector2 hitboxOffset = Vector2.zero;

    // colliders no mesmo GameObject, na ordem: [0]=body, [1]=attack hitbox
    private BoxCollider2D bodyCollider;
    private BoxCollider2D attackHitbox;
    private CapsuleCollider2D detectCollider;
    private Animator animator;

    // flags internas
    public int Vida { get; private set; }
    public bool Damaged { get; private set; }
    public Vector2 LastDamageSource { get; private set; }
    public bool FlagEnterAttack { get; private set; }

    public int Dano => dano;
    public float Velocidade => velocidade;
    public float VelocidadePatrulha => velocidadePatrulha;

    void Awake()
    {
        animator     = GetComponent<Animator>();
        Vida         = maxVida;
        var boxes    = GetComponents<BoxCollider2D>();
        bodyCollider = boxes[0];
        attackHitbox = boxes[1];
        attackHitbox.size   = hitboxSize;
        attackHitbox.offset = hitboxOffset;

        detectCollider = GetComponent<CapsuleCollider2D>();
        attackHitbox.enabled = false;
    }

    public void TakeDamage(Vector2 from, int amount)
    {
        LastDamageSource = from;
        int final = Mathf.Max(0, amount); 
        Vida -= Mathf.Max(0, final);
        if (Vida <= 0)
        {
            animator.SetTrigger("Death");
            GetComponent<StateMorte>().enabled = true;
        }
        else
        {
            Damaged = true;
            animator.SetTrigger("Hit");
            GetComponent<StateDano>().enabled = true;
        }
    }

    public void ClearDamage() => Damaged = false;

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            FlagEnterAttack = true;
    }
    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            FlagEnterAttack = false;
    }

    // chamado por Event no fim da animação de morte
    public void Die()
    {
        Destroy(gameObject);
    }
}