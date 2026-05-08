using System;
using UnityEngine;

public class Yoyo : MonoBehaviour
{
    public float speed = 14f;
    public float maxDistance = 5f;
    public float returnDistance = 0.35f;

    public int damage = 1;
    public string enemyTag = "enemy";

    public LineRenderer lineRenderer;

    public bool isActive;
    public bool isAnchored;

    private enum State { Going, Returning, GoingToAnchor, Anchored }
    private State currentState;

    private Transform owner;
    private Vector2 direction;
    private Vector2 startPosition;
    private Vector2 anchorPosition;
    private Action onAnchorReached;

    void Awake()
    {
        if (lineRenderer == null)
            lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        if (!isActive) return;
        Move();
        DrawString();
    }

    public void Launch(Vector2 dir, Transform player)
    {
        direction = dir.normalized;
        owner = player;
        startPosition = transform.position;
        currentState = State.Going;
        isActive = true;
    }

    // Mantener compatibilidad con SwingController
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

    // Mantener compatibilidad con YoyoPool
    public void ResetState()
    {
        isActive = false;
        isAnchored = false;
        owner = null;
        onAnchorReached = null;
        if (lineRenderer != null) lineRenderer.positionCount = 0;
    }

    void Move()
    {
        if (currentState == State.Going)
        {
            // avanza recto hasta la distancia máxima
            transform.Translate(direction * speed * Time.deltaTime, Space.World);

            float distanceTravelled = Vector2.Distance(transform.position, startPosition);
            if (distanceTravelled >= maxDistance)
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
                isAnchored = true;
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
        if (!isActive || isAnchored) return;
        if (other.CompareTag(enemyTag))
        {
            other.GetComponent<IDamage>()?.TakeDamage(damage);
            ReturnToOwner();
        }
    }

    void Deactivate()
    {
        isActive = false;
        isAnchored = false;
        if (lineRenderer != null) lineRenderer.positionCount = 0;

        // Mantener compatibilidad con el pool
        if (YoyoPool.Instance != null)
            YoyoPool.Instance.Return(gameObject);
        else
            Destroy(gameObject);
    }
}
