using System;
using UnityEngine;

public class Yoyo : MonoBehaviour
{
    public float speed = 14f;
    public float maxDistance = 5f;
    public float returnDistance = 0.35f;

    public float oscillationAmplitude = 0.35f;
    public float oscillationFrequency = 9f;

    public int damage = 1;
    public string enemyTag = "enemy";

    public LineRenderer lineRenderer;

    
    public bool isActive;
    public bool isAnchored;

    private enum State
    {
        Going,
        Returning,
        GoingToAnchor,
        Anchored
    }

    private State currentState;

    private Transform owner;
    private Vector2 direction;
    private Vector2 startPosition;
    private Vector2 anchorPosition;
    private float oscillationTime;
    private Action onAnchorReached;

    void Awake()
    {
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }
    }

    void Update()
    {
        if (isActive == false)
        {
            return;
        }

        Move();
        DrawString();
    }

    // Lanzamiento normal
    public void Launch(Vector2 dir, Transform player)
    {
        direction = dir.normalized;
        owner = player;
        startPosition = transform.position;
        oscillationTime = 0f;

        currentState = State.Going;
        isActive = true;
    }

    // Lanzamiento hacia punto
    public void LaunchToAnchor(Vector2 target, Transform player, Action callback)
    {
        anchorPosition = target;
        owner = player;

        direction = (target - (Vector2)transform.position).normalized;

        onAnchorReached = callback;

        currentState = State.GoingToAnchor;
        isActive = true;
    }

   
    public void ReturnToOwner()
    {
        isAnchored = false;
        currentState = State.Returning;
    }

    // Movimiento principal
    void Move()
    {
        if (currentState == State.Going)
        {
            oscillationTime = oscillationTime + Time.deltaTime;

            Vector2 perpendicular = Vector2.Perpendicular(direction);
            float oscillation = Mathf.Cos(oscillationTime * oscillationFrequency) * oscillationAmplitude * oscillationFrequency;

            Vector2 movement = direction * speed + perpendicular * oscillation;

            transform.Translate(movement * Time.deltaTime, Space.World);

            float distance = Vector2.Distance(startPosition, transform.position);

            if (distance >= maxDistance)
            {
                currentState = State.Returning;
            }
        }
        else if (currentState == State.Returning)
        {
            Vector2 directionToPlayer = ((Vector2)owner.position - (Vector2)transform.position).normalized;

            transform.Translate(directionToPlayer * speed * Time.deltaTime, Space.World);

            float distanceToPlayer = Vector2.Distance(transform.position, owner.position);

            if (distanceToPlayer <= returnDistance)
            {
                Deactivate();
            }
        }
        else if (currentState == State.GoingToAnchor)
        {
            transform.Translate(direction * speed * Time.deltaTime, Space.World);

            float distanceToTarget = Vector2.Distance(transform.position, anchorPosition);

            if (distanceToTarget <= 0.18f)
            {
                transform.position = anchorPosition;

                currentState = State.Anchored;
                isAnchored = true;

                if (onAnchorReached != null)
                {
                    onAnchorReached();
                }
            }
        }
        else if (currentState == State.Anchored)
        {
            transform.position = anchorPosition;
        }
    }

    
    void DrawString()
    {
        if (lineRenderer == null || owner == null)
        {
            return;
        }

        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, owner.position);
        lineRenderer.SetPosition(1, transform.position);
    }

    // Daño a enemigos
    void OnTriggerEnter2D(Collider2D other)
    {
        if (isActive == false || isAnchored == true)
        {
            return;
        }

        if (other.CompareTag(enemyTag))
        {
            IDamage damageComponent = other.GetComponent<IDamage>();

            if (damageComponent != null)
            {
                damageComponent.TakeDamage(damage);
            }
            ReturnToOwner(); // El yoyo regresa al jugador después de golpear a un enemigo, si se quita , el yoyo seguirá su trayectoria normal incluso después de golpear a un enemigo.
        }
    }

    
    void Deactivate()
    {
        isActive = false;
        isAnchored = false;

        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 0;
        }

        Destroy(gameObject);
    }
}