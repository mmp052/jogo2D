using UnityEngine;
using FSM;
using System.Collections;

public class StateAtaque : State
{
    private Animator _animator;
    private BoxCollider2D _hitbox;
    [SerializeField]
    private GameObject _player;
    private AudioSource _audioSource;

    public override void Awake()
    {
        _animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
    }
    public override void OnEnable()
    {
        _audioSource.Play();
        _animator.SetTrigger("Attack");
        _player = GameObject.FindGameObjectWithTag("Player");
        if(_player.transform.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(1, 1, 1); // Inverte o sprite para a direita
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1); // Inverte o sprite para a esquerda
        }
    }

    public void iniciarAtaque()
    {
        _hitbox = GetComponents<BoxCollider2D>()[1];
        _hitbox.enabled = true;

        StartCoroutine(CheckHitNextFrame());
    }

    public void finalizarAtaque()
    {
        _hitbox.enabled = false;
        GetComponent<StatePatrulha>().enabled = true;
        this.enabled = false;
        return;
    }

    private IEnumerator CheckHitNextFrame()
    {
        yield return new WaitForFixedUpdate(); // aguarda a física

        // pega as informações de tamanho e posição da hitbox
        var cb = _hitbox;
        Vector2 center = cb.bounds.center;
        Vector2 size = cb.bounds.size;
        Collider2D[] hits = Physics2D.OverlapBoxAll(center, size, 0f, LayerMask.GetMask("Player"));

        foreach (var hit in hits)
        {
            var pm = hit.GetComponent<PlayerMovement>();
            if (pm != null)
            {
                pm.TakeDamage(new Vector2(transform.position.x, transform.position.y), 10);
            }
        }
    }
}