using UnityEngine;
using FSM;

[RequireComponent(typeof(Animator), typeof(Rigidbody2D))]
public class StateDano : State
{
    [Tooltip("Trigger de dano no Animator")]
    [SerializeField] private string damageTrigger = "Hit";
    [Tooltip("Força do knockback (unidades por segundo)")]
    [SerializeField] private float knockbackForce = 5f;
    [Tooltip("Duração do knockback em segundos")]
    [SerializeField] private float knockbackDuration = 0.2f;

    private Animator _anim;
    private Enemy _enemy;
    private Rigidbody2D _rb;

    // estado interno de knockback
    private Vector2 _kbVelocity;
    private float _kbTimer;

    public override void Awake()
    {
        base.Awake();
        _anim  = GetComponent<Animator>();
        _enemy = GetComponent<Enemy>();
        _rb    = GetComponent<Rigidbody2D>();

        // transição para morte (já existia)
        var toMorte = new Transition {
            condition = new ConditionDeath(_enemy),
            target    = GetComponent<StateMorte>()
        };
        transitions.Add(toMorte);
    }

    public override void OnEnable()
    {
        base.OnEnable();
        Debug.Log($"[Knockback] dir={_kbVelocity} duration={_kbTimer}");

        // garante que o Attack seja desligado
        var atk = GetComponent<StateAtaque>();
        if (atk.enabled) atk.enabled = false;

        // dispara animação de hit
        _anim.SetTrigger(damageTrigger);

        // configura direção e timer do knockback
        Vector2 dir     = ((Vector2)transform.position - _enemy.LastDamageSource).normalized;
        _kbVelocity     = dir * knockbackForce;
        _kbTimer        = knockbackDuration;
    }

    public override void Update()
    {
        Debug.Log($"[Knockback] posição agora em { _rb.position }");
        base.Update();
        // aplica deslocamento de knockback se o timer ainda não zerou
        if (_kbTimer > 0f)
        {
            Vector2 novaPos = _rb.position + _kbVelocity * Time.deltaTime;
            _rb.MovePosition(novaPos);
            _kbTimer -= Time.deltaTime;
        }
    }

    // Animation Event no final da clip "Damage"
    public void OnDamageAnimationEnd()
    {
        Debug.Log(">> StateDano: OnDamageAnimationEnd limpando knockback");
        _kbTimer = 0f;
        _enemy.ClearDamage();
        this.enabled = false;
        GetComponent<StatePatrulha>().enabled = true;
    }
}
