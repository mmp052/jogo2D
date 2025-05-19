using UnityEngine;    
using FSM;
public class StatePatrulha: State
{
    public Transform[] waypoints;
    int nextWaypoint;
    float speed = 2.0f;
    Rigidbody2D rb;
    Enemy me;
    private Animator _animator;
    private float _direction = 1;

    [SerializeField] 
    private float _distanceToPoint = 1.5f;

    public override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
        me = GetComponent<Enemy>();
        _animator = GetComponent<Animator>();

        // Criamos e populamos uma nova transição
        Transition ToAtaque = new Transition();
        ToAtaque.condition = new ConditionCollide(me);
        ToAtaque.target = GetComponent<StateAtaque>();
        // Adicionamos a transição em nossa lista de transições
        transitions.Add(ToAtaque);
        // Garanto que o player estará no primeiro waypoint
        // quando inicializado o jogo
        // transform.position = waypoints[0].position + transform.lossyScale;
        nextWaypoint = 0;

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

    public override void Update()
    {
        float speed = me.VelocidadePatrulha;
        int dir = normalizedDirection();
        rb.linearVelocity = new Vector2(speed * dir, rb.linearVelocity.y);
        transform.localScale = new Vector3(dir, 1, 1);
        _animator.SetFloat("Speed", Mathf.Abs(speed));

        if (normalizedDirection() > 0)
        {
            speed = speed * 1.0f;
            // transform.localScale = new Vector3(1, 1, 1); // Inverte o sprite para a direita
        }
        else
        {
            speed = speed * -1.0f;
            // transform.localScale = new Vector3(-1, 1, 1); // Inverte o sprite para a esquerda
        }

        if (Vector2.Distance(transform.position, waypoints[nextWaypoint].position) < _distanceToPoint)
        {
            nextWaypoint++;
            if (nextWaypoint >= waypoints.Length) nextWaypoint = 0;
        }

        // rb.linearVelocity = new Vector3(speed, 0);

        // _animator.SetFloat("Speed", Mathf.Abs(speed)); // Atualiza a animação de corrida com base na velocidade do Rigidbody2D


    }

    public override void LateUpdate()
    {
        foreach (Transition t in transitions)
        {
            // Para cada transição que esse estado tiver// é feita a verificação de sua condiçãoforeach (Transition t in transitions) {
            if (t.condition.Test())
            {
                t.target.enabled = true;
                this.enabled = false;
                rb.linearVelocity = Vector3.zero; // Para o movimento do inimigo ao mudar de estado
                _animator.SetFloat("Speed", 0); // Para a animação de corrida ao mudar de estado
                return;
            }
        }
    }

    int normalizedDirection() {
        if (Mathf.Abs(_direction) > 0.2)
        {
            _direction = waypoints[nextWaypoint].position.x - transform.position.x;
        }

        return (int)(_direction / Mathf.Abs(_direction));
    }

}