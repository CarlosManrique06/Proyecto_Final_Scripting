using System;
using UnityEngine;

public class Yoyo : MonoBehaviour
{

    [SerializeField] private float speed = 14f;
    [SerializeField] private float maxDistance = 5f;
    [SerializeField] private float returnDistance = 0.35f;
    [SerializeField] private int damage = 1;
    [SerializeField] private string enemyTag = "enemy";
    [SerializeField] private LineRenderer lineRenderer;

    

    public bool IsActive { get; private set; }
    public bool IsAnchored { get; private set; }

    

    private enum State { Going, Returning, GoingToAnchor, Anchored }
    private State currentState;

    private Transform owner;
    private Vector2 direction;
    private Vector2 anchorPosition;
    private Action onAnchorReached;


    void Awake()
    {
        if (lineRenderer == null)
            lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        if (!IsActive) return;
        Move();
        DrawString();
    }

   

    public void Launch(Vector2 dir, Transform player)
    {
        direction = dir.normalized;
        owner = player;
        currentState = State.Going;
        IsActive = true;
    }

    public void LaunchToAnchor(Vector2 target, Transform player, Action callback)
    {
        anchorPosition = target;
        owner = player;
        direction = (target - (Vector2)transform.position).normalized;
        onAnchorReached = callback;
        currentState = State.GoingToAnchor;
        IsActive = true;
    }

    public void ReturnToOwner()
    {
        IsAnchored = false;
        currentState = State.Returning;
    }

    public void ResetState()
    {
        IsActive = false;
        IsAnchored = false;
        owner = null;
        onAnchorReached = null;
        if (lineRenderer != null) lineRenderer.positionCount = 0;
    }

  
    //  Movimiento — private, nadie más lo llama
    

    void Move()
    {
        if (currentState == State.Going)
        {
            transform.Translate(direction * speed * Time.deltaTime, Space.World);

            if (Vector2.Distance(transform.position, owner.position) >= maxDistance)
                currentState = State.Returning;
        }
        else if (currentState == State.Returning)
        {
            Vector2 toOwner = ((Vector2)owner.position - (Vector2)transform.position).normalized;
            transform.Translate(toOwner * speed * Time.deltaTime, Space.World);

            if (Vector2.Distance(transform.position, owner.position) <= returnDistance)
                Deactivate();
        }
        else if (currentState == State.GoingToAnchor)
        {
            transform.Translate(direction * speed * Time.deltaTime, Space.World);

            if (Vector2.Distance(transform.position, anchorPosition) <= 0.18f)
            {
                transform.position = anchorPosition;
                currentState = State.Anchored;
                IsAnchored = true;
                onAnchorReached?.Invoke();
            }
        }
        else if (currentState == State.Anchored)
        {
            transform.position = anchorPosition;
        }
    }

    void DrawString()
    {
        if (lineRenderer == null || owner == null) return;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, owner.position);
        lineRenderer.SetPosition(1, transform.position);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsActive || IsAnchored) return;
        if (other.CompareTag(enemyTag))
        {
            other.GetComponent<IDamage>()?.TakeDamage(damage);
            ReturnToOwner();
        }
    }

    void Deactivate()
    {
        IsActive = false;
        IsAnchored = false;
        if (lineRenderer != null) lineRenderer.positionCount = 0;
        gameObject.SetActive(false);
    }
}