using UnityEngine;

public class EnemyAnimation : MonoBehaviour
{
    private Animator _animator;
    private Player _player;
    private Rigidbody2D _rb;

    void Start()
    {
        _animator = GetComponent<Animator>();
        _player = FindObjectOfType<Player>();
        _rb = GetComponent<Rigidbody2D>();
    }

    public void Update()
    {
        Run();
    }

    public void Run()
    {
        var speed = _rb.linearVelocity.x; // Obtém a velocidade do Rigidbody2D
        if(speed > 0)
        {
            transform.localScale = new Vector3(1, 1, 1); // Inverte o sprite para a direita
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1); // Inverte o sprite para a esquerda
        }
        _animator.SetFloat("Speed", Mathf.Abs(speed)); // Atualiza a animação de corrida com base na velocidade do Rigidbody2D
    }
}