using UnityEngine;    
using FSM;
public class StatePatrulha: State
{
    public Transform[] waypoints;
    int nextWaypoint;
    float speed = 2.0f;
    Rigidbody2D rb;

    [SerializeField] 
    private float _distanceToPoint = 1.5f;

    public override void Awake()
    {
        base.Awake();
        // Criamos e populamos uma nova transição
        Transition ToPerseguindo = new Transition();
        ToPerseguindo.condition = new ConditionDistLT(transform,
                GameObject.FindWithTag("Player").transform,
                2.0f);
        ToPerseguindo.target = GetComponent<StatePerseguindo>();
        // Adicionamos a transição em nossa lista de transições
        transitions.Add(ToPerseguindo);
        // Garanto que o player estará no primeiro waypoint
        // quando inicializado o jogo
        // transform.position = waypoints[0].position + transform.lossyScale;

        nextWaypoint = 1;

        rb = GetComponent<Rigidbody2D>();

    }

    public override void Update()
    {
        if (normalizedDirection() > 0)
            speed = 2.0f;
        else
            speed = -2.0f;

        if (Vector2.Distance(transform.position, waypoints[nextWaypoint].position) < _distanceToPoint)
        {
            nextWaypoint++;
            if (nextWaypoint >= waypoints.Length) nextWaypoint = 0;
        }

        rb.linearVelocity = new Vector3(speed, 0);

    }

    int normalizedDirection() {
        float direction = waypoints[nextWaypoint].position.x - transform.position.x;
        Debug.Log(waypoints[nextWaypoint].position.x + " - " + transform.position.x + " = " + direction);
        return (int)(direction / Mathf.Abs(direction));
    }

}