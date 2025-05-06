using UnityEngine;
using FSM;

public class StateAtaque : State
{
    private Animator _animator;
    private BoxCollider2D _hitbox;

    public override void Awake()
    {
        base.Awake();
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