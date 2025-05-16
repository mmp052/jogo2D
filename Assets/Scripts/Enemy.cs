using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int _vida = 60;
    public int Vida => _vida;

    // sinaliza se o player está em alcance
    public bool flagEnterAttack { get; private set; }

    private int _defesa = 2;

    // sinaliza que levou dano
    private bool _damaged = false;
    public bool Damaged => _damaged;
    public void ClearDamage() => _damaged = false;

    // guarda de onde veio o dano
    private Vector2 _lastDamageSource;
    public Vector2 LastDamageSource => _lastDamageSource;

    public void TakeDamage(Vector2 from, int damage)
    {
        // registra a origem do hit
        _lastDamageSource = from;

        // aplica defesa e subtrai vida
        damage -= _defesa;
        _vida   -= damage;

        if (_vida <= 0)
        {
            Debug.Log($">>> TakeDamage: Vida chegou a {_vida}, entrando em StateMorte");

            // ① desliga tudo que pode estar ligado
            var patrol = GetComponent<StatePatrulha>();
            if (patrol && patrol.enabled) patrol.enabled = false;
            var attack = GetComponent<StateAtaque>();
            if (attack && attack.enabled) attack.enabled = false;
            var dano = GetComponent<StateDano>();
            if (dano && dano.enabled) dano.enabled = false;

            // ② dispara trigger e habilita o StateMorte
            GetComponent<Animator>().SetTrigger("Death");
            var morte = GetComponent<StateMorte>();
            if (morte)
            {
                morte.enabled = true;
            }
            else
            {
                Debug.LogError("StateMorte não encontrado no GameObject!");
            }
        }
        else
        {
            // sinaliza dano e aciona state de dano
            _damaged = true;
            GetComponent<Animator>().SetTrigger("Hit");
            GetComponent<StateDano>().enabled = true;
        }
    }

    // agora público para ser chamado pelo StateMorte
    public void Die()
    {
        Debug.Log("Inimigo morreu!");
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            flagEnterAttack = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            flagEnterAttack = false;
    }
}
