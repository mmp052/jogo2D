using UnityEngine;
using FSM;

[RequireComponent(typeof(Animator))]
public class StateMorte : State
{
    [SerializeField] private string deathTrigger = "Death";
    private Animator _anim;
    private Enemy    _enemy;

    public override void Awake()
    {
        base.Awake();
        _anim  = GetComponent<Animator>();
        _enemy = GetComponent<Enemy>();
    }

    public override void OnEnable()
    {
        base.OnEnable();
        // dispara o trigger — Animator vai para o state "Death"
        _anim.SetTrigger(deathTrigger);
    }

    // Animation Event no fim da clip "Death":
    public void OnDeathAnimationEnd()
    {
        _enemy.Die();
    }
}
