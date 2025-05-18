using UnityEngine;
using FSM;
<<<<<<< HEAD

public class StateAtaque : State
{
    private Animator _animator;
    private BoxCollider2D _hitbox;
=======
using System.Collections;

public class StateAtaque : State
{
    private Animator     _animator;
    private BoxCollider2D _hitbox;
    private Enemy         me;              // ① declare o Enemy
    [SerializeField]
    private GameObject    _player;
    private AudioSource   _audioSource;
>>>>>>> 07ec0b5fcdb43dd9f2f3417bf5dd1e1c2f8c276a

    public override void Awake()
    {
        base.Awake();
<<<<<<< HEAD
        Transition ToPatrulha = new Transition();
        ToPatrulha.condition = new Condition();
        ToPatrulha.target = GetComponent<StateAtaque>();

    }
    public override void OnEnable()
    {
        base.OnEnable();
        _animator = GetComponent<Animator>();
        _hitbox = GetComponents<BoxCollider2D>()[1];

        _hitbox.enabled = true;
        _animator.SetTrigger("Attack");
    }

    public override void OnDisable()
    {
        base.OnDisable();
        _hitbox.enabled = false;
    }
}
=======
        _animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
        me = GetComponent<Enemy>();   // ② atribua aqui

        // criação explícita da transição de dano:
        Transition toDano = new Transition();
        toDano.condition = new ConditionDamage(me);
        toDano.target = GetComponent<StateDano>();
        transitions.Add(toDano);
        
        var toMorte = new Transition();
        toMorte.condition = new ConditionDeath(me);
        toMorte.target    = GetComponent<StateMorte>();
        transitions.Add(toMorte);
    }

    public override void OnEnable()
    {
        base.OnEnable();
        _audioSource.Play();
        _animator.SetTrigger("Attack");
        _player = GameObject.FindGameObjectWithTag("Player");

        // vira o sprite na direção do player
        if (_player.transform.position.x > transform.position.x)
            transform.localScale = Vector3.one;
        else
            transform.localScale = new Vector3(-1, 1, 1);
    }
    private void OnDisable()
    {
        // sempre que o Attack for desativado, limpa o hitbox
        if (_hitbox != null)
            _hitbox.enabled = false;
    }

    public void iniciarAtaque()
    {
        _hitbox = GetComponents<BoxCollider2D>()[1];
        _hitbox.enabled = true;
        StartCoroutine(CheckHitNextFrame());
    }

    public void finalizarAtaque()
    {
        // não precisa mais desativar o hitbox aqui, OnDisable já faz
        GetComponent<StatePatrulha>().enabled = true;
        this.enabled = false;
    }

    private IEnumerator CheckHitNextFrame()
    {
        yield return new WaitForFixedUpdate();
        Collider2D[] hits = Physics2D.OverlapBoxAll(
            _hitbox.bounds.center,
            _hitbox.bounds.size,
            0f,
            LayerMask.GetMask("Player")
        );

        foreach (var hit in hits)
        {
            var pm = hit.GetComponent<PlayerMovement>();
            if (pm != null)
                pm.TakeDamage(transform.position, me.Dano);
        }
    }
}
>>>>>>> 07ec0b5fcdb43dd9f2f3417bf5dd1e1c2f8c276a
