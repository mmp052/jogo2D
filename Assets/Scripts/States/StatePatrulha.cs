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

        nextWaypoint = 1;

        
    }

    public override void Update()
    {
        if (normalizedDirection() > 0)
        {
            speed = 2.0f;
            transform.localScale = new Vector3(1, 1, 1); // Inverte o sprite para a direita
        }
        else
        {
            speed = -2.0f;
            transform.localScale = new Vector3(-1, 1, 1); // Inverte o sprite para a esquerda
        }

        if (Vector2.Distance(transform.position, waypoints[nextWaypoint].position) < _distanceToPoint)
        {
            nextWaypoint++;
            if (nextWaypoint >= waypoints.Length) nextWaypoint = 0;
        }

        rb.linearVelocity = new Vector3(speed, 0);

        _animator.SetFloat("Speed", Mathf.Abs(speed)); // Atualiza a animação de corrida com base na velocidade do Rigidbody2D


    }

    int normalizedDirection() {
        // Debug.Log(waypoints[nextWaypoint].position.x + " - " + transform.position.x + " = " + direction);
        if (Mathf.Abs(_direction) > 0.2)
        {
            _direction = waypoints[nextWaypoint].position.x - transform.position.x;
        }

        return (int)(_direction / Mathf.Abs(_direction));
    }

}