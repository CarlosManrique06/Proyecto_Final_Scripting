using System.Collections;
using UnityEngine;

public class Yoyo : MonoBehaviour
{


    public float speed = 14f;


    public float maxDistance = 5f;


    public float returnThreshold = 0.3f;


    public int damage = 1;


    public string enemyTag = "enemy";

    public LineRenderer stringRenderer;


    // State

    public bool IsActive { get; private set; }

    private enum State { Going, Returning }
    private State _state;

    private Transform _owner;
    private Vector2 _direction;
    private Vector2 _originPos;


    private Rigidbody2D _rb;


    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();

        if (stringRenderer == null)
            stringRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        if (!IsActive) return;

        Move();
        DrawString();
        CheckReturn();
    }


    public void Launch(Vector2 direction, Transform owner)
    {
        _direction = direction.normalized;
        _owner = owner;
        _originPos = transform.position;
        _state = State.Going;
        IsActive = true;
    }

    void Move()
    {
        if (_state == State.Going)
        {
            // Lanzarlo hacia adelante
            transform.Translate(_direction * speed * Time.deltaTime);

            
            float distTravelled = Vector2.Distance(_originPos, transform.position);
            if (distTravelled >= maxDistance)
                _state = State.Returning;
        }
        else 
        {
           
            Vector2 toOwner = ((Vector2)_owner.position - (Vector2)transform.position).normalized;
            transform.Translate(toOwner * speed * Time.deltaTime);
        }
    }

    void CheckReturn()
    {
        if (_state != State.Returning) return;

        float dist = Vector2.Distance(transform.position, _owner.position);
        if (dist <= returnThreshold)
            Deactivate();
    }


    void DrawString()
    {
        if (stringRenderer == null || _owner == null) return;

        stringRenderer.positionCount = 2;
        stringRenderer.SetPosition(0, _owner.position);
        stringRenderer.SetPosition(1, transform.position);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //Daño del yoyo
        if (!IsActive) return;

        if (other.CompareTag(enemyTag))
        {
           
            IDamage damageable = other.GetComponent<IDamage>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
            }

            
             _state = State.Returning; //Activar o descativar el retorno al golpear un enemigo, lo traspasa si no se pone y si se quita sigue
        }
    }


    void Deactivate()
    {
        IsActive = false;
        if (stringRenderer != null)
            stringRenderer.positionCount = 0;

        Destroy(gameObject);
    }
}
